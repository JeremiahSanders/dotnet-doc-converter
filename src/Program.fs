module Jds.DotnetDocConverter.Program

open System
open System.Text
open ArgParsing
open DocParsing
open DocDisplay
open FileLoading

let generateUsage() =
    StringBuilder()
    |> StringBuilder.appendLine "Dotnet Doc Converter"
    |> StringBuilder.appendLine ""
    |> StringBuilder.appendLine "Usage:"
    |> StringBuilder.appendLine "  [filePath]"
    |> (fun stringBuilder -> stringBuilder.ToString())

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
    FileSaving.saveFile destinationPath markdown

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
    |> generateDisplay
    |> printfn "%s"
    0 // return an integer exit code
