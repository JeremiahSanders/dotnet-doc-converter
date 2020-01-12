namespace Jds.DotnetDocConverter

module FileSaving =

  let saveFile path content =
      System.IO.File.WriteAllText(path, content)
      content
