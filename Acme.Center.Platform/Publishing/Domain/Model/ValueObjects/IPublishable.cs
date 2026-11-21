namespace Acme.Center.Platform.Publishing.Domain.Model.ValueObjects;

/// <summary>
///     Represent a publishable content item
/// </summary>
public interface IPublishable
{
    void SendToEdit();
    void SendToApproval();
    void ApproveAndLock();
    void Reject();
    void ReturnToEdit();
}