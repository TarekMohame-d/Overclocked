using Application.Abstraction.Repositories;
using Domain.Entities;
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
