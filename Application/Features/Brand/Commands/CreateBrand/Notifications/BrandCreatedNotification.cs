using Application.Abstraction.Messaging;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Brand.Commands.CreateBrand.Notifications;

public record BrandCreatedNotification(Guid id, IFormFile image) : INotification;
