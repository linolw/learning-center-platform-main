using Acme.Center.Platform.Publishing.Domain.Model.ValueObjects;

namespace Acme.Center.Platform.Publishing.Domain.Model.Entities;

public partial class Asset(EAssetType type) : IPublishable
{
    public AssetId Id { get; }

    public AcmeAssetIdentifier AssetIdentifier { get; private set; } = new();

    public EPublishingStatus Status { get; private set; } = EPublishingStatus.Draft;

    public EAssetType Type { get; private set; } = type;

    public virtual bool Readable => false;
    
    public virtual bool Viewable => false;

    public void SendToEdit()
    {
        Status = EPublishingStatus.ReadyToEdit;
    }

    public void SendToApproval()
    {
        Status = EPublishingStatus.ReadyToApproved;
    }

    public void ApproveAndLock()
    {
        Status = EPublishingStatus.ApprovedAndLocked;
    }

    public void Reject()
    {
        Status = EPublishingStatus.Draft;
    }

    public void ReturnToEdit()
    {
        Status = EPublishingStatus.ReadyToEdit;
    }
    
    public virtual object GetContent()
    {
        return string.Empty;
    }
}