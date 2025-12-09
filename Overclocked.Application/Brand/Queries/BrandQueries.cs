using Overclocked.Application.Abstraction.Persistence;

namespace Overclocked.Application.Brand.Queries;

public sealed partial class BrandQueries(IBrandRepository brandRepository) : IBrandQueries;
