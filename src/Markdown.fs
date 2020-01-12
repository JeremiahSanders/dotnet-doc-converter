namespace Jds.DotnetDocConverter

module Markdown =
    type InlineNode (value:string) =
        member this.Value = value
        static member ofString value =
            InlineNode value
    type BlockNode (nodes:InlineNode list) =
        member this.Nodes = nodes
        static member ofNode (node:InlineNode) =
            BlockNode [node]
        static member ofNodes (nodes:InlineNode list) =
            BlockNode nodes
        static member ofString = InlineNode.ofString>>BlockNode.ofNode
    type DocumentNode (blocks:BlockNode list) =
        member this.Blocks = blocks
        static member ofBlock = List.singleton>>DocumentNode
        static member ofBlocks blocks = DocumentNode blocks

    [<AutoOpen>]
    module Elements =

        [<AutoOpen>]
        module Headings =
            let h1 = sprintf "# %s"
            let h2 = sprintf "## %s"
            let h3 = sprintf "### %s"
            let h4 = sprintf "#### %s"
            let h5 = sprintf "##### %s"
            let h6 = sprintf "###### %s"

        [<AutoOpen>]
        module Inline =
            // Use double-backtick if contents will contain a backtick, so it will display correctly.
            let private getCodeDelimiter (value:string) = if (value.Contains("`")) then "``" else "`"
            let code (value:string) =
                let delimiter = getCodeDelimiter value
                sprintf "%s%s%s" delimiter value delimiter

        [<AutoOpen>]
        module Blocks =
            let codeBlockTyped language = sprintf "```%s\n%s\n```" language
            let codeBlock = codeBlockTyped ""
            let blockquote = sprintf "> %s"
