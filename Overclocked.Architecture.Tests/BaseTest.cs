using System.Reflection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Overclocked.Application;
using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Architecture.Tests;

public abstract class BaseTest
{
    protected static readonly Assembly DomainAssembly = typeof(Entity<>).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(DependencyInjection).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(Infrastructure.DependencyInjection).Assembly;
    protected static readonly Assembly PresentationAssembly = typeof(Program).Assembly;
}
