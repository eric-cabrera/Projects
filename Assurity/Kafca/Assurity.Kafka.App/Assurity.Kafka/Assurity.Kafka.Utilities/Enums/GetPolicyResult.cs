namespace Assurity.Kafka.Utilities.Enums;

/// <summary>
/// When calling BasePolicyEngine.GetPolicy(), there are several reasons to return a null policy.
/// This enum attempts to give a reason with the result for more clarity.
/// </summary>
public enum GetPolicyResult
{
    /// <summary>
    /// The policy was found and met all business rules.
    /// </summary>
    Found,

    /// <summary>
    /// The policy either did not meet all business rules or was not present in the database.
    /// </summary>
    NotFound,

    /// <summary>
    /// The policy has an initial payment declined that is beyond the retention duration.
    /// </summary>
    HasInitialPaymentDeclinedThatIsBeyondRetentionDuration,

    /// <summary>
    /// An exception was thrown when attempting to get the Policy.
    /// </summary>
    ExceptionThrown,

    ApplicationDateNull
}
