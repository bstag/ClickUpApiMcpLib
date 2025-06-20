using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Members
{
    /// <summary>
    /// Represents the response when fetching multiple members.
    /// </summary>
    public class GetMembersResponse
    {
        /// <summary>
        /// Gets or sets the list of Members.
        /// </summary>
        [JsonPropertyName("members")]
        public List<Member> Members { get; set; }

        /// <summary>
        /// Parameterless constructor for deserialization.
        /// </summary>
        public GetMembersResponse()
        {
            Members = new List<Member>();
        }

        public GetMembersResponse(List<Member> members)
        {
            Members = members;
        }
    }
}
