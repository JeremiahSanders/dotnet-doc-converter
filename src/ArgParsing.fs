namespace Jds.DotnetDocConverter

module ArgParsing =
    type ProgramArguments =
        { DocFiles : string list
          OutputDirectory : string
          RequestedNotFoundDocFiles : string list }

    let (|IsFile|_|) (path : string) =
        if ((System.IO.Path.GetExtension(path) = ".xml") && (System.IO.File.Exists(path))) then Some path
        else None

    let (|IsDirectory|_|) path =
        if (System.IO.Directory.Exists(path)) then Some path
        else None

    let private tryGetFiles path =
        match (System.IO.Path.GetFullPath(path)) with
        | IsFile filePath -> Some [ filePath ]
        | IsDirectory directoryPath ->
            let enumerationOptions = System.IO.EnumerationOptions()
            enumerationOptions.RecurseSubdirectories <- true
            System.IO.Directory.GetFiles(directoryPath, "*.xml", enumerationOptions)
            |> List.ofArray
            |> Some
        | _ -> None

    let tryGetOutputDirectory (options : string list) =
        let verifyDirectory outputDirectory =
            match (System.IO.Path.GetFullPath(outputDirectory)) with
            | IsDirectory outputDirectory -> Some outputDirectory
            | _ -> None
        match options with
        | [ "-o"; outputDirectory ] -> verifyDirectory outputDirectory
        | [ "--output"; outputDirectory ] -> verifyDirectory outputDirectory
        | _ -> None

    let private getOutputDirectory =
        function
        | IsFile filePath -> System.IO.Path.GetDirectoryName(filePath)
        | IsDirectory directoryPath -> directoryPath
        | _ -> "."

    type ParsingResult =
        | Success of ProgramArguments
        | Failure of string

    let parseArguments (arguments : string array) =
        arguments
        |> List.ofArray
        |> function
        | [] -> Failure "Documentation source required"
        | first :: rest ->
            first
            |> tryGetFiles
            |> Option.map (fun validFiles ->
                   { DocFiles = validFiles
                     OutputDirectory =
                         tryGetOutputDirectory rest |> Option.defaultWith (fun () -> getOutputDirectory first)
                     RequestedNotFoundDocFiles = [] })
            |> Option.map Success
            |> Option.defaultWith (fun () -> Failure "Failed to load documentation source")
