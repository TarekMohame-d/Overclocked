using Bogus;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.ReviewAggregate;
using Overclocked.Domain.ReviewAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Architecture.Tests.FakeData;

public class ReviewFaker : Faker<Review>
{
    public ReviewFaker(Guid userId, Guid productId)
    {
        CustomInstantiator(f =>
            Review.Create(
                ReviewId.Create(),
                UserId.Create(userId),
                ProductId.Create(productId),
                $"Comment",
                f.Random.Int(1, 5)
            ));
    }
}
