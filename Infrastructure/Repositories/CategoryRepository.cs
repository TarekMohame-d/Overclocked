using Application.Abstraction.Repositories;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class CategoryRepository(ApplicationDbContext dbContext)
    : GenericRepository<Category>(dbContext), ICategoryRepository
{

}
