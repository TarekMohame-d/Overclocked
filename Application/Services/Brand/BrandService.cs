using Application.Abstraction.DomainServices;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;

namespace Application.Services.Brand;

public sealed partial class BrandService(
    IBrandRepository brandRepository,
    IUnitOfWork unitOfWork,
    IEventDispatcher eventDispatcher)
    : IBrandService;
