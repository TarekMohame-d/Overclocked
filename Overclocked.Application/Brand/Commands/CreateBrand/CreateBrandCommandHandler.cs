using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.Common.Results;
using BrandEntity = Overclocked.Domain.BrandAggregate.Brand;

namespace Overclocked.Application.Brand.Commands.CreateBrand;

public class CreateBrandCommandHandler(
    IBrandRepository brandRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateBrandCommand>
{
    public async Task<Result> Handle(CreateBrandCommand command, CancellationToken cancellationToken)
    {
        var brand = BrandEntity.Create(command.Name, command.ImageUrl);

        await brandRepository.AddAsync(brand, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
