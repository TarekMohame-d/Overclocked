using Overclocked.Application.Abstraction;
using Overclocked.Application.Abstraction.Persistence;

namespace Overclocked.Application.Tag.Commands;

public sealed partial class TagCommands(
    ITagRepository tagRepository,
    IUnitOfWork unitOfWork) : ITagCommands;
