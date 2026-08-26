using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Entities;
using Brechosfera.Infrastructure.Data;

namespace Brechosfera.Infrastructure.Repositories;

public class PostRepository : Repository<Post>, IRepository<Post>
{
    public PostRepository(AppDbContext context) : base(context) { }
}
