using Overclocked.Application.Abstraction.Persistence;

namespace Overclocked.Application.Tag.Queries;

public sealed partial class TagQueries(ITagRepository tagRepository) : ITagQueries;
