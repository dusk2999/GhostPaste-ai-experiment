using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using MdBlock = Markdig.Syntax.Block;
using MdInline = Markdig.Syntax.Inlines.Inline;
using WpfList = System.Windows.Documents.List;

namespace GhostPaste.AI;

public sealed class MarkdownFlowDocumentRenderer
{
    private static readonly FontFamily BodyFont = new("Segoe UI");
    private static readonly FontFamily CodeFont = new("Consolas");
    private static readonly Brush TextBrush = BrushFromRgb(0x18, 0x18, 0x1B);
    private static readonly Brush MutedBrush = BrushFromArgb(0xAA, 0x18, 0x18, 0x1B);
    private static readonly Brush LinkBrush = BrushFromRgb(0x25, 0x63, 0xEB);
    private static readonly Brush CodeBackgroundBrush = BrushFromArgb(0x16, 0x00, 0x00, 0x00);
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder().Build();

    public FlowDocument Render(string? markdown)
    {
        var document = new FlowDocument
        {
            Background = Brushes.Transparent,
            Foreground = TextBrush,
            FontFamily = BodyFont,
            FontSize = 13,
            PagePadding = new Thickness(0),
        };

        var parsed = Markdown.Parse(markdown ?? string.Empty, Pipeline);
        if (!parsed.Any())
        {
            document.Blocks.Add(CreateParagraph(string.Empty));
            return document;
        }

        foreach (MdBlock block in parsed)
        {
            AppendBlock(document.Blocks, block);
        }

        return document;
    }

    private static void AppendBlock(BlockCollection destination, MdBlock block)
    {
        switch (block)
        {
            case HeadingBlock heading:
                destination.Add(RenderHeading(heading));
                break;
            case ParagraphBlock paragraph:
                destination.Add(RenderParagraph(paragraph));
                break;
            case ListBlock list:
                destination.Add(RenderList(list));
                break;
            case FencedCodeBlock fencedCode:
                destination.Add(RenderCodeBlock(fencedCode));
                break;
            case CodeBlock code:
                destination.Add(RenderCodeBlock(code));
                break;
            case QuoteBlock quote:
                AppendQuote(destination, quote);
                break;
            case ThematicBreakBlock:
                destination.Add(CreateParagraph("---", MutedBrush));
                break;
            case ContainerBlock container:
                AppendContainerBlocks(destination, container);
                break;
            case LeafBlock leaf:
                destination.Add(CreateParagraph(leaf.Lines.ToString()));
                break;
        }
    }

    private static Paragraph RenderHeading(HeadingBlock heading)
    {
        var paragraph = CreateParagraph();
        paragraph.FontWeight = FontWeights.SemiBold;
        paragraph.FontSize = heading.Level switch
        {
            1 => 20,
            2 => 17,
            3 => 15,
            _ => 13.5,
        };
        paragraph.Margin = new Thickness(0, 0, 0, 8);
        AppendInlineContainer(paragraph.Inlines, heading.Inline);
        return paragraph;
    }

    private static Paragraph RenderParagraph(ParagraphBlock paragraphBlock)
    {
        var paragraph = CreateParagraph();
        AppendInlineContainer(paragraph.Inlines, paragraphBlock.Inline);
        return paragraph;
    }

    private static WpfList RenderList(ListBlock listBlock)
    {
        var list = new WpfList
        {
            MarkerStyle = listBlock.IsOrdered ? TextMarkerStyle.Decimal : TextMarkerStyle.Disc,
            Margin = new Thickness(18, 0, 0, 8),
            Padding = new Thickness(0),
        };

        foreach (MdBlock child in listBlock)
        {
            if (child is not ListItemBlock itemBlock)
            {
                continue;
            }

            var item = new ListItem
            {
                Margin = new Thickness(0, 0, 0, 2),
                Padding = new Thickness(0),
            };

            AppendContainerBlocks(item.Blocks, itemBlock);
            list.ListItems.Add(item);
        }

        return list;
    }

    private static Paragraph RenderCodeBlock(CodeBlock codeBlock)
    {
        return new Paragraph(new Run(codeBlock.Lines.ToString()))
        {
            Background = CodeBackgroundBrush,
            FontFamily = CodeFont,
            FontSize = 12.5,
            Margin = new Thickness(0, 0, 0, 8),
            Padding = new Thickness(8),
        };
    }

    private static void AppendQuote(BlockCollection destination, QuoteBlock quote)
    {
        var section = new Section
        {
            BorderBrush = BrushFromArgb(0x30, 0x00, 0x00, 0x00),
            BorderThickness = new Thickness(3, 0, 0, 0),
            Foreground = MutedBrush,
            Margin = new Thickness(0, 0, 0, 8),
            Padding = new Thickness(8, 0, 0, 0),
        };

        AppendContainerBlocks(section.Blocks, quote);
        destination.Add(section);
    }

    private static void AppendContainerBlocks(BlockCollection destination, ContainerBlock container)
    {
        foreach (MdBlock child in container)
        {
            AppendBlock(destination, child);
        }
    }

    private static Paragraph CreateParagraph(string? text = null, Brush? foreground = null)
    {
        var paragraph = new Paragraph
        {
            Foreground = foreground ?? TextBrush,
            Margin = new Thickness(0, 0, 0, 8),
        };

        if (text is not null)
        {
            paragraph.Inlines.Add(new Run(text));
        }

        return paragraph;
    }

    private static void AppendInlineContainer(InlineCollection destination, ContainerInline? container)
    {
        if (container is null)
        {
            return;
        }

        for (MdInline? child = container.FirstChild; child is not null; child = child.NextSibling)
        {
            AppendInline(destination, child);
        }
    }

    private static void AppendInline(InlineCollection destination, MdInline inline)
    {
        switch (inline)
        {
            case LiteralInline literal:
                destination.Add(new Run(literal.Content.ToString()));
                break;
            case LineBreakInline:
                destination.Add(new LineBreak());
                break;
            case EmphasisInline emphasis:
                destination.Add(RenderEmphasis(emphasis));
                break;
            case CodeInline code:
                destination.Add(new Run(code.Content)
                {
                    Background = CodeBackgroundBrush,
                    FontFamily = CodeFont,
                });
                break;
            case LinkInline link:
                destination.Add(RenderLink(link));
                break;
            case ContainerInline container:
                var span = new Span();
                AppendInlineContainer(span.Inlines, container);
                destination.Add(span);
                break;
        }
    }

    private static Span RenderEmphasis(EmphasisInline emphasis)
    {
        Span span = emphasis.DelimiterCount >= 2 ? new Bold() : new Italic();
        AppendInlineContainer(span.Inlines, emphasis);
        return span;
    }

    private static Hyperlink RenderLink(LinkInline link)
    {
        var hyperlink = new Hyperlink
        {
            Foreground = LinkBrush,
        };

        if (!string.IsNullOrWhiteSpace(link.Url)
            && Uri.TryCreate(link.Url, UriKind.RelativeOrAbsolute, out Uri? uri))
        {
            hyperlink.NavigateUri = uri;
        }

        AppendInlineContainer(hyperlink.Inlines, link);
        if (!hyperlink.Inlines.Any())
        {
            hyperlink.Inlines.Add(new Run(link.Url ?? string.Empty));
        }

        return hyperlink;
    }

    private static SolidColorBrush BrushFromRgb(byte r, byte g, byte b)
    {
        return BrushFromArgb(0xFF, r, g, b);
    }

    private static SolidColorBrush BrushFromArgb(byte a, byte r, byte g, byte b)
    {
        return new SolidColorBrush(Color.FromArgb(a, r, g, b));
    }
}
