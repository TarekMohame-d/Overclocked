using System.Net;
using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Features.Category.Commands.CreateCategory.Notifications;
using Application.Features.Category.Mapping;
using Domain.Repositories;

namespace Application.Features.Category.Commands.CreateCategory;

public class CreateCategoryCommandHandler : ICommandHandler<CreateCategoryCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMediator _mediator;

    public CreateCategoryCommandHandler(
        IUnitOfWork unitOfWork,
        ICategoryRepository categoryRepository,
        IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _categoryRepository = categoryRepository;
        _mediator = mediator;
    }

    public async Task<Result> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = command.ToEntity();

        await _categoryRepository.AddAsync(category, cancellationToken);

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _mediator.Publish(new CategoryCreatedNotification(category.Id), cancellationToken);

        return Result.Success(HttpStatusCode.Created);
    }
}
