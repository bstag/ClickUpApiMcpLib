using ClickUp.Api.Client.Helpers;
using Xunit;

namespace ClickUp.Api.Client.Tests.Helpers
{
    public class FluentUrlBuilderTests
    {
        [Fact]
        public void Build_WithQueryParameters_ShouldNotHaveDoubleQuestionMark()
        {
            var url = FluentUrlBuilder.Create("https://api.clickup.com")
                .WithQueryParameter("key", "value")
                .Build();

            Assert.DoesNotContain("??", url);
            Assert.Equal("https://api.clickup.com?key=value", url);
        }

        [Fact]
        public void Build_WithMultipleQueryParameters_ShouldFormatCorrectly()
        {
            var url = FluentUrlBuilder.Create("https://api.clickup.com")
                .WithQueryParameter("key1", "value1")
                .WithQueryParameter("key2", "value2")
                .Build();

            Assert.Equal("https://api.clickup.com?key1=value1&key2=value2", url);
        }
    }
}
