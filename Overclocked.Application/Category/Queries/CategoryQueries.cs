using Overclocked.Application.Abstraction.Persistence;

namespace Overclocked.Application.Category.Queries;

public sealed partial class CategoryQueries(ICategoryRepository categoryRepository) : ICategoryQueries;
