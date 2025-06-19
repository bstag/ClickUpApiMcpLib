namespace ClickUp.Api.Client.Models.Goals;

/// <summary>
/// Represents the last action taken on a Key Result.
/// </summary>
public record LastAction
{
    public string Id { get; init; } = string.Empty;
    public string KeyResultId { get; init; } = string.Empty;
    public int UserId { get; init; } // Renamed from 'userid' for C# convention
    public string DateModified { get; init; } = string.Empty; // Timestamp
    public string? Note { get; init; }

    // These can be numbers or null. Using string? for flexibility from spec examples,
    // but could be int? or double? if API is consistent.
    public string? StepsTaken { get; init; }
    public string? StepsBefore { get; init; }
    public string? StepsCurrent { get; init; }

    // From EditKeyResultresponse (LastAction1)
    public double? StepsBeforeFloat { get; init; }
    public double? StepsTakenFloat { get; init; }
    public double? StepsCurrentFloat { get; init; } // Spec says string? null, but float makes more sense
}
