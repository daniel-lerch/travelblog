using TravelBlog.Services;
using Xunit;

namespace TravelBlog.Tests
{
    public class MarkdownServiceTests
    {
        [Fact]
        public void TestRendersHtml()
        {
            MarkdownService markdownService = new();
            string markdown = "<img src=\"/media/files/asdf.jpg\">\n";
            string html = markdownService.ToHtml(markdown);
            Assert.Equal(markdown, html);
        }

        [Fact]
        public void TestRendersImgClass()
        {
            MarkdownService markdownService = new();
            string markdown = "![this is an image](/media/files/asdf.jpg)\n";
            string expected = "<p><img src=\"/media/files/asdf.jpg\" class=\"markdown-img\" alt=\"this is an image\" /></p>\n";
            Assert.Equal(expected, markdownService.ToHtml(markdown));
        }
    }
}
