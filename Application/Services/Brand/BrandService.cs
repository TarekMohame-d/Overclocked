using Application.Abstraction.Repositories;
using Application.Abstraction.Services;

namespace Application.Services.Brand;

public sealed partial class BrandService : IBrandService
{
    private readonly IBrandRepository _brandRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public BrandService(
        IBrandRepository brandRepository,
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService)
    {
        _brandRepository = brandRepository;
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }
}
