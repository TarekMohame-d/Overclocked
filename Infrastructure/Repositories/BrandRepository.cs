using Application.Abstraction.Repositories;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class BrandRepository(ApplicationDbContext context) : GenericRepository<Brand>(context), IBrandRepository
{

}
