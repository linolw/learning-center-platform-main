using Acme.Center.Platform.Profiles.Application.CommandServices;
using Acme.Center.Platform.Profiles.Application.QueryServices;
using Acme.Center.Platform.Profiles.Domain.Model.Queries;
using Acme.Center.Platform.Profiles.Interfaces.Rest.Resources;
using Acme.Center.Platform.Profiles.Interfaces.Rest.Transform;
using Acme.Center.Platform.Resources.Errors;
using Acme.Center.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Acme.Center.Platform.Profiles.Interfaces.Rest;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Tags("Profiles")]
public class ProfilesController(
    IProfileCommandService profileCommandService,
    IProfileQueryService profileQueryService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
: ControllerBase
{
    private readonly IStringLocalizer<ErrorMessages> _errorLocalizer = errorLocalizer;
    private readonly ProblemDetailsFactory _problemDetailsFactory = problemDetailsFactory;

    [HttpGet("{profileId:int}")]
    [EndpointSummary("Get a profile by Id")]
    [EndpointDescription("Get a profile by Id its unique identifier")]
    [ProducesResponseType(typeof(ProfileResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfileById(int profileId, CancellationToken cancellationToken)
    {
        var getProfilesByIdQuery = new GetProfileByIdQuery(profileId);
        var profile = await profileQueryService.Handle(getProfilesByIdQuery, cancellationToken);

        return ProfilesActionsResultAssembler.ToActionResultFromGetProfileByIdResult(
            this, profile, _errorLocalizer, _problemDetailsFactory,
            foundProfile => Ok(ProfileResourceFromEntityAssembler.ToResourceFromEntity(foundProfile)));
    }
    
    [HttpGet]
    [EndpointSummary("Get all profiles")]
    [EndpointDescription("Get all profiles")]
    [ProducesResponseType(typeof(IEnumerable<ProfileResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllProfiles(CancellationToken cancellationToken)
    {
        var getAllProfiles = new GetAllProfilesQuery();
        var profiles = await profileQueryService.Handle(getAllProfiles, cancellationToken);
        var profileResources = profiles.Select(ProfileResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(profileResources);
    }
    
    
    [HttpPost]
    [EndpointSummary("Create profile")]
    [EndpointDescription("Create profile")]
    [ProducesResponseType(typeof(ProfileResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProfile(CreateProfileResource resource, CancellationToken cancellationToken)
    {
        var createProfileCommand = CreateProfileCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await profileCommandService.Handle(createProfileCommand, cancellationToken);
        
        return ProfilesActionsResultAssembler.ToActionResultFromCreateProfileResult(
            this,
            result,
            _errorLocalizer,
            _problemDetailsFactory,
            createdProfile => CreatedAtAction(nameof(GetProfileById), new { profileId = createdProfile.Id }, 
                ProfileResourceFromEntityAssembler.ToResourceFromEntity(createdProfile)));
    }
}