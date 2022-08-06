using System.Net;
using DictionaryApiNet.Model;

namespace DictionaryApiNet
{
    public static class DictionaryApi
    {
        private static string GetRequestAddress(string RequestedWord)
        {
            return $"https://api.dictionaryapi.dev/api/v2/entries/en/{RequestedWord}";
        }

        public static WordDefinition GetWordDefinition(string Word)
        {
            if (string.IsNullOrEmpty(Word))
            {
                throw new ArgumentException($"'{nameof(Word)}' cannot be null or empty.", nameof(Word));
            }

            return GetWordDefinitionAsync(Word).Result;
        }

        public static async Task<WordDefinition> GetWordDefinitionAsync(string Word)
        {
            if (string.IsNullOrEmpty(Word))
            {
                throw new ArgumentException($"'{nameof(Word)}' cannot be null or empty.", nameof(Word));
            }

            using var client = new HttpClient();
            var data = await client.GetStringAsync(GetRequestAddress(Word));
            
            return WordDefinition.FromJson(data);
        }
    }
}