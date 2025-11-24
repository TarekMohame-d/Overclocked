using Application.Abstraction.DomainServices;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;

namespace Application.Services.Category;

public sealed partial class CategoryService(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    IEventDispatcher eventDispatcher)
    : ICategoryService;
