using Application.Abstraction.Repositories;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class CartRepository(ApplicationDbContext dbContext) : GenericRepository<Cart>(dbContext), ICartRepository;
