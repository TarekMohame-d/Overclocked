using Overclocked.Application.Product.Commands.CreateProduct;
using Overclocked.Application.Product.Commands.DeleteProduct;
using Overclocked.Application.Product.Commands.UpdateProduct;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Product.Commands;

public interface IProductCommands
{
    Task<Result> CreateProductCommandHandler(CreateProductCommand command, CancellationToken cancellationToken);
    Task<Result> UpdateProductCommandHandler(UpdateProductCommand command, CancellationToken cancellationToken);
    Task<Result> DeleteProductCommandHandler(DeleteProductCommand command, CancellationToken cancellationToken);
}
