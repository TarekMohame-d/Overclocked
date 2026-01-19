namespace Overclocked.Integration.Tests.Shared;

[CollectionDefinition(nameof(IntegrationTestCollection))]
public class IntegrationTestCollection : ICollectionFixture<IntegrationTestWebAppFactory>;
