using Acme.Center.Platform.Publishing.Domain.Model.ValueObjects;

namespace Acme.Center.Platform.Publishing.Domain.Model.Entities;

public class ReadableContentAsset : Asset
{
    public ReadableContentAsset() : base(EAssetType.ReadableContentItem)
    {
        ReadableContent = string.Empty;
    }

    public ReadableContentAsset(string content) : base(EAssetType.ReadableContentItem)
    {
        ReadableContent = content;
    }
    
    public string ReadableContent { get;   set; }

    public override bool Readable => false;
    public override bool Viewable => true;
    
}
