using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.BrandUseCases.DeleteBrand;

public class DeleteBrandRequestHandler(IBrandRepository brandRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteBrandRequest>
{
    public async Task<Result> Handle(DeleteBrandRequest request, CancellationToken ct)
    {
        Brand? brand = await brandRepository.GetByIdAsync(BrandId.Create(request.Id), ct);

        if (brand is null)
            return Result.Failure(BrandErrors.BrandNotFound(request.Id));

        brand.DeleteBrandImage();
        brandRepository.Remove(brand);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
