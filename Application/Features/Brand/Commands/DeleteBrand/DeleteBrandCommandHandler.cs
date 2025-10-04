using System.Net;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Features.Brand.Commands.DeleteBrand.Notifications;

namespace Application.Features.Brand.Commands.DeleteBrand;

public class DeleteBrandCommandHandler : ICommandHandler<DeleteBrandCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBrandRepository _brandRepository;
    private readonly IMediator _mediator;

    public DeleteBrandCommandHandler(
        IUnitOfWork unitOfWork,
        IBrandRepository brandRepository,
        IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _brandRepository = brandRepository;
        _mediator = mediator;
    }

    public async Task<Result> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.GetByIdAsync([request.Id], cancellationToken);

        if (brand is null)
            return Result.Failure(Errors.BrandNotFound, HttpStatusCode.NotFound);

        _brandRepository.Delete(brand);

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _mediator.Publish(new BrandDeletedNotification(brand.Id, brand.Image), cancellationToken);

        return Result.Success();
    }
}
