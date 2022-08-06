using DictionaryApiNet;
using DictionaryBot.Components;
using DictionaryBot.Components.Extensions;
using Discord;
using Discord.Interactions;

namespace DictionaryBot.Modules
{
    public class Lookup : InteractionModuleBase<SocketInteractionContext>
    {
        public InteractionService? InteractionService { get; set; }

        public InteractionHandler? InteractionHandler { get; set; }

        public static Embed WordNotFoundEmbed => new EmbedBuilder().WithDescription("The specified word was not found. Check for any misspelling and try again.")
                                                                   .WithCurrentTimestamp()
                                                                   .WithColor(Color.Red)
                                                                   .Build();

        [SlashCommand("lookup", "Lookup words in the dictionary")]
        public async Task LookupCommandProcess(string Word, [Summary("include-pronunciation", "Set this parameter to true to include pronunciation audio.")] bool phonetics = false)
        {
            try
            {
                string description = String.Empty;
                var wordData = await DictionaryApi.GetWordDefinitionAsync(Word);

                description += $"**Origin**: {wordData.Origin}\n\n";

                wordData.Meanings.ToList().ForEach((m) =>
                {
                    description += $"***As {m.PartOfSpeech}:***\n";

                    for (int x = 0; x < m.Definitions.Length; x++)
                    {
                        description += $"{x + 1}. {m.Definitions[x].DefinitionText}\n";
                    }

                    description += "\n";
                });


                if (phonetics && !string.IsNullOrEmpty(wordData.Phonetics[0].Audio))
                {
                    description += "*See the attached file for the pronounciation audio.*";
                }

                var embed = new EmbedBuilder().WithTitle($"{Word.ToUpper()} /{wordData.Phonetics[0].Text}/")
                                              .WithTimestamp(DateTime.Now)
                                              .WithColor(Color.Teal)
                                              .WithDescription(description);

                await RespondAsync(embed: embed.Build());

                // Try to post the pronounciation audio

                if (phonetics && !string.IsNullOrEmpty(wordData.Phonetics[0].Audio))
                {
                    var uri = new Uri(wordData.Phonetics[0].Audio);
                    var filename = $"{wordData.Word}_pronounciation.mp3";

                    using var client = new HttpClient();

                    await client.DownloadFileTaskAsync(uri, filename);
                    await Context.Channel.SendFileAsync(filename);

                    File.Delete(filename);
                }
            }
            catch
            {
                await RespondAsync(embed: WordNotFoundEmbed);
            }
        }

        [Group("get", "Get information about the word.")]
        public class GetCommandGroup : InteractionModuleBase<SocketInteractionContext>
        {
            [SlashCommand("origin", "Gets origin of the word.")]
            public async Task GetOriginCommandProcess(string Word)
            {
                try
                {
                    var wordData = await DictionaryApi.GetWordDefinitionAsync(Word);

                    var embed = new EmbedBuilder().WithTitle($"{Word.ToUpper()}")
                                                  .WithCurrentTimestamp()
                                                  .WithColor(Color.Orange)
                                                  .WithDescription(wordData.Origin ?? "Unavailable.");

                    await RespondAsync(embed: embed.Build());
                }
                catch
                {
                    await RespondAsync(embed: WordNotFoundEmbed);
                }
            }

            [SlashCommand("meaning", "Gets meaning of the word.")]
            public async Task GetMeaningCommandProcess(string Word)
            {
                try
                {
                    var description = string.Empty;
                    var wordData = await DictionaryApi.GetWordDefinitionAsync(Word);

                    wordData.Meanings.ToList().ForEach((m) =>
                    {
                        description += $"***As {m.PartOfSpeech}:***\n";

                        for (int x = 0; x < m.Definitions.Length; x++)
                        {
                            description += $"{x + 1}. {m.Definitions[x].DefinitionText}\n";
                        }

                        description += "\n";
                    });

                    var embed = new EmbedBuilder().WithTitle($"{Word.ToUpper()}")
                                                  .WithCurrentTimestamp()
                                                  .WithColor(Color.Green)
                                                  .WithDescription(description);

                    await RespondAsync(embed: embed.Build());
                }
                catch
                {
                    await RespondAsync(embed: WordNotFoundEmbed);
                }
            }

            [SlashCommand("phonetics", "Gets phonetics of the word.")]
            public async Task GetPhoneticsCommandProcess(string Word)
            {
                try
                {
                    var description = string.Empty;
                    var wordData = await DictionaryApi.GetWordDefinitionAsync(Word);

                    for (int x = 0; x < wordData.Phonetics.Length; x++)
                        description += $"{x + 1}. {wordData.Phonetics[x].Text}\n";

                    var embed = new EmbedBuilder().WithTitle($"{Word.ToUpper()} - Phonetics")
                                                  .WithCurrentTimestamp()
                                                  .WithColor(Color.Blue)
                                                  .WithDescription(description);

                    await RespondAsync(embed: embed.Build());
                }
                catch
                {
                    await RespondAsync(embed: WordNotFoundEmbed);
                }
            }

            [SlashCommand("pronunciation", "Gets pronunciation of the word.")]
            public async Task GetPronunciationCommandProcess(string Word)
            {
                try
                {
                    var description = string.Empty;
                    var wordData = await DictionaryApi.GetWordDefinitionAsync(Word);

                    // Download and post the pronunciation audio file.

                    var phonetic = wordData.Phonetics.Where(f => string.IsNullOrEmpty(f.Audio) == false).FirstOrDefault();

                    if (phonetic == null)
                    {
                        await RespondAsync(embed: WordNotFoundEmbed);
                    }
                    else
                    {
                        await RespondAsync(embed: new EmbedBuilder().WithDescription($"See below for the pronunciation of the word ***{wordData.Word}***:")
                                                                    .WithColor(Color.Gold)
                                                                    .Build());

                        using var client = new HttpClient();

                        var uri = new Uri(phonetic.Audio);
                        var filename = $"{wordData.Word}_pronunciation.mp3";

                        await client.DownloadFileTaskAsync(uri, filename);
                        await Context.Channel.SendFileAsync(filename);

                        File.Delete(filename);
                    }
                }
                catch
                {
                    await RespondAsync(embed: WordNotFoundEmbed);
                }
            }
        }
    }
}
