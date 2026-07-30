using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Portfolio.Infrastructure.Services;
using Xunit;

namespace Portfolio.Api.Tests;

public class HeartbeatServiceTests
{
    private class TestHttpMessageHandler : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }
        public HttpResponseMessage ResponseToReturn { get; set; } = new(HttpStatusCode.OK);
        public bool ThrowException { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            if (ThrowException)
            {
                throw new HttpRequestException("Network failure simulated");
            }
            return Task.FromResult(ResponseToReturn);
        }
    }

    private class TestHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpMessageHandler _handler;

        public TestHttpClientFactory(HttpMessageHandler handler)
        {
            _handler = handler;
        }

        public HttpClient CreateClient(string name)
        {
            return new HttpClient(_handler, disposeHandler: false);
        }
    }

    [Fact]
    public async Task PerformHeartbeatAsync_NoSelfUrl_CompletesWithoutException()
    {
        // Arrange
        var inMemoryConfig = new Dictionary<string, string?>
        {
            ["Heartbeat:Enabled"] = "true",
            ["Heartbeat:IntervalMinutes"] = "10",
            ["Heartbeat:SelfUrl"] = ""
        };

        var config = new ConfigurationBuilder().AddInMemoryCollection(inMemoryConfig).Build();
        var handler = new TestHttpMessageHandler();
        var clientFactory = new TestHttpClientFactory(handler);
        var logger = NullLogger<HeartbeatService>.Instance;

        var service = new HeartbeatService(clientFactory, config, logger);

        // Act & Assert
        var act = async () => await service.PerformHeartbeatAsync();
        await act.Should().NotThrowAsync();
        handler.LastRequest.Should().BeNull();
    }

    [Fact]
    public async Task PerformHeartbeatAsync_WithSelfUrl_SendsGetRequestToHealthEndpoint()
    {
        // Arrange
        var inMemoryConfig = new Dictionary<string, string?>
        {
            ["Heartbeat:Enabled"] = "true",
            ["Heartbeat:IntervalMinutes"] = "10",
            ["Heartbeat:SelfUrl"] = "https://api.example.com"
        };

        var config = new ConfigurationBuilder().AddInMemoryCollection(inMemoryConfig).Build();
        var handler = new TestHttpMessageHandler();
        var clientFactory = new TestHttpClientFactory(handler);
        var logger = NullLogger<HeartbeatService>.Instance;

        var service = new HeartbeatService(clientFactory, config, logger);

        // Act
        await service.PerformHeartbeatAsync();

        // Assert
        handler.LastRequest.Should().NotBeNull();
        handler.LastRequest!.Method.Should().Be(HttpMethod.Get);
        handler.LastRequest.RequestUri.Should().Be(new Uri("https://api.example.com/health"));
    }

    [Fact]
    public async Task PerformHeartbeatAsync_WhenNetworkFails_HandlesExceptionGracefully()
    {
        // Arrange
        var inMemoryConfig = new Dictionary<string, string?>
        {
            ["Heartbeat:Enabled"] = "true",
            ["Heartbeat:IntervalMinutes"] = "10",
            ["Heartbeat:SelfUrl"] = "https://api.example.com"
        };

        var config = new ConfigurationBuilder().AddInMemoryCollection(inMemoryConfig).Build();
        var handler = new TestHttpMessageHandler { ThrowException = true };
        var clientFactory = new TestHttpClientFactory(handler);
        var logger = NullLogger<HeartbeatService>.Instance;

        var service = new HeartbeatService(clientFactory, config, logger);

        // Act & Assert
        var act = async () => await service.PerformHeartbeatAsync();
        await act.Should().NotThrowAsync();
    }
}
