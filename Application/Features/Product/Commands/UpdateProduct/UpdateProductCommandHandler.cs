using System.Net;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Features.Product.Commands.UpdateProduct.Notifications;
using Application.Features.Product.Mapping;

namespace Application.Features.Product.Commands.UpdateProduct;

public class UpdateProductCommandHandler : ICommandHandler<UpdateProductWithIdCommand, Result>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public UpdateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork, IMediator mediator)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<Result> Handle(UpdateProductWithIdCommand command, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync([command.Id], cancellationToken);

        if (product is null)
            return Result.Failure(Errors.ProductNotFound, HttpStatusCode.NotFound);

        if (product.Name != command.Name)
        {
            bool exist = await _productRepository
                .AnyAsync(x => x.NormalizedName == command.Name.ToUpper(), cancellationToken);

            if (exist)
                return Result.Failure(Errors.ProductNameAlreadyExists, HttpStatusCode.Conflict);
        }

        product.UpdateFrom(command);

        _productRepository.Update(product);

        await _mediator.Publish(new ProductUpdatedNotification(command.Id), cancellationToken);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
