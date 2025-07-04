using ClickUp.Api.Client.Helpers;
using Xunit; // Added for FactAttribute

namespace ClickUp.Api.Client.Tests.Helpers
{
    public class UrlBuilderHelperTests
    {
        [Fact]
        public void BuildQueryString_List_EmptyList_ReturnsEmptyString()
        {
            var queryParams = new List<KeyValuePair<string, string>>();
            var result = UrlBuilderHelper.BuildQueryString(queryParams);
            Assert.Empty(result);
        }

        [Fact]
        public void BuildQueryString_List_SingleParameter_ReturnsCorrectString()
        {
            var queryParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("key1", "value1")
            };
            var result = UrlBuilderHelper.BuildQueryString(queryParams);
            Assert.Equal("?key1=value1", result);
        }

        [Fact]
        public void BuildQueryString_List_MultipleParameters_ReturnsCorrectString()
        {
            var queryParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("key1", "value1"),
                new KeyValuePair<string, string>("key2", "value2")
            };
            var result = UrlBuilderHelper.BuildQueryString(queryParams);
            Assert.Equal("?key1=value1&key2=value2", result);
        }

        [Fact]
        public void BuildQueryString_List_KeyNeedsEncoding_ReturnsEncodedKey()
        {
            var queryParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("key 1", "value1")
            };
            var result = UrlBuilderHelper.BuildQueryString(queryParams);
            Assert.Equal("?key%201=value1", result);
        }

        [Fact]
        public void BuildQueryString_List_ValueIsPreEncoded_ReturnsValueAsIs()
        {
            var queryParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("key1", "value%201") // Pre-encoded space
            };
            var result = UrlBuilderHelper.BuildQueryString(queryParams);
            Assert.Equal("?key1=value%201", result);
        }

        [Fact]
        public void BuildQueryString_Dictionary_EmptyDictionary_ReturnsEmptyString()
        {
            var queryParams = new Dictionary<string, string?>();
            var result = UrlBuilderHelper.BuildQueryString(queryParams);
            Assert.Empty(result);
        }

        [Fact]
        public void BuildQueryString_Dictionary_SingleParameter_ReturnsCorrectString()
        {
            var queryParams = new Dictionary<string, string?>
            {
                { "key1", "value1" }
            };
            var result = UrlBuilderHelper.BuildQueryString(queryParams);
            Assert.Equal("?key1=value1", result);
        }

        [Fact]
        public void BuildQueryString_Dictionary_MultipleParameters_ReturnsCorrectString()
        {
            var queryParams = new Dictionary<string, string?>
            {
                { "key1", "value1" },
                { "key2", "value2" }
            };
            var result = UrlBuilderHelper.BuildQueryString(queryParams);
            Assert.Equal("?key1=value1&key2=value2", result);
        }

        [Fact]
        public void BuildQueryString_Dictionary_KeyAndValueNeedEncoding_ReturnsEncodedKeyAndValue()
        {
            var queryParams = new Dictionary<string, string?>
            {
                { "key 1", "value 1" }
            };
            var result = UrlBuilderHelper.BuildQueryString(queryParams);
            Assert.Equal("?key%201=value%201", result);
        }

        [Fact]
        public void BuildQueryString_Dictionary_NullValueParameter_IsSkipped()
        {
            var queryParams = new Dictionary<string, string?>
            {
                { "key1", "value1" },
                { "key2", null },
                { "key3", "value3" }
            };
            var result = UrlBuilderHelper.BuildQueryString(queryParams);
            Assert.Equal("?key1=value1&key3=value3", result);
        }

        [Fact]
        public void BuildQueryString_Dictionary_AllValuesNull_ReturnsEmptyString()
        {
            var queryParams = new Dictionary<string, string?>
            {
                { "key1", null },
                { "key2", null }
            };
            var result = UrlBuilderHelper.BuildQueryString(queryParams);
            Assert.Empty(result);
        }

        [Fact]
        public void BuildQueryStringFromArray_NullEnumerable_ReturnsEmptyString()
        {
            var result = UrlBuilderHelper.BuildQueryStringFromArray<string>("key", null);
            Assert.Empty(result);
        }

        [Fact]
        public void BuildQueryStringFromArray_EmptyEnumerable_ReturnsEmptyString()
        {
            var result = UrlBuilderHelper.BuildQueryStringFromArray<string>("key", new List<string>());
            Assert.Empty(result);
        }

        [Fact]
        public void BuildQueryStringFromArray_SingleValue_ReturnsCorrectString()
        {
            var values = new List<string> { "value1" };
            var result = UrlBuilderHelper.BuildQueryStringFromArray("key", values);
            Assert.Equal("key[]=value1", result); // Note: This helper doesn't add the initial '?'
        }

        [Fact]
        public void BuildQueryStringFromArray_MultipleValues_ReturnsCorrectString()
        {
            var values = new List<string> { "value1", "value2" };
            var result = UrlBuilderHelper.BuildQueryStringFromArray("key", values);
            Assert.Equal("key[]=value1&key[]=value2", result);
        }

        [Fact]
        public void BuildQueryStringFromArray_ValuesNeedEncoding_ReturnsEncodedValues()
        {
            var values = new List<string> { "value 1", "value@2" };
            var result = UrlBuilderHelper.BuildQueryStringFromArray("key", values);
            Assert.Equal("key[]=value%201&key[]=value%402", result);
        }

        [Fact]
        public void BuildQueryStringFromArray_KeyNeedsEncoding_ReturnsEncodedKey()
        {
            var values = new List<string> { "value1" };
            var result = UrlBuilderHelper.BuildQueryStringFromArray("key name", values);
            Assert.Equal("key%20name[]=value1", result);
        }

        [Fact]
        public void BuildQueryStringFromArray_IntegerValues_ReturnsCorrectString()
        {
            var values = new List<int> { 1, 2, 3 };
            var result = UrlBuilderHelper.BuildQueryStringFromArray("numbers", values);
            Assert.Equal("numbers[]=1&numbers[]=2&numbers[]=3", result);
        }
    }
}
