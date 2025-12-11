using System.Net;
using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Product.Commands.UpdateProduct;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using ProductEntity = Overclocked.Domain.ProductAggregate.Product;

namespace Overclocked.Application.Product.Commands;

public sealed partial class ProductCommands
{
    public async Task<Result> UpdateProductCommandHandler(
        UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        ProductEntity? product = await productRepository
            .FirstOrDefaultAsync(
            x => x.Id == ProductId.Create(command.Id),
            include: x => x.Include(x => x.Images)
                .Include(x => x.Tags)
                .Include(x => x.Specifications),
            false,
            cancellationToken);

        if(product is null)
        {
            return Result.Failure(ProductErrors.ProductNotFound(command.Id), HttpStatusCode.NotFound);
        }

        if(product.Name != command.Name)
        {
            var exist = await productRepository.AnyAsync(
                x => x.NormalizedName == command.Name.ToUpper(), cancellationToken);

            if(exist)
            {
                return Result.Failure(ProductErrors.ProductNameAlreadyExists, HttpStatusCode.Conflict);
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
