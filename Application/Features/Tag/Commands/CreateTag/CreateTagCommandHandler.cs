using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Features.Tag.Commands.CreateTag.Notifications;
using Application.Features.Tag.Mapping;
using Domain.Repositories;
using System.Net;

namespace Application.Features.Tag.Commands.CreateTag;

public class CreateTagCommandHandler : ICommandHandler<CreateTagCommand, Result>
{
    private readonly ITagRepository _tagRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public CreateTagCommandHandler(
        ITagRepository tagRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _tagRepository = tagRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<Result> Handle(CreateTagCommand command, CancellationToken cancellationToken)
    {
        var tag = command.ToEntity();

        await _tagRepository.AddAsync(tag, cancellationToken);

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _mediator.Publish(new TagCreatedNotification(), cancellationToken);

        return Result.Success(HttpStatusCode.Created);
    }
}
