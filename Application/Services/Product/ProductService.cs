using Application.Abstraction.Repositories;
using Application.Abstraction.Services;

namespace Application.Services.Product;

public sealed partial class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public ProductService(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }
}
