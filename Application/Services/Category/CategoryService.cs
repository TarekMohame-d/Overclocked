using Application.Abstraction.Repositories;
using Application.Abstraction.Services;

namespace Application.Services.Category;

public sealed partial class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public CategoryService(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }
}

