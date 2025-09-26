namespace Application.Abstraction.Messaging;

public interface ICommand<TResponse> : IRequest, IValidation;

public interface ITransactionalCommand<TResponse> : ICommand<TResponse>;
