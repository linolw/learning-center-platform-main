using Acme.Center.Platform.Profiles.Domain.Model.Commands;
using Acme.Center.Platform.Profiles.Domain.Model.Events;
using Acme.Center.Platform.Profiles.Domain.Model.ValueObjects;
using Acme.Center.Platform.Shared.Domain.Model.Aggregates.Internal;

namespace Acme.Center.Platform.Profiles.Domain.Model.Aggregates;

public partial class Profile : AggregateRoot
{
    public Profile()
    {
        Name = new PersonName();
        Address = new StreetAddress();
        Email = new EmailAddress();
    }
    
    public Profile(string firsName, string lastName, string email, string street, string city, string state,
        string postalCode, string country)

    {
        Name = new PersonName(firsName, lastName);
        Email = new EmailAddress(email);
        Address = new StreetAddress(street, city, state, postalCode, country);
        AddDomainEvent(new ProfileCreatedEvent(Id, Email));
    }

    public Profile(CreateProfileCommand command)
    {
        Name = new PersonName(command.FirstName, command.LastName);
        Email = new EmailAddress(command.Email);
        Address = new StreetAddress(command.Street, command.City, command.PostalCode, command.Country);
        AddDomainEvent(new ProfileCreatedEvent(Id, Email));
    }
    
    public ProfileId Id { get; }
    
    public PersonName Name { get; private set; }
    public StreetAddress Address { get; private set; }
    public EmailAddress Email { get; private set; }
    
    public string FullName => Name.FullName;
    public string EmailAddress => Email.Address;
    public string StreetAddress => Address.FullAddress;
}