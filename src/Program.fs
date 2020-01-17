module Jds.DotnetDocConverter.Program

open System
open System.Text
open ArgParsing
open DocParsing
open DocDisplay
open FileLoading

let generateAppHeader() = "Convert .NET XML documentation to Markdown\n\n"

let generateUsage() =
    StringBuilder()
    |> StringBuilder.appendLine "Usage: dotnet docconvert <DOC_LOCATION> [options]"
    |> StringBuilder.appendLine ""
    |> StringBuilder.appendLine "Arguments:"
    |> StringBuilder.appendLine "  <DOC_LOCATION>              Path to XML file or directory."
    |> StringBuilder.appendLine ""
    |> StringBuilder.appendLine "Options:"
    |> StringBuilder.appendLine " -o, --output <DIRECTORY>     Path to save converted Markdown files."
    |> StringBuilder.toString

let generateDocs docFiles =
    docFiles
    |> List.map (fun docPath ->
           (loadXmlFile >> parseAssemblyDocs) docPath
           |> (fun assemblyDocs ->
           (//              (assemblyDocs, (asMarkdownDocument assemblyDocs))
            assemblyDocs,
            (assemblyDocs
             |> assemblyDocsToMarkdownDocumentNode
             |> markdownDocumentNodeToString))))

let saveAssemblyDocument appArguments (assemblyDocument, markdown) =
    let fileName = assemblyDocument.Assembly.Name + ".md"
    let destinationPath = System.IO.Path.Combine(appArguments.OutputDirectory, fileName)
    FileSaving.saveFile destinationPath markdown |> ignore
    destinationPath |> sprintf "Saved %s"

let generateDisplay appArguments =
    if (List.isEmpty appArguments.DocFiles) then generateUsage()
    else
        generateDocs appArguments.DocFiles
        |> Seq.map (saveAssemblyDocument appArguments)
        |> String.concat "\n\n"

[<EntryPoint>]
let main argv =
    argv
    |> parseArguments
    |> function
    | Success applicationArguments -> applicationArguments |> generateDisplay
    | Failure reason ->
        sprintf "Error: %s\n\n%s" reason (generateUsage())
    |> printfn "%s%s" (generateAppHeader())
    0 // return an integer exit code
