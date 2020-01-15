namespace Jds.DotnetDocConverter

open System.IO

module FileSaving =
    let private ensureDirectoryExists directory =
        if Directory.Exists(directory) then directory
        else
            let _ = Directory.CreateDirectory(directory)
            directory

    let private getDirectory (path : string) = Path.GetDirectoryName(path)

    let saveFile path content =
        let _ = getDirectory path |> ensureDirectoryExists
        File.WriteAllText(path, content)
        content
