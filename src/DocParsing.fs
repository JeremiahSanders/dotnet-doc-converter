namespace Jds.DotnetDocConverter

open System.Xml.Linq

// Reference : https://docs.microsoft.com/en-us/dotnet/csharp/codedoc
//             https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/processing-the-xml-file

module DocParsing =

    type AssemblyDoc =
        { Name: string }
    
    type MemberType =
        | NamespaceMember = 0
        | TypeMember = 1
        | FieldMember = 2
        | PropertyMember = 4
        | MethodMember = 8
        | EventMember = 16
        | ErrorString = 32
    
    let getMemberTypeName (memberType : MemberType) =
            match memberType with
            | MemberType.NamespaceMember -> "Namespace"
            | MemberType.TypeMember -> "Type"
            | MemberType.FieldMember -> "Field"
            | MemberType.PropertyMember -> "Property"
            | MemberType.MethodMember -> "Method"
            | MemberType.EventMember -> "Event"
            | MemberType.ErrorString -> "Compiler reference parsing error string"
            | _ -> ""

    type MemberDoc =
        { FullName: string
          MemberType: MemberType
          Namespace: string
          Remarks: string
          ShortName:string
          Summary: string }

    type AssemblyDocs =
        { Assembly: AssemblyDoc
          Members: MemberDoc list }

    [<AutoOpen>]
    module ValueParsing =
        let getAssembly (xDocument: XDocument) =
            xDocument.Descendants(XName.Get "assembly")
            |> Seq.collect (fun assemblyElement ->
                assemblyElement.Descendants(XName.Get "name") |> Seq.map (fun nameElement -> nameElement.Value))
            |> String.concat ", "
    
        let parseMemberName (xElement: XElement) = xElement.Attribute(XName.Get "name").Value
        let (|StringPrefix|_|) (prefix:string) (value:string) =
            if value.StartsWith(prefix) then
                Some(value.Substring(prefix.Length))
            else
                None
        let parseMemberType name =
            match name with
            | StringPrefix "N:" memberName -> (MemberType.NamespaceMember, memberName)
            | StringPrefix "T:" typeName -> (MemberType.TypeMember, typeName)
            | StringPrefix "F:" fieldName -> (MemberType.FieldMember, fieldName)
            | StringPrefix "P:" propertyName -> (MemberType.PropertyMember, propertyName)
            | StringPrefix "M:" methodName -> (MemberType.MethodMember, methodName)
            | StringPrefix "E:" eventName -> (MemberType.EventMember, eventName)
            | StringPrefix "!:" errorString -> (MemberType.ErrorString, errorString)
            | _ -> (MemberType.ErrorString, name)
    
        let parseSingleValueDescenants xPath (xElement: XElement) =
            xElement.Descendants(XName.Get xPath)
            |> Seq.map (fun summaryElement -> summaryElement.Value.Trim())
            |> String.concat "\n"
    
        let splitNamespaceAndMember (fullName:string) =
            let divider = fullName.LastIndexOf('.')
            ((fullName.Substring(0,divider)), (fullName.Substring(divider+1)))
    let private getMemberName (fullName:string) =
        (fullName.Split('(')).[0]
        |> splitNamespaceAndMember
    let parseMemberDocs (xDocument: XDocument) =
        xDocument.Descendants(XName.Get "members")
        |> Seq.collect (fun membersElement ->
            membersElement.Descendants(XName.Get "member")
            |> Seq.map (fun memberElement ->
                let (memberType, memberName) = parseMemberType (parseMemberName memberElement)
                let (namespaceValue, memberLocalName) = getMemberName memberName
                { FullName = memberName
                  MemberType = memberType
                  Namespace = namespaceValue
                  Remarks = parseSingleValueDescenants "remarks" memberElement
                  ShortName = memberLocalName
                  Summary = parseSingleValueDescenants "summary" memberElement }))
        |> Seq.toList

    let parseAssemblyDocs (xDocument: XDocument) =
        { Assembly = { Name = getAssembly xDocument }
          Members = parseMemberDocs xDocument }
