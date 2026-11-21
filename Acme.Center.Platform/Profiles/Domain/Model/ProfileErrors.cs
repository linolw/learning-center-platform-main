using Acme.Center.Platform.Shared.Domain.Model.Errors;

namespace Acme.Center.Platform.Profiles.Domain.Model;

public static class ProfileErrors
{
    public static readonly Error ProfileNotFound =
        Error.NotFound("Profiles.ProfileNotfound", "The specified profile was not found.");
    
    public static readonly Error EmailAlreadyRegistered =
        Error.Conflict("Profiles.EmailAlreadyRegistered", "A profile with this email already exists");
    
    public static readonly Error InvalidProfileData =
        Error.NotFound("Profiles.InvalidProfileData", "The profile data provided is invalid");
    
    public static readonly Error ProfileCreationFailed =
        Error.NotFound("Profiles.ProfileCreationFailed", "An error occurred while creating the profile");
}