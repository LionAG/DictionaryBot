using DictionaryBot.Components;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DictionaryBot
{
    internal class Core
    {
        private IConfiguration Configuration { get; init; }
        private IServiceProvider Services { get; init; }

        private string AppConfigurationFile => "appsettings.json";

        private DiscordSocketConfig DiscordConfig => new()
        {
            GatewayIntents = GatewayIntents.AllUnprivileged & ~GatewayIntents.GuildScheduledEvents & ~GatewayIntents.GuildInvites
        };

        public Core()
        {
            this.Configuration = new ConfigurationBuilder().AddJsonFile(AppConfigurationFile, optional: false).Build();

            this.Services = new ServiceCollection().AddSingleton(Configuration)
                                                          .AddSingleton(DiscordConfig)
                                                          .AddSingleton<DiscordSocketClient>()
                                                          .AddSingleton(s => new InteractionService(s.GetRequiredService<DiscordSocketClient>()))
                                                          .AddSingleton<InteractionHandler>()
                                                          .BuildServiceProvider();
        }

        public async Task RunAsync()
        {
            var token = Configuration.GetValue<string>(Program.IsDebugConfiguration() ? "token_debug" : "token_release");
            var discordClient = Services.GetRequiredService<DiscordSocketClient>();

            discordClient.Log += DiscordClient_Log;

            await Services.GetRequiredService<InteractionHandler>().InitializeAsync();
            await Connect(token);
            await Task.Delay(Timeout.Infinite);
        }

        private async Task Connect(string ConnectionToken)
        {
            var client = Services.GetRequiredService<DiscordSocketClient>();

            await client.LoginAsync(TokenType.Bot, ConnectionToken);
            await client.StartAsync();
        }

        private async Task DiscordClient_Log(LogMessage arg) => Logger.Log(arg);
    }
}
