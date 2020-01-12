namespace Jds.DotnetDocConverter

module ArgParsing =
    type ProgramArguments =
        { DocFiles : string list
          OutputDirectory : string
          RequestedNotFoundDocFiles : string list }

    let parseArguments arguments =
        arguments
        |> Array.toList
        |> List.partition System.IO.File.Exists
        |> (fun (validFiles, invalidFiles) ->
        { DocFiles = validFiles
          OutputDirectory = "."
          RequestedNotFoundDocFiles = invalidFiles })
