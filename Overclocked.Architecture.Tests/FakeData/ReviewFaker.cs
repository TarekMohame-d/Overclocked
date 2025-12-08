// using Bogus;
// using Domain.Entities;

// namespace ArchitectureTests.FakeData;

// public class ReviewFaker : Faker<Review>
// {
//     public ReviewFaker()
//     {
//         RuleFor(r => r.Id, f => Guid.CreateVersion7());
//         RuleFor(r => r.UserId, f => Guid.CreateVersion7());
//         RuleFor(r => r.ProductId, f => Guid.CreateVersion7());
//         RuleFor(r => r.Comment, f => "Comment");
//         RuleFor(r => r.Rating, f => f.Random.Int(0, 5));
//         RuleFor(r => r.CreatedAt, f => DateTime.UtcNow);
//         RuleFor(r => r.UpdatedAt, f => DateTime.UtcNow);
//     }
// }
