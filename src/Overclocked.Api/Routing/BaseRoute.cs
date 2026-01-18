namespace Overclocked.Api.Routing;

public abstract class BaseRoute
{
    protected const string Base = $"{Root}/{Version}";
    private const string Root = "api";
    private const string Version = "v1";
}
