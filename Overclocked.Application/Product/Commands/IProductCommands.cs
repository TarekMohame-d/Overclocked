using Overclocked.Application.Product.Commands.CreateProduct;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Product.Commands;

public interface IProductCommands
{
    Task<Result> CreateProductCommandHandler(CreateProductCommand command, CancellationToken cancellationToken);
    //     Task<Result> UpdateProductAsync(UpdateProductRequest request, CancellationToken cancellationToken);
    //     Task<Result> DeleteProductAsync(Guid productId, CancellationToken cancellationToken);
}
