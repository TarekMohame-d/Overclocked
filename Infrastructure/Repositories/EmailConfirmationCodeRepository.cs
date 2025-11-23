using Application.Abstraction.Repositories;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class EmailConfirmationCodeRepository(ApplicationDbContext dbContext)
        : GenericRepository<EmailConfirmationCode>(dbContext), IEmailConfirmationCodeRepository
{

}
