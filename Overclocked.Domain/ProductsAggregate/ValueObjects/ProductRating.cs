using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.ProductsAggregate.ValueObjects;

public record ProductRating : IValueObject
{
    public double Rating { get; private set; }
    public int ReviewCount { get; private set; }

    private ProductRating(double rating, int reviewCount)
    {
        Rating = rating;
        ReviewCount = reviewCount;
    }

    public static ProductRating Create(double rating, int reviewCount)
        => new(rating, reviewCount);

    public static ProductRating Zero => new(0, 0);
}
