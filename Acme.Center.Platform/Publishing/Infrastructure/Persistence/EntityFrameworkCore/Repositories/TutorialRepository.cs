using Acme.Center.Platform.Publishing.Domain.Model.Aggregates;
using Acme.Center.Platform.Publishing.Domain.Repositories;
using Acme.Center.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Acme.Center.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Acme.Center.Platform.Publishing.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class TutorialRepository(AppDbContext context) : BaseRepository<Tutorial>(context), ITutorialRepository
{
    public new async Task<Tutorial?> FindByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Tutorial>()
            .Include(tutorial => tutorial.Category)
            .FirstOrDefaultAsync(tutorial => tutorial.Id == id, cancellationToken);
    }

    public new async Task<Tutorial?> FindByIdAsNoTrackingAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Tutorial>()
            .AsNoTracking()
            .AsSingleQuery()
            .Include(tutorial => tutorial.Category)
            .FirstOrDefaultAsync(tutorial => tutorial.Id == id, cancellationToken);
    }

    public new async Task<IEnumerable<Tutorial>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<Tutorial>()
            .Include(tutorial => tutorial.Category)
            .ToListAsync(cancellationToken);
    }

    public new async Task<IEnumerable<Tutorial>> ListAsNoTrackingAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<Tutorial>()
            .AsNoTracking()
            .AsSingleQuery()
            .Include(tutorial => tutorial.Category)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Tutorial>> FindByCategoryIdAsync(int categoryId, CancellationToken cancellationToken)
    {
        return await Context.Set<Tutorial>()
            .Include(tutorial => tutorial.Category)
            .Where(tutorial => tutorial.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Tutorial>> FindByCategoryIdAsNoTrackingAsync(int categoryId, CancellationToken cancellationToken)
    {
        return await Context.Set<Tutorial>()
            .AsNoTracking()
            .AsSingleQuery()
            .Include(tutorial => tutorial.Category)
            .Where(tutorial => tutorial.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByTitleAsync(string title, CancellationToken cancellationToken)
    {
        return await Context.Set<Tutorial>().AnyAsync(tutorial => tutorial.Title == title, cancellationToken);
    }
}