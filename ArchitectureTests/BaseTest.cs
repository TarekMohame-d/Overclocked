using System.Reflection;
using Application;
using Domain.Entities.Common;
using Infrastructure;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace ArchitectureTests;

public abstract class BaseTest
{
    protected static readonly Assembly DomainAssembly = typeof(BaseEntity).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(ApplicationServicesRegistration).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(InfrastructureServicesRegistration).Assembly;
    protected static readonly Assembly PresentationAssembly = typeof(Program).Assembly;
}
