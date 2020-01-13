namespace Jds.DotnetDocConverter

open System.Text

[<AutoOpen>]
module TypeExtensions =
    type System.Text.StringBuilder with
        static member appendLine text (stringBuilder : StringBuilder) = stringBuilder.AppendLine(text)
        static member append (text : string) (stringBuilder : StringBuilder) = stringBuilder.Append(text)
        static member toString (stringBuilder : StringBuilder) = stringBuilder.ToString()
        static member appendLines (lines : string seq) (stringBuilder : StringBuilder) =
            lines |> Seq.fold (fun sb line -> StringBuilder.appendLine line sb) stringBuilder
