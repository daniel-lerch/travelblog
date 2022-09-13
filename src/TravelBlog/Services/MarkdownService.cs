using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace TravelBlog.Services;

public class MarkdownService
{
    private readonly MarkdownPipeline pipeline;

    public MarkdownService()
    {
        MarkdownPipelineBuilder builder = new();
        builder.Use<ImgClassExtension>();
        pipeline = builder.Build();
    }

    public string ToHtml(string markdown)
    {
        return Markdown.ToHtml(markdown, pipeline);
    }

    private class ImgClassExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            // Make sure we don't have a delegate twice
            pipeline.DocumentProcessed -= PipelineOnDocumentProcessed;
            pipeline.DocumentProcessed += PipelineOnDocumentProcessed;
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
        }

        private static void PipelineOnDocumentProcessed(MarkdownDocument document)
        {
            foreach (var node in document.Descendants())
            {
                if (node is LinkInline link && link.IsImage)
                {
                    link.GetAttributes().AddClass("markdown-img");
                }
            }
        }
    }
}
