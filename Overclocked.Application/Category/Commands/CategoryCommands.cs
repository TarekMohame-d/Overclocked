using Overclocked.Application.Abstraction;
using Overclocked.Application.Abstraction.Persistence;

namespace Overclocked.Application.Category.Commands;

public sealed partial class CategoryCommands(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : ICategoryCommands;
