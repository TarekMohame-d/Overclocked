using Overclocked.Application.Features.ReviewUseCases.DTOs.Responses;
using Overclocked.Domain.ReviewAggregate;

namespace Overclocked.Application.Features.ReviewUseCases.Mapping;

public static class ReviewMapper
{
    public static IEnumerable<ReviewResponse> ToDto(this IEnumerable<Review> entities) =>
        entities.Select(x => new ReviewResponse
        {
            Id = x.Id.Value,
            UserId = x.UserId.Value,
            UserEmail = x.User!.Email,
            UserName = $"{x.User!.FirstName} {x.User.LastName}",
            Comment = x.Comment,
            Rating = x.Rating,
            CreatedAt = x.UpdatedAt,

            Reply = x.ReviewReply is not null
                ? new ReviewReplyResponse
                {
                    Id = x.ReviewReply.Id.Value,
                    Reply = x.ReviewReply.Reply,
                    CreatedAt = x.ReviewReply.UpdatedAt,
                }
                : null,
        });
}
