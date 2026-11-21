using Acme.Center.Platform.Shared.Domain.Model.ValueObjects;

namespace Acme.Center.Platform.Iam.Domain.Model.ValueObjects;
/// <summary>
///     Represents a unique identifier for a user.
/// </summary>
/// <param name="Value"></param>
public readonly record struct UserId(int Value) : IEntityId<int>
{
    public static implicit operator int(UserId id) => id.Value;
    public static implicit operator UserId(int value) => new(value);
    
    public override string ToString() => Value.ToString();
}