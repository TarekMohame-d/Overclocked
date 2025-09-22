using Application.Abstraction.Messaging;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Brand.Commands.UpdateBrand.Notifications;

public record BrandUpdatedNotification(
    Guid id,
    IFormFile? image = default,
    string? imageUrl = default) : INotification;
