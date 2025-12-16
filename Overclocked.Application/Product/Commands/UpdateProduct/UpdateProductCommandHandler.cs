using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.Common.Shared.ValueObjects;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using ProductEntity = Overclocked.Domain.ProductAggregate.Product;

namespace Overclocked.Application.Product.Commands.UpdateProduct;

public class UpdateProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateProductCommand>
{
    public async Task<Result> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        ProductEntity? product = await productRepository
            .GetForUpdateAsync(ProductId.Create(command.Id), cancellationToken);

        if(product is null)
        {
            return Result.Failure(ProductErrors.ProductNotFound(command.Id));
        }

        if(product.Name != command.Name)
        {
            var exist = await productRepository.AnyAsync(
                x => x.NormalizedName == command.Name.ToUpper(), cancellationToken);

            if(exist)
            {
                return Result.Failure(ProductErrors.ProductNameAlreadyExists);
            }
        }

        product.Update(
            brandId: BrandId.Create(command.BrandId),
            categoryId: CategoryId.Create(command.CategoryId),
            name: command.Name,
            description: command.Description,
            thumbnail: command.Thumbnail,
            stock: command.StockQuantity,
            price: Money.Create(command.Price),
            discount: command.Discount is null ? Money.Zero : Money.Create((decimal)command.Discount),
            images: command.Images,
            specifications: command.Specifications,
            tags: command.Tags
        );

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
