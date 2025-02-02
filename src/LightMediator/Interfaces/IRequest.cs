

namespace LightMediator;

public interface IRequest : IBaseRequest
{
}
public interface IRequest<out TResponse> : IBaseRequest
{
}
public interface IBaseRequest { }