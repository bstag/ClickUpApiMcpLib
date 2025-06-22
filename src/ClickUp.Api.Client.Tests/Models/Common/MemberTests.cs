using System.Text.Json;
using ClickUp.Api.Client.Models.Common;
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Helpers; // For JsonSerializerOptionsHelper
using Xunit;
using FluentAssertions;

namespace ClickUp.Api.Client.Tests.Models.Common
{
    public class MemberTests
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = JsonSerializerOptionsHelper.Options;

        [Fact]
        public void Member_ShouldSerializeCorrectly_WhenAllPropertiesAreSet()
        {
            // Arrange
            var user = new User(
                Id: 123,
                Username: "testuser",
                Email: "test@example.com",
                Color: "#FF0000",
                ProfilePicture: "https://example.com/profile.jpg",
                Initials: "TU",
                Role: 1, // Assuming 1 is a valid role ID
                CustomRole: null, // Keep complex types null for now
                LastActive: null,
                DateJoined: null,
                DateInvited: null,
                ProfileInfo: null
            );

            var member = new Member(
                User: user,
                Role: "Admin",
                PermissionLevel: "ReadWrite"
            );

            var expectedJson = """
            {
              "user": {
                "id": 123,
                "username": "testuser",
                "email": "test@example.com",
                "color": "#FF0000",
                "profilePicture": "https://example.com/profile.jpg",
                "initials": "TU",
                "role": 1
              },
              "role": "Admin",
              "permission_level": "ReadWrite"
            }
            """;
            // Null properties like custom_role, last_active etc. in User are omitted due to JsonIgnoreCondition.WhenWritingNull

            // Act
            var actualJson = JsonSerializer.Serialize(member, _jsonSerializerOptions);

            // Assert
            // Re-parse and re-serialize to get a canonical string representation for comparison
            var canonicalExpectedJson = JsonSerializer.Serialize(JsonDocument.Parse(expectedJson).RootElement, _jsonSerializerOptions);
            var canonicalActualJson = JsonSerializer.Serialize(JsonDocument.Parse(actualJson).RootElement, _jsonSerializerOptions);

            canonicalActualJson.Should().Be(canonicalExpectedJson);
        }

        [Fact]
        public void Member_ShouldDeserializeCorrectly_WhenAllPropertiesAreSet()
        {
            // Arrange
            var json = """
            {
              "user": {
                "id": 456,
                "username": "anotheruser",
                "email": "another@example.com",
                "color": "#00FF00",
                "profilePicture": "https://example.com/another.jpg",
                "initials": "AU",
                "role": 2,
                "custom_role": null,
                "last_active": null,
                "date_joined": null,
                "date_invited": null,
                "profileInfo": null
              },
              "role": "Member",
              "permission_level": "ReadOnly"
            }
            """;

            var expectedUser = new User(
                Id: 456,
                Username: "anotheruser",
                Email: "another@example.com",
                Color: "#00FF00",
                ProfilePicture: "https://example.com/another.jpg",
                Initials: "AU",
                Role: 2,
                CustomRole: null,
                LastActive: null,
                DateJoined: null,
                DateInvited: null,
                ProfileInfo: null
            );

            var expectedMember = new Member(
                User: expectedUser,
                Role: "Member",
                PermissionLevel: "ReadOnly"
            );

            // Act
            var actualMember = JsonSerializer.Deserialize<Member>(json, _jsonSerializerOptions);

            // Assert
            actualMember.Should().NotBeNull();
            actualMember.Should().BeEquivalentTo(expectedMember);
        }

        [Fact]
        public void Member_ShouldSerializeCorrectly_WhenOptionalPropertiesAreNull()
        {
            // Arrange
            var user = new User(
                Id: 789,
                Username: null, // Nullable string
                Email: "minimal@example.com",
                Color: null,
                ProfilePicture: null,
                Initials: null,
                Role: null, // Nullable int
                CustomRole: null,
                LastActive: null,
                DateJoined: null,
                DateInvited: null,
                ProfileInfo: null
            );

            var member = new Member(
                User: user,
                Role: null, // Nullable string
                PermissionLevel: null // Nullable string
            );

            // Note: System.Text.Json by default omits null values during serialization.
            // If we want to explicitly include them, options need to be set, but default is usually fine.
            var expectedJson = """
            {
              "user": {
                "id": 789,
                "email": "minimal@example.com"
              }
            }
            """;
            // "username": null, "color": null, etc. would be included if JsonSerializerOptions.DefaultIgnoreCondition
            // was set to JsonIgnoreCondition.Never. Default is JsonIgnoreCondition.WhenWritingDefault/Null.
            // For this test, we assume default behavior of omitting nulls.

            // Act
            var actualJson = JsonSerializer.Serialize(member, _jsonSerializerOptions);

            // Assert
            // Re-parse and re-serialize to get a canonical string representation for comparison
            var canonicalExpectedJson = JsonSerializer.Serialize(JsonDocument.Parse(expectedJson).RootElement, _jsonSerializerOptions);
            var canonicalActualJson = JsonSerializer.Serialize(JsonDocument.Parse(actualJson).RootElement, _jsonSerializerOptions);

            canonicalActualJson.Should().Be(canonicalExpectedJson);
            // If the above fails, then there's a subtle difference. If it passes, the BeEquivalentTo on JsonElement might have issues.
            // For robustness, also keep the BeEquivalentTo check if the simpler string check passes.
            // using var expectedJsonDoc = JsonDocument.Parse(expectedJson);
            // using var actualJsonDoc = JsonDocument.Parse(actualJson);
            // actualJsonDoc.RootElement.Should().BeEquivalentTo(expectedJsonDoc.RootElement);
        }

        [Fact]
        public void Member_ShouldDeserializeCorrectly_WhenOptionalPropertiesAreMissing()
        {
            // Arrange
            // This JSON is missing optional fields like 'username', 'color' for user, and 'role', 'permission_level' for member.
            var json = """
            {
              "user": {
                "id": 101,
                "email": "partial@example.com"
              }
            }
            """;

            var expectedUser = new User(
                Id: 101,
                Username: null,
                Email: "partial@example.com",
                Color: null,
                ProfilePicture: null,
                Initials: null,
                Role: null,
                CustomRole: null,
                LastActive: null,
                DateJoined: null,
                DateInvited: null,
                ProfileInfo: null
            );

            var expectedMember = new Member(
                User: expectedUser,
                Role: null,
                PermissionLevel: null
            );

            // Act
            var actualMember = JsonSerializer.Deserialize<Member>(json, _jsonSerializerOptions);

            // Assert
            actualMember.Should().NotBeNull();
            actualMember.Should().BeEquivalentTo(expectedMember);
        }
    }
}
