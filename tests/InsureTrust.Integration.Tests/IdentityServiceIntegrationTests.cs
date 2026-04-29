using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using System.Net;

namespace InsureTrust.Integration.Tests
{
    // This test actually spins up the Identity Service in memory to verify it starts correctly
    public class IdentityServiceIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public IdentityServiceIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task IdentityService_IsHealthy_AndReachable()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            // We hit an endpoint that we know exists. 
            // Even if it returns 401, it proves the service logic is loading correctly.
            var response = await client.GetAsync("/api/auth/users");

            // Assert
            // 401 means the service is alive but requires a token. 
            // 404 or 500 would mean a routing or startup error.
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
