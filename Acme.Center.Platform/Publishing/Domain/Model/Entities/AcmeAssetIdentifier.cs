namespace Acme.Center.Platform.Publishing.Domain.Model.Entities;
/// <summary>
///     Represents an identifier for an asset
/// </summary>
/// <param name="Identifier"></param>
public record AcmeAssetIdentifier(Guid Identifier) 
{
    public AcmeAssetIdentifier() : this(Guid.NewGuid())
    {
        
    }
}