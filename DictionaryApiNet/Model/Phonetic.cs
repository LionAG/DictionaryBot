using Newtonsoft.Json;

namespace DictionaryApiNet.Model
{
    public class Phonetic
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("audio", NullValueHandling = NullValueHandling.Ignore)]
        public string Audio { get; set; }
    }
}
