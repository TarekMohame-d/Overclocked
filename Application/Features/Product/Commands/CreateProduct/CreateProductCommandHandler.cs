using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Features.Product.Mapping;
using Domain.Entities;

namespace Application.Features.Product.Commands.CreateProduct;

public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, Result>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<Result> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var product = command.ToEntity();

        product.TagProducts = command.Tags.Select(t => new TagProduct
        {
            ProductId = product.Id,
            TagId = t
        }).ToList();

        product.ProductImages = command.Images is not null
            ? command.Images.Select(url => new ProductImage
            {
                ProductId = product.Id,
                Image = url
            }).ToList()
            : [];

        product.Specifications = command.Specification.Select(s => new Specification
        {
            ProductId = product.Id,
            Name = s.Name,
            Value = s.Value
        }).ToList();

        await _productRepository.AddAsync(product);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
