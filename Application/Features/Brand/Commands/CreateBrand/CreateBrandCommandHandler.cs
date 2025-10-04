using System.Net;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Features.Brand.Commands.CreateBrand.Notifications;
using Application.Features.Brand.Mapping;

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

    public async Task<Result> Handle(CreateBrandCommand command, CancellationToken cancellationToken)
    {
        var brand = command.ToEntity();

        await _brandRepository.AddAsync(brand, cancellationToken);

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _mediator.Publish(new BrandCreatedNotification(brand.Id), cancellationToken);

        return Result.Success(HttpStatusCode.Created);
    }
}
