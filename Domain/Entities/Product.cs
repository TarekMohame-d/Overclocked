using Domain.Entities.Common;

namespace Domain.Entities;

public class Product : Entity
{
    public required Guid CategoryId { get; set; }
    public required Guid BrandId { get; set; }
    public required string Name { get; set; }
    public string NormalizedName { get; } = string.Empty;
    public required string Thumbnail { get; set; }
    public required string Description { get; set; }
    public required decimal Price { get; set; }
    public decimal Discount { get; set; } = 0m;
    public double Rating { get; private set; }
    public int ReviewCount { get; private set; }
    public required int StockQuantity { get; set; }
    public bool IsDeleted { get; set; } = false;

    // Navigation Properties
    public Brand? Brand { get; set; }
    public Category? Category { get; set; }
    public ICollection<CartItem> CartItems { get; set; } = [];
    public ICollection<WishlistItem> WishlistItems { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<OrderItem> OrderItems { get; set; } = [];
    public ICollection<TagProduct> TagProducts { get; set; } = [];
    public ICollection<ProductImage> ProductImages { get; set; } = [];
    public ICollection<Specification> Specifications { get; set; } = [];

    public void CalculateRating(int newReviewRating)
    {
        var newRating = ((Rating * ReviewCount) + newReviewRating) / (ReviewCount + 1);

        Rating = Math.Clamp(newRating, 0, 5);

        ReviewCount++;
    }

    public void RemoveRating(int oldReviewRating)
    {
        if(ReviewCount <= 1)
        {
            Rating = 0;
            ReviewCount = 0;
            return;
        }

        var currentTotalScore = Rating * ReviewCount;
        var newRating = (currentTotalScore - oldReviewRating) / (ReviewCount - 1);

        Rating = Math.Clamp(newRating, 0, 5);
        ReviewCount--;
    }

    public void UpdateRating(int oldReviewRating, int newReviewRating)
    {
        if(ReviewCount == 0)
            return;

        if(oldReviewRating == newReviewRating)
            return;

        var currentTotalScore = Rating * ReviewCount;
        var newRating = (currentTotalScore - oldReviewRating + newReviewRating) / ReviewCount;

        Rating = Math.Clamp(newRating, 0, 5);
    }
}
