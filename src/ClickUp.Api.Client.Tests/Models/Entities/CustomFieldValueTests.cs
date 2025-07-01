using System.Text.Json;
using ClickUp.Api.Client.Helpers; // For JsonSerializerOptionsHelper
using Xunit;
using ClickUp.Api.Client.Models.Entities.CustomFields;

namespace ClickUp.Api.Client.Tests.Models.Entities
{
    public class CustomFieldValueTests
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = JsonSerializerOptionsHelper.Options;

        [Fact]
        public void Deserialize_TextFieldValue_Correctly()
        {
            var json = @"{
                ""id"": ""03efda77-c7a0-42d3-8afd-fd546353c2f5"",
                ""name"": ""Text Field"",
                ""type"": ""text"",
                ""type_config"": {},
                ""date_created"": ""1566400407303"",
                ""hide_from_guests"": false,
                ""value"": ""This is some text""
            }";

            var customField = JsonSerializer.Deserialize<CustomFieldValue>(json, _jsonSerializerOptions);

            Assert.NotNull(customField);
            Assert.Equal("text", customField.Type);
            Assert.IsType<JsonElement>(customField.Value);
            var valueElement = (JsonElement)customField.Value;
            Assert.Equal(JsonValueKind.String, valueElement.ValueKind);
            Assert.Equal("This is some text", valueElement.GetString());
        }

        [Fact]
        public void Deserialize_NumberFieldValue_Correctly()
        {
            var json = @"{
                ""id"": ""123abcde-c7a0-42d3-8afd-fd546353c2f5"",
                ""name"": ""Number Field"",
                ""type"": ""number"",
                ""type_config"": {},
                ""date_created"": ""1566400407304"",
                ""hide_from_guests"": false,
                ""value"": 123.45
            }";

            var customField = JsonSerializer.Deserialize<CustomFieldValue>(json, _jsonSerializerOptions);

            Assert.NotNull(customField);
            Assert.Equal("number", customField.Type);
            Assert.IsType<JsonElement>(customField.Value);
            var valueElement = (JsonElement)customField.Value;
            Assert.Equal(JsonValueKind.Number, valueElement.ValueKind);
            Assert.Equal(123.45, valueElement.GetDouble());
        }

        [Fact]
        public void Deserialize_DropdownFieldValue_Correctly()
        {
            // Dropdown 'value' stores the ID of the selected option, which is often a GUID (string) or an int.
            // Assuming it's a string ID for this test.
            var json = @"{
                ""id"": ""abcdef12-c7a0-42d3-8afd-fd546353c2f5"",
                ""name"": ""Dropdown Field"",
                ""type"": ""drop_down"",
                ""type_config"": { ""options"": [{""id"": ""opt_guid_1"", ""name"": ""Option 1""}] },
                ""date_created"": ""1566400407305"",
                ""hide_from_guests"": false,
                ""value"": ""opt_guid_1""
            }";

            var customField = JsonSerializer.Deserialize<CustomFieldValue>(json, _jsonSerializerOptions);

            Assert.NotNull(customField);
            Assert.Equal("drop_down", customField.Type);
            Assert.IsType<JsonElement>(customField.Value);
            var valueElement = (JsonElement)customField.Value;
            Assert.Equal(JsonValueKind.String, valueElement.ValueKind);
            Assert.Equal("opt_guid_1", valueElement.GetString());
        }

        [Fact]
        public void Deserialize_DropdownFieldValue_WithNumericValue_Correctly()
        {
            // For some dropdowns, the 'value' might be the numeric orderindex of the selected option.
            var json = @"{
                ""id"": ""abcdef12-c7a0-42d3-8afd-fd546353c2f5"",
                ""name"": ""Dropdown Field"",
                ""type"": ""drop_down"",
                ""type_config"": { ""options"": [{""id"": ""opt_guid_1"", ""name"": ""Option 1"", ""orderindex"": 0}] },
                ""date_created"": ""1566400407305"",
                ""hide_from_guests"": false,
                ""value"": 0
            }";

            var customField = JsonSerializer.Deserialize<CustomFieldValue>(json, _jsonSerializerOptions);

            Assert.NotNull(customField);
            Assert.Equal("drop_down", customField.Type);
            Assert.IsType<JsonElement>(customField.Value);
            var valueElement = (JsonElement)customField.Value;
            Assert.Equal(JsonValueKind.Number, valueElement.ValueKind);
            Assert.Equal(0, valueElement.GetInt32());
        }

        [Fact]
        public void Deserialize_LabelsFieldValue_Correctly()
        {
            var json = @"{
                ""id"": ""labels123-c7a0-42d3-8afd-fd546353c2f5"",
                ""name"": ""Labels Field"",
                ""type"": ""labels"",
                ""type_config"": { ""options"": [{""id"": ""label_A"", ""label"": ""Label A""}] },
                ""date_created"": ""1566400407306"",
                ""hide_from_guests"": false,
                ""value"": [""label_A""]
            }";

            var customField = JsonSerializer.Deserialize<CustomFieldValue>(json, _jsonSerializerOptions);

            Assert.NotNull(customField);
            Assert.Equal("labels", customField.Type);
            Assert.IsType<JsonElement>(customField.Value);
            var valueElement = (JsonElement)customField.Value;
            Assert.Equal(JsonValueKind.Array, valueElement.ValueKind);
            Assert.Equal(1, valueElement.GetArrayLength());
            Assert.Equal("label_A", valueElement[0].GetString());
        }

        [Fact]
        public void Deserialize_UsersFieldValue_Correctly()
        {
            // Value for 'users' type is an array of user IDs (integers)
            var json = @"{
                ""id"": ""users123-c7a0-42d3-8afd-fd546353c2f5"",
                ""name"": ""Users Field"",
                ""type"": ""users"",
                ""type_config"": {},
                ""date_created"": ""1566400407307"",
                ""hide_from_guests"": false,
                ""value"": [183, 184]
            }";

            var customField = JsonSerializer.Deserialize<CustomFieldValue>(json, _jsonSerializerOptions);

            Assert.NotNull(customField);
            Assert.Equal("users", customField.Type);
            Assert.IsType<JsonElement>(customField.Value);
            var valueElement = (JsonElement)customField.Value;
            Assert.Equal(JsonValueKind.Array, valueElement.ValueKind);
            Assert.Equal(2, valueElement.GetArrayLength());
            Assert.Equal(183, valueElement[0].GetInt32());
            Assert.Equal(184, valueElement[1].GetInt32());
        }

        [Fact]
        public void Deserialize_DateFieldValue_Correctly()
        {
            // Date value is typically a Unix timestamp string
            var json = @"{
                ""id"": ""date123-c7a0-42d3-8afd-fd546353c2f5"",
                ""name"": ""Date Field"",
                ""type"": ""date"",
                ""type_config"": {},
                ""date_created"": ""1566400407308"",
                ""hide_from_guests"": false,
                ""value"": ""1667367645000""
            }";

            var customField = JsonSerializer.Deserialize<CustomFieldValue>(json, _jsonSerializerOptions);

            Assert.NotNull(customField);
            Assert.Equal("date", customField.Type);
            Assert.IsType<JsonElement>(customField.Value);
            var valueElement = (JsonElement)customField.Value;
            Assert.Equal(JsonValueKind.String, valueElement.ValueKind);
            Assert.Equal("1667367645000", valueElement.GetString());
            // Further conversion to DateTimeOffset would be handled by the SDK user or a helper
        }

        [Fact]
        public void Deserialize_CheckboxFieldValue_Correctly()
        {
            // Checkbox value is typically "0" or "1" as a string, or boolean true/false
            var json = @"{
                ""id"": ""checkbox123-c7a0-42d3-8afd-fd546353c2f5"",
                ""name"": ""Checkbox Field"",
                ""type"": ""checkbox"",
                ""type_config"": {},
                ""date_created"": ""1566400407309"",
                ""hide_from_guests"": false,
                ""value"": ""1""
            }";

            var customField = JsonSerializer.Deserialize<CustomFieldValue>(json, _jsonSerializerOptions);

            Assert.NotNull(customField);
            Assert.Equal("checkbox", customField.Type);
            Assert.IsType<JsonElement>(customField.Value);
            var valueElement = (JsonElement)customField.Value;
            Assert.Equal(JsonValueKind.String, valueElement.ValueKind);
            Assert.Equal("1", valueElement.GetString());

            // Test with boolean true
             json = @"{
                ""id"": ""checkbox123-c7a0-42d3-8afd-fd546353c2f5"",
                ""name"": ""Checkbox Field"",
                ""type"": ""checkbox"",
                ""type_config"": {},
                ""date_created"": ""1566400407309"",
                ""hide_from_guests"": false,
                ""value"": true
            }";
            customField = JsonSerializer.Deserialize<CustomFieldValue>(json, _jsonSerializerOptions);
            Assert.NotNull(customField);
            valueElement = (JsonElement)customField.Value;
            Assert.Equal(JsonValueKind.True, valueElement.ValueKind);
            Assert.True(valueElement.GetBoolean());
        }

        [Fact]
        public void Deserialize_LocationFieldValue_Correctly()
        {
            var json = @"{
                ""id"": ""location123-abc"",
                ""name"": ""Location Field"",
                ""type"": ""location"",
                ""type_config"": {},
                ""date_created"": ""1566400407310"",
                ""hide_from_guests"": false,
                ""value"": {
                    ""location"": { ""lat"": -28.016667, ""lng"": 153.4 },
                    ""formatted_address"": ""Gold Coast QLD, Australia""
                }
            }";

            var customField = JsonSerializer.Deserialize<CustomFieldValue>(json, _jsonSerializerOptions);
            Assert.NotNull(customField);
            Assert.Equal("location", customField.Type);
            Assert.IsType<JsonElement>(customField.Value);
            var valueElement = (JsonElement)customField.Value;
            Assert.Equal(JsonValueKind.Object, valueElement.ValueKind);
            Assert.True(valueElement.TryGetProperty("location", out var locObj));
            Assert.True(locObj.TryGetProperty("lat", out var latVal));
            Assert.Equal(-28.016667, latVal.GetDouble());
            Assert.True(valueElement.TryGetProperty("formatted_address", out var addrVal));
            Assert.Equal("Gold Coast QLD, Australia", addrVal.GetString());
        }
    }
}
