using System.Net;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Features.Tag.Commands.DeleteTag.Notifications;

namespace Application.Features.Tag.Commands.DeleteTag;

public class DeleteTagCommandHandler : ICommandHandler<DeleteTagCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITagRepository _TagRepository;
    private readonly IMediator _mediator;

    public DeleteTagCommandHandler(
        IUnitOfWork unitOfWork,
        ITagRepository TagRepository,
        IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _TagRepository = TagRepository;
        _mediator = mediator;
    }

    public async Task<Result> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var Tag = await _TagRepository.GetByIdAsync([request.Id], cancellationToken);

        if (Tag is null)
            return Result.Failure(Errors.TagNotFound, HttpStatusCode.NotFound);

        _TagRepository.Delete(Tag);

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _mediator.Publish(new TagDeletedNotification(Tag.Id), cancellationToken);

        return Result.Success();
    }
}
