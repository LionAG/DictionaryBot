using DictionaryBot.Components;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DictionaryBot.Modules
{
    public class Status : InteractionModuleBase<SocketInteractionContext>
    {
        public InteractionService? InteractionService { get; set; }

        public InteractionHandler? InteractionHandler { get; set; }

        [Group("status", "Get bot status.")]
        public class StatusCommandGroup : InteractionModuleBase<SocketInteractionContext>
        {
            public IServiceProvider Services { get; set; }

            public StatusCommandGroup(IServiceProvider Services)
            {
                this.Services = Services;
            }

            [SlashCommand("version", "Get current version")]
            public async Task StatusVersionCommandProcess() =>
                await RespondAsync($"Bot version: {Assembly.GetExecutingAssembly().GetName()?.Version?.ToString(3)}");

            [SlashCommand("latency", "Get bot response time.")]
            public async Task StatusLatencyCommandProcess()
                => await RespondAsync($"Current latency: {Context.Client.Latency} ms.");
        }
    }
}
