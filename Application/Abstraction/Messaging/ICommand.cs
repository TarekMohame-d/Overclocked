namespace Application.Abstraction.Messaging;

public interface ICommand<TResponse> : IRequest, IValidationalRequest;

public interface ITransactionalCommand<TResponse> : ICommand<TResponse>;
