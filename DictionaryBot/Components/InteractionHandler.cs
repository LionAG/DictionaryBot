using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DictionaryBot.Components
{
    public class InteractionHandler
    {
        private InteractionService InteractionService { get; init; }
        private IServiceProvider Services { get; init; }
        private IConfiguration Configuration { get; init; }

        public InteractionHandler(InteractionService interactionService, IServiceProvider services, IConfiguration configuration)
        {
            this.InteractionService = interactionService;
            this.Services = services;
            this.Configuration = configuration;
        }

        public async Task InitializeAsync()
        {
            var discordClient = Services.GetRequiredService<DiscordSocketClient>();

            // Hook events

            discordClient.Ready += DiscordClient_ReadyAsync;
            discordClient.InteractionCreated += DiscordClient_InteractionCreatedAsync;

            // Add modules

            await this.InteractionService.AddModulesAsync(assembly: Assembly.GetExecutingAssembly(), services: Services)
                .ContinueWith(i => Logger.Log(new LogData(nameof(InteractionHandler), "Initialization completed.", Enums.LogImportance.Info)));
        }

        private async Task DiscordClient_InteractionCreatedAsync(SocketInteraction interaction)
        {
            try
            {
                var client = Services.GetRequiredService<DiscordSocketClient>();
                var context = new SocketInteractionContext(client, interaction);
                var execResult = await this.InteractionService.ExecuteCommandAsync(context, Services);

                if (execResult.IsSuccess == false)
                {
                    Logger.Log(new LogData(nameof(InteractionHandler), $"Failed to process interaction: {execResult.ErrorReason}", Enums.LogImportance.Warning));
                    await interaction.RespondAsync($"Cannot process the interaction! ({execResult.Error}: {execResult.ErrorReason})");
                }
                else
                {
                    Logger.Log(new LogData(nameof(InteractionHandler), $"Successfully processed interaction: {execResult.ErrorReason}", Enums.LogImportance.Info));
                }
            }
            catch
            {
                if (interaction.Type is InteractionType.ApplicationCommand)
                {
                    await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
                    await interaction.Channel.SendMessageAsync("Cannot process the command!");
                }
            }
        }

        private async Task DiscordClient_ReadyAsync()
        {
            // Once ready register commands.

            if (Program.IsDebugConfiguration())
            {
                ulong testGuildId = Configuration.GetValue<ulong>("test_guild");

                await this.InteractionService.RegisterCommandsToGuildAsync(testGuildId, deleteMissing: true);

                Logger.Log(new LogData(nameof(InteractionHandler), "Commands has been registered to the test guild.", Enums.LogImportance.Info));
            }
            else
            {
                await this.InteractionService.RegisterCommandsGloballyAsync(deleteMissing: true);

                Logger.Log(new LogData(nameof(InteractionHandler),
                                       "Commands has been registered globally. This change may take up to an hour to be reflected in all guilds.",
                                       Enums.LogImportance.Info));
            }
        }
    }
}
