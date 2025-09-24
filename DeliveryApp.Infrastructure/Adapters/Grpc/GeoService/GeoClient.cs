using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Core.Ports;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Primitives;

namespace DeliveryApp.Infrastructure.Adapters.Grpc.GeoService
{
    public class GeoClient : IGeoClient, IDisposable
    {
        private readonly MethodConfig _methodConfig;
        private readonly SocketsHttpHandler _socksHttpHandler;
        private readonly string _url;
        private readonly GrpcChannel _channel;
        private readonly GeoApp.Api.Geo.GeoClient _client;

        public GeoClient(string url)
        {
            _url = url;

            _socksHttpHandler = new SocketsHttpHandler()
            {
                PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                EnableMultipleHttp2Connections = true
            };

            _methodConfig = new MethodConfig()
            {
                Names = { MethodName.Default },
                RetryPolicy = new RetryPolicy
                {
                    MaxAttempts = 5,
                    InitialBackoff = TimeSpan.FromSeconds(1),
                    MaxBackoff = TimeSpan.FromSeconds(5),
                    BackoffMultiplier = 1.5,
                    RetryableStatusCodes = { StatusCode.Unavailable }
                }
            };

            _channel= GrpcChannel.ForAddress(_url, new GrpcChannelOptions
            {
                HttpHandler = _socksHttpHandler,
                ServiceConfig = new ServiceConfig { MethodConfigs = { _methodConfig } }
            });
            _client = new GeoApp.Api.Geo.GeoClient(_channel);
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }

        public async Task<Result<Location, Error>> GetLocationAsync(string address, CancellationToken cancellationToken)
        {
            var reply = await _client.GetGeolocationAsync(new GeoApp.Api.GetGeolocationRequest { Street = address }, null, null, cancellationToken);
                
            var resultLocation = Location.Create(reply.Location.X, reply.Location.Y);
            if (!resultLocation.IsSuccess)
                return resultLocation.Error;

            var result = resultLocation.Value;

            return result;
        }
    }
}
