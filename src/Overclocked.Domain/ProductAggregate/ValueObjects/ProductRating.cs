using System.ComponentModel.DataAnnotations.Schema;
using Overclocked.SharedKernel;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.ProductAggregate.ValueObjects;

public record ProductRating : IValueObject
{
    public int TotalScore { get; private set; }
    public int ReviewCount { get; private set; }

    [NotMapped]
    public double AverageRating
    {
        get
        {
            if (ReviewCount == 0)
                return 0;

            var avg = Math.Round((double)TotalScore / ReviewCount, 1);
            return Math.Clamp(avg, 0.0, 5.0);
        }
    }

    private ProductRating(int totalScore, int reviewCount)
    {
        TotalScore = totalScore;
        ReviewCount = reviewCount;
    }

    public static Result<ProductRating> Create(int totalScore, int reviewCount)
    {
        if (totalScore < 0)
            return Result.Failure<ProductRating>(ProductErrors.InvalidRatingTotalScore);

        if (reviewCount < 0)
            return Result.Failure<ProductRating>(ProductErrors.InvalidRatingReviewCount);

        return Result.Success(new ProductRating(totalScore, reviewCount));
    }

    public static ProductRating Zero => new(0, 0);
}
