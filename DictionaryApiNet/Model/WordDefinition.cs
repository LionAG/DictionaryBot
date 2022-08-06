using Newtonsoft.Json;

namespace DictionaryApiNet.Model
{
    public class WordDefinition
    {
        [JsonProperty("word")]
        public string Word { get; set; }

        [JsonProperty("phonetic")]
        public string Phonetic { get; set; }

        [JsonProperty("phonetics")]
        public Phonetic[] Phonetics { get; set; }

        [JsonProperty("origin", NullValueHandling = NullValueHandling.Ignore)]
        public string Origin { get; set; }

        [JsonProperty("meanings")]
        public Meaning[] Meanings { get; set; }

        public static WordDefinition FromJson(string json) => JsonConvert.DeserializeObject<WordDefinition[]>(json)[0];
    }
}
