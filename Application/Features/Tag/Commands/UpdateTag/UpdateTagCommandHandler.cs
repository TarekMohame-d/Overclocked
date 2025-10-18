using System.Net;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Features.Tag.Commands.UpdateTag.Notifications;
using Application.Features.Tag.Mapping;

namespace Application.Features.Tag.Commands.UpdateTag;

public class UpdateTagCommandHandler : ICommandHandler<UpdateTagWithIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITagRepository _tagRepository;
    private readonly IMediator _mediator;

    public UpdateTagCommandHandler(
        IUnitOfWork unitOfWork,
        ITagRepository tagRepository,
        IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _tagRepository = tagRepository;
        _mediator = mediator;
    }

    public async Task<Result> Handle(UpdateTagWithIdCommand command, CancellationToken cancellationToken)
    {
        var tag = await _tagRepository.GetByIdAsync([command.Id], cancellationToken);

        if (tag is null)
            return Result.Failure(Errors.TagNotFound, HttpStatusCode.NotFound);

        if (tag.Name != command.Name)
        {
            bool exist = await _tagRepository
                .AnyAsync(x => x.NormalizedName == command.Name.ToUpper(), cancellationToken);

            if (exist)
                return Result.Failure(Errors.TagNameAlreadyExists, HttpStatusCode.Conflict);
        }

        tag.UpdateFrom(command);

        _tagRepository.Update(tag);

        await _mediator.Publish(new TagUpdatedNotification(command.Id), cancellationToken);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
