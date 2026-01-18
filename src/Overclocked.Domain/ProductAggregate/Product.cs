using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Domain.Common.Shared.ValueObjects.Image;
using Overclocked.Domain.Common.Shared.ValueObjects.Money;
using Overclocked.Domain.ProductAggregate.Entities;
using Overclocked.Domain.ProductAggregate.Events;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.TagAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.ProductAggregate;

public sealed class Product : AggregateRoot<ProductId>
{
    public BrandId BrandId { get; private set; } = null!;
    public CategoryId CategoryId { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string NormalizedName { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public Image Thumbnail { get; private set; } = null!;
    public Money Price { get; private set; } = null!;
    public DiscountRate Discount { get; private set; } = null!;
    public int StockQuantity { get; private set; }
    public ProductRating ProductRating { get; private set; } = null!;
    public bool IsDeleted { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private readonly List<ProductImage> _images = [];
    public IReadOnlyList<ProductImage> Images => _images.AsReadOnly();

    private readonly List<Specification> _specifications = [];
    public IReadOnlyList<Specification> Specifications => _specifications.AsReadOnly();

    private readonly List<ProductTag> _productTags = [];
    public IReadOnlyList<ProductTag> ProductTags => _productTags.AsReadOnly();

    // Navigation Properties
    public Brand? Brand { get; }
    public Category? Category { get; }

    private Product() { }

    private Product(
        ProductId id,
        BrandId brandId,
        CategoryId categoryId,
        string name,
        string description,
        Image thumbnail,
        int stock,
        Money price,
        DiscountRate discount,
        IEnumerable<ProductImage> images,
        IEnumerable<Specification> specifications,
        IEnumerable<ProductTag> productTags
    )
        : base(id)
    {
        BrandId = brandId;
        CategoryId = categoryId;
        Name = name;
        NormalizedName = name.ToUpperInvariant();
        Description = description;
        Thumbnail = thumbnail;
        StockQuantity = stock;
        Price = price;
        Discount = discount;

        _images.AddRange(images);
        _specifications.AddRange(specifications);
        _productTags.AddRange(productTags);

        ProductRating = ProductRating.Zero;
        IsDeleted = false;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public static Result<Product> Create(
        BrandId brandId,
        CategoryId categoryId,
        string name,
        string description,
        Image thumbnail,
        int stock,
        Money price,
        DiscountRate discount,
        List<ProductImage> images,
        List<Specification> specifications,
        List<ProductTag> productTags
    )
    {
        Result validationResult = ValidateState(name, description, stock, specifications);
        if (validationResult.IsFailure)
            return Result.Failure<Product>(validationResult.Error);

        var product = new Product(
            ProductId.Create(),
            brandId,
            categoryId,
            name,
            description,
            thumbnail,
            stock,
            price,
            discount,
            images,
            specifications,
            productTags
        );

        return Result.Success(product);
    }

    public Result Update(
        BrandId brandId,
        CategoryId categoryId,
        string name,
        string description,
        Image thumbnail,
        int stock,
        Money price,
        DiscountRate discount,
        List<ProductImage>? images,
        List<Specification> specifications,
        List<ProductTag> productTags,
        bool isDeleted = false
    )
    {
        Result validationResult = ValidateState(name, description, stock, specifications);
        if (validationResult.IsFailure)
            return Result.Failure<Product>(validationResult.Error);

        BrandId = brandId;
        CategoryId = categoryId;
        Name = name;
        NormalizedName = name.ToUpperInvariant();
        Description = description;
        Thumbnail = thumbnail;
        StockQuantity = stock;
        Price = price;
        Discount = discount;

        UpdateProductImages(images);
        UpdateProductTags(productTags);
        UpdateProductSpecifications(specifications);

        IsDeleted = isDeleted;
        UpdatedAt = DateTimeOffset.UtcNow;

        return Result.Success(this);
    }

    private void UpdateProductImages(List<ProductImage>? images)
    {
        List<string> imagesUrls = images?.Select(x => x.Image.Value).ToList() ?? [];
        var imagesToRemove = _images.Where(x => !imagesUrls.Contains(x.Image.Value)).ToList();

        if (imagesToRemove.Count != 0)
        {
            foreach (ProductImage image in imagesToRemove)
                _images.Remove(image);

            List<string> urlsToDelete = imagesToRemove.ConvertAll(x => x.Image.Value);

            RaiseDomainEvent(new ProductImagesRemovedEvent(Id.Value, urlsToDelete));
        }

        List<string> existingImageUrls = _images.ConvertAll(x => x.Image.Value);

        foreach (var imageUrl in imagesUrls)
        {
            if (!existingImageUrls.Contains(imageUrl))
                _images.Add(ProductImage.Create(Image.Create(imageUrl).Value));
        }
    }

    private void UpdateProductTags(List<ProductTag> tags)
    {
        List<Guid> inputTags = tags.ConvertAll(x => x.TagId.Value);

        _productTags.RemoveAll(x => !inputTags.Contains(x.TagId.Value));

        var existingTagIds = new HashSet<Guid>(_productTags.Select(x => x.TagId.Value));

        foreach (Guid tagId in inputTags)
        {
            if (!existingTagIds.Contains(tagId))
                _productTags.Add(ProductTag.Create(TagId.Create(tagId)));
        }
    }

    private void UpdateProductSpecifications(List<Specification> specifications)
    {
        var newSpecsDict = specifications.DistinctBy(x => x.Name).ToDictionary(x => x.Name, x => x.Value);

        _specifications.RemoveAll(x => !newSpecsDict.ContainsKey(x.Name));

        var currentSpecsDict = _specifications.ToDictionary(x => x.Name);

        foreach (Specification spec in specifications)
        {
            if (currentSpecsDict.TryGetValue(spec.Name, out Specification? existingSpec))
            {
                if (existingSpec.Value != spec.Value)
                    existingSpec.UpdateValue(spec.Value);
            }
            else
            {
                _specifications.Add(spec);
            }
        }
    }

    public Money CalculateFinalPrice()
    {
        if (Discount == DiscountRate.Zero)
            return Price;

        var multiplier = 1.0m - Discount.Value;

        return Price * multiplier;
    }

    public void DeleteProductImages()
    {
        List<string> images = _images.ConvertAll(x => x.Image.Value);
        images.Add(Thumbnail.Value);
        RaiseDomainEvent(new ProductDeletedEvent(Id.Value, images));
    }

    public Result AddReviewVote(int reviewRating)
    {
        if (reviewRating is < 1 or > 5)
            return Result.Failure<Product>(ProductErrors.InvalidReviewRating);

        var newTotalScore = ProductRating.TotalScore + reviewRating;
        var newCount = ProductRating.ReviewCount + 1;
        Result<ProductRating> productRatingResult = ProductRating.Create(newTotalScore, newCount);

        if (productRatingResult.IsFailure)
            return Result.Failure<Product>(productRatingResult.Error);

        ProductRating = productRatingResult.Value;
        UpdatedAt = DateTimeOffset.UtcNow;

        return Result.Success();
    }

    public Result RemoveReviewVote(int reviewRating)
    {
        if (reviewRating is < 1 or > 5)
            return Result.Failure<Product>(ProductErrors.InvalidReviewRating);

        if (ProductRating.ReviewCount == 0)
        {
            ProductRating = ProductRating.Zero;
            return Result.Success();
        }

        var newTotalScore = ProductRating.TotalScore - reviewRating;
        var newCount = ProductRating.ReviewCount - 1;

        Result<ProductRating> productRatingResult = ProductRating.Create(newTotalScore, newCount);

        if (productRatingResult.IsFailure)
            return Result.Failure<Product>(productRatingResult.Error);

        ProductRating = productRatingResult.Value;
        UpdatedAt = DateTimeOffset.UtcNow;

        return Result.Success();
    }

    public Result UpdateReviewVote(int oldReviewRating, int newReviewRating)
    {
        if ((newReviewRating is < 1 or > 5) || (oldReviewRating is < 1 or > 5))
            return Result.Failure<Product>(ProductErrors.InvalidReviewRating);

        if (ProductRating.ReviewCount == 0 || oldReviewRating == newReviewRating)
            return Result.Success();

        var newTotalScore = ProductRating.TotalScore - oldReviewRating + newReviewRating;

        Result<ProductRating> productRatingResult = ProductRating.Create(newTotalScore, ProductRating.ReviewCount);

        if (productRatingResult.IsFailure)
            return Result.Failure<Product>(productRatingResult.Error);

        ProductRating = productRatingResult.Value;
        UpdatedAt = DateTimeOffset.UtcNow;

        return Result.Success();
    }

    public Result RemoveStock(int quantity)
    {
        if (StockQuantity < quantity)
            return Result.Failure(ProductErrors.NotEnoughStock);

        StockQuantity -= quantity;
        UpdatedAt = DateTimeOffset.UtcNow;

        return Result.Success();
    }

    public void AddStock(int quantity)
    {
        StockQuantity += quantity;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private static Result ValidateState(string name, string description, int stock, List<Specification> specifications)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(ProductErrors.ProductNameIsRequired);

        if (name.Length > 50)
            return Result.Failure(ProductErrors.ProductNameIsTooLong);

        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure(ProductErrors.ProductDescriptionIsRequired);

        if (description.Length > 500)
            return Result.Failure(ProductErrors.ProductDescriptionIsTooLong);

        if (stock < 0)
            return Result.Failure(ProductErrors.ProductStockIsInvalid);

        var hasDuplicateSpecs = specifications.GroupBy(x => x.NormalizedName).Any(g => g.Count() > 1);

        if (hasDuplicateSpecs)
            return Result.Failure(ProductErrors.SpecificationNameNotUnique);

        return Result.Success();
    }
}
