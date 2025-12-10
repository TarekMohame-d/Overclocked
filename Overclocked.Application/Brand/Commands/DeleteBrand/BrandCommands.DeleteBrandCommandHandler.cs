using System.Net;
using Overclocked.Application.Brand.Commands.DeleteBrand;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.TagAggregate.ValueObjects;

namespace Overclocked.Application.Brand.Commands;

public sealed partial class BrandCommands
{
    public async Task<Result> DeleteBrandCommandHandler(DeleteBrandCommand command, CancellationToken cancellationToken)
    {
        Domain.BrandAggregate.Brand? brand = await brandRepository
            .GetByIdAsync(BrandId.Create(command.Id), cancellationToken);

        if(brand is null)
        {
            return Result.Failure(BrandErrors.BrandNotFound(command.Id), HttpStatusCode.NotFound);
        }

        brand.Delete();
        brandRepository.Delete(brand);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
