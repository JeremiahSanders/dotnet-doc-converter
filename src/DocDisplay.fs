namespace Jds.DotnetDocConverter

open System.Text
open DocParsing
open Markdown

module DocDisplay =
    let asConsoleString (assemblyDocs : AssemblyDocs) =
        let assembly = sprintf "Assembly: %s" assemblyDocs.Assembly.Name

        let members =
            assemblyDocs.Members
            |> List.map (fun memberInfo ->
                   let summary =
                       if memberInfo.Summary = "" then ""
                       else (sprintf ": %s" memberInfo.Summary)

                   let remarks =
                       if memberInfo.Remarks = "" then ""
                       else
                           (sprintf "\n%s\n" memberInfo.Remarks)

                   sprintf "%s (%O)%s%s" memberInfo.ShortName memberInfo.MemberType summary remarks)
            |> String.concat "\n"
        sprintf "%s\n\nMembers:\n%s" assembly members

    let private getTypeMembers (typeMember : MemberDoc) (memberDocs : MemberDoc seq) =
        if (typeMember.MemberType <> MemberType.TypeMember) then Seq.empty
        else
            memberDocs
            |> Seq.filter
                   (fun memberDoc ->
                   (memberDoc.FullName <> typeMember.FullName) && memberDoc.FullName.StartsWith(typeMember.FullName))

    let private getSummary (memberDoc : MemberDoc) =
        if (memberDoc.Summary = "") then ""
        else
            sprintf "\n%s" memberDoc.Summary

    let private getRemarks (memberDoc : MemberDoc) =
        if (memberDoc.Remarks = "") then ""
        else
            sprintf "\n%s" (blockquote memberDoc.Remarks)

    let private addSummaryAndRemarks (memberDoc : MemberDoc) existingValue =
        sprintf "%s\n%s%s" existingValue (getSummary memberDoc) (getRemarks memberDoc)

    let private typeMemberToMarkdown (memberDoc : MemberDoc) =
        memberDoc.ShortName
        |> (code >> h2)
        |> (addSummaryAndRemarks memberDoc)

    let membersToMarkdown (memberDocs : MemberDoc list) =
        StringBuilder()
        |> (fun initialStringBuilder ->
        let memberMap =
            memberDocs
            |> List.groupBy (fun memberDoc -> memberDoc.MemberType)
            |> Map.ofList

        let typeMembers = memberMap.[MemberType.TypeMember]
        typeMembers
        |> List.fold (fun (typeMemberFoldingStringBuilder : StringBuilder) typeMember ->
               typeMemberFoldingStringBuilder
               |> StringBuilder.appendLine (typeMemberToMarkdown typeMember)
               |> StringBuilder.appendLine ""
               |> (fun sb ->
               getTypeMembers typeMember memberDocs
               |> Seq.map (fun typeMember ->
                      typeMember.ShortName
                      |> (fun memberName ->
                      sprintf "%s (%s)" (code memberName) (getMemberTypeName typeMember.MemberType))
                      |> h3
                      |> (addSummaryAndRemarks typeMember))
               |> (fun memberNames -> StringBuilder.appendLines memberNames sb))
               |> StringBuilder.appendLine "") initialStringBuilder)
        |> StringBuilder.toString

    let asMarkdownDocument (assemblyDocs : AssemblyDocs) =
        StringBuilder()
        |> StringBuilder.appendLine (h1 assemblyDocs.Assembly.Name)
        |> StringBuilder.appendLine ""
        |> StringBuilder.append (membersToMarkdown assemblyDocs.Members)
        |> StringBuilder.toString

    // Convert MemberDoc to Markdown document
    let private getTypeMemberSummary (memberNode : MemberDoc) : BlockNode list =
        memberNode.Summary |> (BlockNode.ofString >> List.singleton)

    let private getTypeMemberRemarks (memberNode : MemberDoc) : BlockNode list =
        memberNode.Remarks
        |> (blockquote
            >> BlockNode.ofString
            >> List.singleton)

    let private getTypeMemberBlockNodes (memberNode : MemberDoc) : BlockNode list =
        let getTypeMemberHeader node = [ InlineNode.ofString (code node.ShortName) ] |> BlockNode.ofNodes
        getTypeMemberHeader memberNode
        |> List.singleton
        |> (fun blockList ->
        if (memberNode.Summary <> "") then (List.append blockList (getTypeMemberSummary memberNode))
        else blockList)
        |> (fun blockList ->
        if (memberNode.Remarks <> "") then (List.append blockList (getTypeMemberRemarks memberNode))
        else blockList)

    let private getTypesMembersBlockNodes (typeMember : MemberDoc) (nonTypes : MemberDoc list) : BlockNode list =
        let getNamespaceBlockNode (namespaceValue : string) : BlockNode =
            [ (InlineNode.ofString "Namespace: ")
              (namespaceValue |> (code >> InlineNode.ofString)) ]
            |> BlockNode.ofNodes

        let getFullNameBlockNode (fullName : string) : BlockNode =
            [ (InlineNode.ofString "Full name: ")
              (fullName |> (code >> InlineNode.ofString)) ]
            |> BlockNode.ofNodes

        let getTypeHeader (memberDoc : MemberDoc) : BlockNode list =
            (code memberDoc.ShortName)
            |> (h2
                >> InlineNode.ofString
                >> BlockNode.ofNode)
            |> (fun nameBlock ->
            [ nameBlock
              (getNamespaceBlockNode memberDoc.Namespace)
              (getFullNameBlockNode memberDoc.FullName) ])
            |> (fun blockList ->
            if (memberDoc.Summary <> "") then (List.append blockList (getTypeMemberSummary memberDoc))
            else blockList)
            |> (fun blockList ->
            if (memberDoc.Remarks <> "") then (List.append blockList (getTypeMemberRemarks memberDoc))
            else blockList)

        nonTypes
        |> getTypeMembers typeMember
        |> Seq.sortBy (fun typeMember -> (typeMember.MemberType, typeMember.ShortName))
        |> Seq.collect getTypeMemberBlockNodes
        |> Seq.toList
        |> List.append (getTypeHeader typeMember)

    let private isTypeMember memberDoc = memberDoc.MemberType = MemberType.TypeMember

    let private getTypeBlockNodes (assemblyDocs : AssemblyDocs) : BlockNode list =
        assemblyDocs.Members
        |> List.partition isTypeMember
        |> (fun (types, nonTypes) ->
        types
        |> List.map (fun (typeMember : MemberDoc) -> getTypesMembersBlockNodes typeMember nonTypes)
        |> List.concat)

    let assemblyDocsToMarkdownDocumentNode (assemblyDocs : AssemblyDocs) : DocumentNode =
        assemblyDocs.Assembly.Name
        |> (h1 >> BlockNode.ofString)
        |> List.singleton
        |> (fun assemblyHeader -> List.append assemblyHeader (getTypeBlockNodes assemblyDocs))
        |> DocumentNode.ofBlocks

    // Convert Markdown document nodes to string
    let markdownBlockNodeToString (block : BlockNode) =
        block.Nodes
        |> List.map (fun inlineNode -> inlineNode.Value)
        |> String.concat (" ")
        |> sprintf "%s\n"

    let markdownDocumentNodeToString (markdown : Markdown.DocumentNode) =
        markdown.Blocks
        |> List.fold (fun sb block -> StringBuilder.appendLine (markdownBlockNodeToString block) sb) (StringBuilder())
        |> StringBuilder.toString
