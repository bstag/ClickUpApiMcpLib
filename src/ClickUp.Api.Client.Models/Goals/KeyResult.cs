using System.Collections.Generic;
using ClickUp.Api.Client.Models; // For User model

namespace ClickUp.Api.Client.Models.Goals;

/// <summary>
/// Represents a Key Result for a Goal in ClickUp.
/// </summary>
public record KeyResult
{
    public string Id { get; init; } = string.Empty;
    public string GoalId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty; // e.g., "number", "currency", "boolean"
    public string? Unit { get; init; } // e.g., "km", "$", "%"
    public int Creator { get; init; } // User ID
    public string DateCreated { get; init; } = string.Empty; // Timestamp
    public string? GoalPrettyId { get; init; }
    public string? PercentCompleted { get; init; } // Can be number or string in spec examples, string is safer.
    public bool Completed { get; init; }
    public List<string> TaskIds { get; init; } = new();
    public List<string> SubcategoryIds { get; init; } = new(); // Corresponds to 'list_ids' in request
    public List<User> Owners { get; init; } = new();
    public LastAction? LastAction { get; init; }

    // Fields from request bodies (CreateKeyResultrequest, EditKeyResultrequest)
    // These describe the target values for manual key results.
    public int? StepsStart { get; init; }
    public int? StepsEnd { get; init; }
    public int? StepsCurrent { get; init; } // Current value, distinct from LastAction.StepsCurrent which is historical
                                          // This might also be a double/decimal if type is currency/number with precision.
                                          // For now, int? based on common usage.
}
