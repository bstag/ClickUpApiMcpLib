using System;
using System.Text.Json;
using ClickUp.Api.Client.Models.Serialization.Converters;
using Xunit; // Changed from MSTest

namespace ClickUp.Api.Client.Tests.Models.Serialization.Converters
{
    public class JsonStringOrNumberConverterTests // Removed [TestClass]
    {
        private readonly JsonSerializerOptions _options;

        // Changed from [TestInitialize] to constructor for xUnit
        public JsonStringOrNumberConverterTests()
        {
            _options = new JsonSerializerOptions();
            _options.Converters.Add(new JsonStringOrNumberConverter());
        }

        [Fact] // Changed from [TestMethod]
        public void Read_WhenJsonIsString_ShouldReturnString()
        {
            var json = "\"hello\"";
            var result = JsonSerializer.Deserialize<string>(json, _options);
            Assert.Equal("hello", result); // Changed from Assert.AreEqual
        }

        [Fact] // Changed from [TestMethod]
        public void Read_WhenJsonIsInteger_ShouldReturnString()
        {
            var json = "123";
            var result = JsonSerializer.Deserialize<string>(json, _options);
            Assert.Equal("123", result); // Changed from Assert.AreEqual
        }

        [Fact] // Changed from [TestMethod]
        public void Read_WhenJsonIsDecimal_ShouldReturnString()
        {
            var json = "123.45";
            var result = JsonSerializer.Deserialize<string>(json, _options);
            Assert.Equal("123.45", result); // Changed from Assert.AreEqual
        }

        [Fact] // Changed from [TestMethod]
        public void Read_WhenJsonIsNegativeNumber_ShouldReturnString()
        {
            var json = "-42";
            var result = JsonSerializer.Deserialize<string>(json, _options);
            Assert.Equal("-42", result); // Changed from Assert.AreEqual
        }

        [Fact] // Changed from [TestMethod]
        public void Read_WhenJsonIsZero_ShouldReturnString()
        {
            var json = "0";
            var result = JsonSerializer.Deserialize<string>(json, _options);
            Assert.Equal("0", result); // Changed from Assert.AreEqual
        }

        [Fact] // Changed from [TestMethod]
        public void Read_WhenJsonIsEmptyString_ShouldReturnEmptyString()
        {
            var json = "\"\"";
            var result = JsonSerializer.Deserialize<string>(json, _options);
            Assert.Equal("", result); // Changed from Assert.AreEqual
        }

        [Fact] // Changed from [TestMethod]
        public void Read_WhenJsonIsNull_ShouldReturnNull()
        {
            var json = "null";
            var result = JsonSerializer.Deserialize<string>(json, _options);
            Assert.Null(result); // Changed from Assert.IsNull
        }

        [Fact] // Changed from [TestMethod]
        public void Write_WhenValueIsString_ShouldWriteJsonString()
        {
            var value = "world";
            var result = JsonSerializer.Serialize(value, _options);
            Assert.Equal("\"world\"", result); // Changed from Assert.AreEqual
        }

        [Fact] // Changed from [TestMethod]
        public void Write_WhenValueIsNull_ShouldWriteJsonNull()
        {
            string? value = null; // Explicitly nullable string
            var result = JsonSerializer.Serialize(value, _options);
            Assert.Equal("null", result); // Changed from Assert.AreEqual
        }

        [Fact] // Changed from [TestMethod]
        public void Read_WhenJsonIsScientificNotation_ShouldReturnString()
        {
            var json = "1.23e4"; // Represents 12300
            var result = JsonSerializer.Deserialize<string>(json, _options);
            // JsonDocument.RootElement.GetRawText() for a number token normalizes its representation.
            // e.g., 1.23e4 becomes "12300". This is the expected behavior of the current converter.
            Assert.Equal("12300", result);

            json = "1.23E+4"; // Represents 12300
            result = JsonSerializer.Deserialize<string>(json, _options);
            Assert.Equal("12300", result);

            json = "1E-3"; // Represents 0.001
            result = JsonSerializer.Deserialize<string>(json, _options);
            Assert.Equal("0.001", result); // Normalized from 1E-3
        }
    }
}
