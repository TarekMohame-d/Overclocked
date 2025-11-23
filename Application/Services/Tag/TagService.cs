using Application.Abstraction.DomainServices;
using Application.Abstraction.Repositories;

namespace Application.Services.Tag;

public sealed partial class TagService(
    ITagRepository tagRepository,
    IUnitOfWork unitOfWork)
    : ITagService;
