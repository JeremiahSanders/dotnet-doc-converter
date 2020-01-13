namespace Jds.DotnetDocConverter

open System.IO
open System.Xml.Linq

module FileLoading =
    let tryGetFiles directory =
        if (Directory.Exists(directory)) then (Some(Directory.GetFiles(directory)))
        else None

    let loadXmlFile file = System.IO.File.ReadAllText(file) |> XDocument.Parse
    let loadXmlFiles (files : string array) = files |> Array.map loadXmlFile
