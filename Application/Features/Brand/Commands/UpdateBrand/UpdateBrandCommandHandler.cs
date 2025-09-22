using System.Net;
using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Features.Brand.Commands.UpdateBrand.Notifications;
using Application.Features.Brand.Mapping;
using Domain.Repositories;

namespace Application.Features.Brand.Commands.UpdateBrand;

public class UpdateBrandCommandHandler : ICommandHandler<UpdateBrandWithIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBrandRepository _brandRepository;
    private readonly IMediator _mediator;

    public UpdateBrandCommandHandler(
        IUnitOfWork unitOfWork,
        IBrandRepository brandRepository,
        IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _brandRepository = brandRepository;
        _mediator = mediator;
    }

    public async Task<Result> Handle(UpdateBrandWithIdCommand request, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.GetByIdAsync([request.Id], cancellationToken);

        if (brand is null)
            return Result.Failure(Errors.BrandNotFound, HttpStatusCode.NotFound);

        if (brand.Name != request.Name)
        {
            bool exist = await _brandRepository
                .AnyAsync(x => x.NormalizedName == request.Name.ToUpper(), cancellationToken);

            if (exist)
                return Result.Failure(Errors.BrandNameAlreadyExists, HttpStatusCode.Conflict);
        }

        brand.UpdateFrom(request);

        _brandRepository.Update(brand);

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _mediator.Publish(new BrandUpdatedNotification(request.Id, request.ImageFile, brand.Image), cancellationToken);

        return Result.Success();
    }
}
