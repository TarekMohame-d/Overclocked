using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.ProductAggregate.ValueObjects;

public record ProductRating : IValueObject
{
    public int TotalScore { get; private set; }
    public int ReviewCount { get; private set; }
    public double AverageRating => ReviewCount == 0
    ? 0
    : Math.Round((double)TotalScore / ReviewCount, 1);

    private ProductRating(int totalScore, int reviewCount)
    {
        TotalScore = totalScore;
        ReviewCount = reviewCount;
    }

    public static ProductRating Create(int totalScore, int reviewCount)
        => new(totalScore, reviewCount);

    public static ProductRating Zero => new(0, 0);
}
