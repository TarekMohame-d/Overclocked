using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Brand.Commands.DeleteBrand;

public class DeleteBrandCommandHandler(
    IBrandRepository brandRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteBrandCommand>
{
    public async Task<Result> Handle(DeleteBrandCommand command, CancellationToken cancellationToken)
    {
        Domain.BrandAggregate.Brand? brand = await brandRepository
            .FindAsync(BrandId.Create(command.Id), cancellationToken);

        if(brand is null)
        {
            return Result.Failure(BrandErrors.BrandNotFound(command.Id));
        }

        brand.Delete();
        brandRepository.Delete(brand);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
