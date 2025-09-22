namespace Application.Abstraction.Messaging;

public interface ICommand<TResponse> : IRequest;

public interface ITransactionalCommand<TResponse> : ICommand<TResponse>;
