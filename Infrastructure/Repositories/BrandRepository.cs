using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class BrandRepository : GenericRepository<Brand>, IBrandRepository
{
    private readonly ApplicationDbContext _context;
    public BrandRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}
