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
        match path with
        | IsFile filePath -> Some [ filePath ]
        | IsDirectory directoryPath ->
            let enumerationOptions = System.IO.EnumerationOptions()
            enumerationOptions.RecurseSubdirectories <- true
            System.IO.Directory.GetFiles(directoryPath, "*.xml", enumerationOptions)
            |> List.ofArray
            |> Some
        | _ -> None

    let tryGetOutputDirectory (options : string list) =
        match options with
        | [ "-o"; outputDirectory ] ->
            match outputDirectory with
            | IsDirectory outputDirectory -> Some outputDirectory
            | _ -> None
        | _ -> None

    let private getOutputDirectory =
        function
        | IsFile filePath -> System.IO.Path.GetDirectoryName(filePath)
        | IsDirectory directoryPath -> directoryPath
        | _ -> "."

    let parseArguments (arguments : string array) =
        arguments
        |> List.ofArray
        |> function
        | [] -> None
        | first :: rest ->
            first
            |> tryGetFiles
            |> Option.map (fun validFiles ->
                   { DocFiles = validFiles
                     OutputDirectory =
                         tryGetOutputDirectory rest |> Option.defaultWith (fun () -> getOutputDirectory first)
                     RequestedNotFoundDocFiles = [] })
