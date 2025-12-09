using Overclocked.Application.Abstraction;
using Overclocked.Application.Abstraction.Persistence;

namespace Overclocked.Application.Brand.Commands;

public sealed partial class BrandCommands(
    IBrandRepository brandRepository,
    IUnitOfWork unitOfWork) : IBrandCommands;
