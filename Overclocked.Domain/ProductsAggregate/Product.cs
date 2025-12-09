using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.ProductsAggregate.Entities;
using Overclocked.Domain.ProductsAggregate.ValueObjects;

namespace Overclocked.Domain.ProductsAggregate;

public sealed class Product : AggregateRoot<ProductId>
{
    public BrandId BrandId { get; private set; }
    public CategoryId CategoryId { get; private set; }

    public string Name { get; private set; }
    public string NormalizedName { get; private set; }
    public string Description { get; private set; }
    public string Thumbnail { get; private set; }

    public Money Price { get; private set; }
    public Money Discount { get; private set; }
    public int StockQuantity { get; private set; }
    public ProductRating ProductRating { get; private set; }
    public bool IsDeleted { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<ProductImage> _images = [];
    public IReadOnlyList<ProductImage> Images => _images.AsReadOnly();

    private readonly List<Specification> _specifications = [];
    public IReadOnlyList<Specification> Specifications => _specifications.AsReadOnly();

    private readonly List<ProductTag> _tags = [];
    public IReadOnlyCollection<ProductTag> Tags => _tags.AsReadOnly();

    private Product()
    {
    }
    private Product(
        ProductId id,
        BrandId brandId,
        CategoryId categoryId,
        string name,
        string description,
        string thumbnail,
        int stock,
        Money price,
        ProductRating? productRating = null,
        Money? discount = null,
        bool isDeleted = false) : base(id)
    {
        BrandId = brandId;
        CategoryId = categoryId;
        Name = name;
        NormalizedName = name.ToUpper();
        Description = description;
        Thumbnail = thumbnail;
        Price = price;
        StockQuantity = stock;

        ProductRating = productRating ?? ProductRating.Zero;
        Discount = discount ?? Money.Zero;

        IsDeleted = isDeleted;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Product Create(
        ProductId id,
        BrandId brandId,
        CategoryId categoryId,
        string name,
        string description,
        string thumbnail,
        int stock,
        Money price,
        ProductRating? productRating = null,
        Money? discount = null,
        bool isDeleted = false) =>
        new(
            id: id,
            brandId: brandId,
            categoryId: categoryId,
            name: name,
            description: description,
            thumbnail: thumbnail,
            stock: stock,
            price: price,
            productRating: productRating,
            discount: discount,
            isDeleted: isDeleted);

    // --- Domain Behaviors ---

    // public void CalculateRating(int newReviewRating)
    // {
    //     var newRating = ((Rating * ReviewCount) + newReviewRating) / (ReviewCount + 1);

    //     Rating = Math.Clamp(newRating, 0, 5);

    //     ReviewCount++;
    // }

    // public void RemoveRating(int oldReviewRating)
    // {
    //     if(ReviewCount <= 1)
    //     {
    //         Rating = 0;
    //         ReviewCount = 0;
    //         return;
    //     }

    //     var currentTotalScore = Rating * ReviewCount;
    //     var newRating = (currentTotalScore - oldReviewRating) / (ReviewCount - 1);

    //     Rating = Math.Clamp(newRating, 0, 5);
    //     ReviewCount--;
    // }

    // public void UpdateRating(int oldReviewRating, int newReviewRating)
    // {
    //     if(ReviewCount == 0)
    //         return;

    //     if(oldReviewRating == newReviewRating)
    //         return;

    //     var currentTotalScore = Rating * ReviewCount;
    //     var newRating = (currentTotalScore - oldReviewRating + newReviewRating) / ReviewCount;

    //     Rating = Math.Clamp(newRating, 0, 5);
    // }
}
