using Brechosfera.Domain.Entities;
using Brechosfera.Domain.Repositories;
using Brechosfera.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Brechosfera.Infrastructure.Repositories;

public class EventRepository : Repository<Event>, IEventRepository
{
    public EventRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Event>> ListByStoreIdAsync(Guid storeId, CancellationToken cancellationToken)
    {
        return await DbSet
            .Where(x => x.ResponsibleStoreId == storeId)
            .Include(x => x.Address)
            .ToListAsync();
    }
}
