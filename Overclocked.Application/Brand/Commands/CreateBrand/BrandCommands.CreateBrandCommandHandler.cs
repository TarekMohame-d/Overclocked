using System.Net;
using Overclocked.Application.Brand.Commands.CreateBrand;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.Common.Results;
using BrandEntity = Overclocked.Domain.BrandAggregate.Brand;

namespace Overclocked.Application.Brand.Commands;

public sealed partial class BrandCommands
{
    public async Task<Result> CreateBrandCommandHandler(CreateBrandCommand command, CancellationToken cancellationToken)
    {
        var brand = BrandEntity.Create(BrandId.Create(), command.Name, command.ImageUrl);

        await brandRepository.AddAsync(brand, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(HttpStatusCode.Created);
    }
}
