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
                     OutputDirectory = "."
                     RequestedNotFoundDocFiles = [] })
