using System.Net;
using Overclocked.Application.Brand.Commands.UpdateBrand;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Brand.Commands;

public sealed partial class BrandCommands
{
    public async Task<Result> UpdateBrandCommandHandler(UpdateBrandCommand command, CancellationToken cancellationToken)
    {
        Domain.BrandAggregate.Brand? brand = await brandRepository
            .SingleOrDefaultAsync(x => x.Id == BrandId.Create(command.Id), asNoTracking: false, cancellationToken);

        if(brand is null)
        {
            return Result.Failure(BrandErrors.BrandNotFound(command.Id), HttpStatusCode.NotFound);
        }

        if(brand.Name != command.Name)
        {
            var exist = await brandRepository
                .AnyAsync(x => x.NormalizedName == command.Name.ToUpper(), cancellationToken);

            if(exist)
            {
                return Result.Failure(BrandErrors.BrandNameAlreadyExists, HttpStatusCode.Conflict);
            }
        }

        brand.Update(command.Name, command.ImageUrl);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
