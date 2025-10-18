using System.Net;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Features.Product.Commands.DeleteProduct.Notifications;

namespace Application.Features.Product.Commands.DeleteProduct;

public class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _ProductRepository;
    private readonly IMediator _mediator;

    public DeleteProductCommandHandler(
        IUnitOfWork unitOfWork,
        IProductRepository ProductRepository,
        IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _ProductRepository = ProductRepository;
        _mediator = mediator;
    }

    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var Product = await _ProductRepository.GetByIdAsync([request.Id], cancellationToken);

        if (Product is null)
            return Result.Failure(Errors.ProductNotFound, HttpStatusCode.NotFound);

        _ProductRepository.Delete(Product);

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _mediator.Publish(new ProductDeletedNotification(Product.Id), cancellationToken);

        return Result.Success();
    }
}
