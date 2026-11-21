using Acme.Center.Platform.Profiles.Domain.Model.Aggregates;
using Acme.Center.Platform.Profiles.Domain.Model.ValueObjects;
using Acme.Center.Platform.Profiles.Domain.Repositories;
using Acme.Center.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Acme.Center.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Acme.Center.Platform.Profiles.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class ProfileRepository(AppDbContext context)
: BaseRepository<Profile>(context), IProfileRepository
{
    public async Task<Profile?> FindByIdAsNoTrackingAsync(ProfileId id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Profile>()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Profile?> FindByProfileByEmailAsync(EmailAddress email, CancellationToken cancellationToken)
    {
        return await Context.Set<Profile>()
            .FirstOrDefaultAsync(p => p.Email == email, cancellationToken);
    }

    public async Task<Profile?> FindProfileByEmailAsNoTrackingAsync(EmailAddress email, CancellationToken cancellationToken)
    {
        return await Context.Set<Profile>()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Email == email, cancellationToken);
    }
    
    
}