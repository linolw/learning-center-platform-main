using Acme.Center.Platform.Iam.Domain.Model.ValueObjects;
using Acme.Center.Platform.Shared.Domain.Model.Events;

namespace Acme.Center.Platform.Iam.Domain.Model.Events;

public record UserRegisteredEvent(UserId UserId, string Username) : IEvent;