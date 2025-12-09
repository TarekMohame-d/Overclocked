using Overclocked.Application.Brand.Commands.CreateBrand;
using Overclocked.Application.Brand.Commands.DeleteBrand;
using Overclocked.Application.Brand.Commands.UpdateBrand;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Brand.Commands;

public interface IBrandCommands
{
    Task<Result> CreateBrandCommandHandler(CreateBrandCommand command, CancellationToken cancellationToken);
    Task<Result> UpdateBrandCommandHandler(UpdateBrandCommand command, CancellationToken cancellationToken);
    Task<Result> DeleteBrandCommandHandler(DeleteBrandCommand command, CancellationToken cancellationToken);
}
