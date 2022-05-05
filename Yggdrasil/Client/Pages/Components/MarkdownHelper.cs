using Markdig;

namespace Yggdrasil.Client.Pages.Components
{
    /// <summary>
    /// Support for converting Markdown to HTML
    /// </summary>
    static class MarkdownHelper
    {
        static MarkdownPipeline Pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

        /// <summary>
        /// Converts a markdown-based string to an HTML-based string
        /// </summary>
        /// <param name="markdown">Markdown-based string to convert</param>
        /// <returns>Converted text, in HTML</returns>
        public static string ToHtml(string markdown)
        {
            if (markdown == null)
                return string.Empty;
            return Markdown.ToHtml(markdown, Pipeline);
        }
    }
}
