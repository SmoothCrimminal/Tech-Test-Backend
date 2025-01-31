﻿using Microsoft.Extensions.Options;
using TechTestBackend.Configurations;
using TechTestBackend.Constants;
using TechTestBackend.DelegatingHandlers;

namespace TechTestBackend.Extensions
{
    public static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddHttpClients(this IServiceCollection services)
        {
            services.AddHttpClient(ApiConstants.SpotifyApiClient, (sp, client) =>
            {
                var spotifyApiConfiguration = sp.GetRequiredService<IOptions<SpotifyApiConfiguration>>().Value;
                client.BaseAddress = new Uri(spotifyApiConfiguration.BaseAddress);
            })
            .AddHttpMessageHandler<SpotifyAuthenticationHandler>();

            return services;
        }

        internal static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SpotifyApiConfiguration>(configuration.GetSection(ApiConstants.SpotifyApiClient));

            return services;
        }
    }
}
