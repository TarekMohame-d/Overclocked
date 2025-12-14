using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Brand.Commands.UpdateBrand;

public class UpdateBrandCommandHandler(
    IBrandRepository brandRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateBrandCommand>
{
    public async Task<Result> Handle(UpdateBrandCommand command, CancellationToken cancellationToken)
    {
        Domain.BrandAggregate.Brand? brand = await brandRepository
            .FindAsync(BrandId.Create(command.Id), cancellationToken);

        if(brand is null)
        {
            return Result.Failure(BrandErrors.BrandNotFound(command.Id));
        }

        if(brand.Name != command.Name)
        {
            var exist = await brandRepository
                .AnyAsync(x => x.NormalizedName == command.Name.ToUpper(), cancellationToken);

            if(exist)
            {
                return Result.Failure(BrandErrors.BrandNameAlreadyExists);
            }
        }

        brand.Update(command.Name, command.ImageUrl);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
