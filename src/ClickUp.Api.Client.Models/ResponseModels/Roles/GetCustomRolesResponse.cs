using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Roles
{
    /// <summary>
    /// Represents the response for getting a list of Custom Roles.
    /// </summary>
    public class GetCustomRolesResponse
    {
        /// <summary>
        /// Gets or sets the list of Custom Roles.
        /// </summary>
        [JsonPropertyName("roles")]
        public List<CustomRole> Roles { get; set; }

        public GetCustomRolesResponse()
        {
            Roles = new List<CustomRole>();
        }

        public GetCustomRolesResponse(List<CustomRole> roles)
        {
            Roles = roles;
        }
    }
}
