using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.ProductAggregate.Entities;
using Overclocked.Domain.ProductAggregate.Events;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.TagAggregate.ValueObjects;

namespace Overclocked.Domain.ProductAggregate;

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
        Money discount,
        IEnumerable<ProductImage> images,
        IEnumerable<Specification> specifications,
        IEnumerable<ProductTag> tags,
        bool isDeleted = false) : base(id)
    {
        BrandId = brandId;
        CategoryId = categoryId;
        Name = name;
        NormalizedName = name.ToUpper();
        Description = description;
        Thumbnail = thumbnail;
        StockQuantity = stock;
        Price = price;
        Discount = discount;
        _images.AddRange(images);
        _specifications.AddRange(specifications);
        _tags.AddRange(tags);

        ProductRating = ProductRating.Zero;
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
        Money discount,
        IEnumerable<ProductImage> images,
        IEnumerable<Specification> specifications,
        IEnumerable<ProductTag> tags) =>
        new(
            id: id,
            brandId: brandId,
            categoryId: categoryId,
            name: name,
            description: description,
            thumbnail: thumbnail,
            stock: stock,
            price: price,
            discount: discount,
            images: images,
            specifications: specifications,
            tags: tags);

    public void Update(
        BrandId brandId,
        CategoryId categoryId,
        string name,
        string description,
        string thumbnail,
        int stock,
        Money price,
        Money discount,
        IEnumerable<string>? images,
        IEnumerable<(string Name, string Value)> specifications,
        IEnumerable<Guid> tags,
        bool isDeleted = false)
    {
        BrandId = brandId;
        CategoryId = categoryId;
        Name = name;
        NormalizedName = name.ToUpper();
        Description = description;
        Thumbnail = thumbnail;
        StockQuantity = stock;
        Price = price;
        Discount = discount;

        UpdateProductImages(images);
        UpdateProductTags(tags);
        UpdateProductSpecifications(specifications);

        IsDeleted = isDeleted;
        UpdatedAt = DateTime.UtcNow;
    }

    private void UpdateProductImages(IEnumerable<string>? images)
    {
        var inputImagesSet = new HashSet<string>(images ?? []);

        var imagesToRemove = _images
            .Where(x => !inputImagesSet.Contains(x.ImageUrl))
            .ToList();

        if(imagesToRemove.Count != 0)
        {
            foreach(ProductImage image in imagesToRemove)
            {
                _images.Remove(image);
            }

            var urlsToDelete = imagesToRemove.Select(x => x.ImageUrl).ToList();

            RaiseDomainEvent(new ProductImagesRemovedEvent(Id, urlsToDelete));
        }

        var existingImageUrls = new HashSet<string>(_images.Select(x => x.ImageUrl));

        foreach(var imageUrl in inputImagesSet)
        {
            if(!existingImageUrls.Contains(imageUrl))
            {
                _images.Add(ProductImage.Create(ProductImageId.Create(), imageUrl));
            }
        }
    }

    private void UpdateProductTags(IEnumerable<Guid> tags)
    {
        var inputTagsSet = new HashSet<Guid>(tags);

        _tags.RemoveAll(x => !inputTagsSet.Contains(x.TagId.Value));

        var existingTagIds = new HashSet<Guid>(_tags.Select(x => x.TagId.Value));

        foreach(Guid tagId in inputTagsSet)
        {
            if(!existingTagIds.Contains(tagId))
            {
                _tags.Add(ProductTag.Create(TagId.Create(tagId)));
            }
        }
    }

    private void UpdateProductSpecifications(IEnumerable<(string Name, string Value)> specifications)
    {
        var newSpecsDict = specifications
            .DistinctBy(x => x.Name)
            .ToDictionary(x => x.Name, x => x.Value);

        var specsToRemove = _specifications
            .Where(existing => !newSpecsDict.ContainsKey(existing.Name))
            .ToList();

        foreach(Specification spec in specsToRemove)
        {
            _specifications.Remove(spec);
        }

        var currentSpecsDict = _specifications.ToDictionary(x => x.Name);

        foreach((var name, var value) in newSpecsDict)
        {
            if(currentSpecsDict.TryGetValue(name, out Specification? existingSpec))
            {
                if(existingSpec.Value != value)
                {
                    existingSpec.UpdateValue(value);
                }
            }
            else
            {
                _specifications.Add(Specification.Create(
                    SpecificationId.Create(),
                    name,
                    value));
            }
        }
    }
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
