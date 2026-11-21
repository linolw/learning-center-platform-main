namespace Acme.Center.Platform.Publishing.Domain.Model.ValueObjects;

/// <summary>
///     Represents the publishing status of a content item 
/// </summary>
public enum EPublishingStatus
{
    Draft,
    ReadyToEdit,
    ReadyToApproved,
    ApprovedAndLocked
}