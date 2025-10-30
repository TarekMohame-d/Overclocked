using System.Net;
using Application.Common.Results;
using Application.Services.Product.DTOs.Request;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Product;

public sealed partial class ProductService
{
    public async Task<Result> DeleteProductAsync(DeleteProductRequest request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdWithImagesAsync(request.Id, cancellationToken);

        if (product is null)
            return Result.Failure(Errors.ProductNotFound, HttpStatusCode.NotFound);

        _productRepository.Delete(product);

        if (product.ProductImages is not null && product.ProductImages.Count != 0)
        {
            var images = product.ProductImages.Select(x => x.Image);
            await DeleteProductImages(images);
        }

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }

    private async Task DeleteProductImages(IEnumerable<string> images)
    {
        List<Task<bool>> tasks = images.Select(image => _fileStorageService.DeleteFileAsync(image)).ToList();
        await Task.WhenAll(tasks);
    }
}
