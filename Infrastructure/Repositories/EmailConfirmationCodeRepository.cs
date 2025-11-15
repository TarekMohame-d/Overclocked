using Application.Abstraction.Repositories;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class EmailConfirmationCodeRepository : GenericRepository<EmailConfirmationCode>, IEmailConfirmationCodeRepository
{
    private readonly ApplicationDbContext _dbContext;

    public EmailConfirmationCodeRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
