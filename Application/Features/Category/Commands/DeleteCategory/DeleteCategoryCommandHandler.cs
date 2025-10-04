using System.Net;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Features.Category.Commands.DeleteCategory.Notifications;

namespace Application.Features.Category.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : ICommandHandler<DeleteCategoryCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMediator _mediator;

    public DeleteCategoryCommandHandler(
        IUnitOfWork unitOfWork,
        ICategoryRepository categoryRepository,
        IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _categoryRepository = categoryRepository;
        _mediator = mediator;
    }

    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync([request.Id], cancellationToken);

        if (category is null)
            return Result.Failure(Errors.CategoryNotFound, HttpStatusCode.NotFound);

        _categoryRepository.Delete(category);

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _mediator.Publish(new CategoryDeletedNotification(category.Id, category.Image), cancellationToken);

        return Result.Success();
    }
}

