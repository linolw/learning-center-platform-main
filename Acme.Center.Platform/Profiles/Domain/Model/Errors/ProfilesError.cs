namespace Acme.Center.Platform.Profiles.Domain.Model.Errors;

public enum ProfilesError
{
    None, 
    ProfileNotFound,
    EmailAlreadyRegistered,
    InvalidProfileData,
    OperationCancelled,
    DatabaseError,
    InternalServerError
}