using System.Net;
using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Features.Brand.Commands.CreateBrand.Notifications;
using Application.Features.Brand.Mapping;
using Domain.Repositories;

namespace Application.Features.Brand.Commands.CreateBrand;

public class CreateBrandCommandHandler : ICommandHandler<CreateBrandCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBrandRepository _brandRepository;
    private readonly IMediator _mediator;

    public CreateBrandCommandHandler(
        IUnitOfWork unitOfWork,
        IBrandRepository brandRepository,
        IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _brandRepository = brandRepository;
        _mediator = mediator;
    }

    public async Task<Result> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = request.ToEntity("temp.jpg");

        await _brandRepository.AddAsync(brand, cancellationToken);

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _mediator.Publish(new BrandCreatedNotification(brand.Id, request.ImageFile), cancellationToken);

        return Result.Success(HttpStatusCode.Created);
    }
}
