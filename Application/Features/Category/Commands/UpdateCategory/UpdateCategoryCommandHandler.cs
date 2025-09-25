using System.Net;
using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Features.Category.Commands.UpdateCategory.Notifications;
using Application.Features.Category.Mapping;
using Domain.Repositories;

namespace Application.Features.Category.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : ICommandHandler<UpdateCategoryWithIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMediator _mediator;

    public UpdateCategoryCommandHandler(
        IUnitOfWork unitOfWork,
        ICategoryRepository categoryRepository,
        IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _categoryRepository = categoryRepository;
        _mediator = mediator;
    }

    public async Task<Result> Handle(UpdateCategoryWithIdCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync([request.Id], cancellationToken);

        if (category is null)
            return Result.Failure(Errors.CategoryNotFound, HttpStatusCode.NotFound);

        if (category.Name != request.Name)
        {
            bool exist = await _categoryRepository
                .AnyAsync(x => x.NormalizedName == request.Name.ToUpper(), cancellationToken);

            if (exist)
                return Result.Failure(Errors.CategoryNameAlreadyExists, HttpStatusCode.Conflict);
        }

        category.UpdateFrom(request);

        _categoryRepository.Update(category);

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _mediator.Publish(new CategoryUpdatedNotification(request.Id), cancellationToken);

        return Result.Success();
    }
}
