using ClickUp.Api.Client.Helpers;
using Xunit;
using System.Collections.Generic;

namespace ClickUp.Api.Client.Tests.Helpers
{
    public class FluentUrlBuilderReproTest
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
    }
}
