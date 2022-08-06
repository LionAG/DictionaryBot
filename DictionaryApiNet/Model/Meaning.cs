using Newtonsoft.Json;

namespace DictionaryApiNet.Model
{
    public class Meaning
    {
        [JsonProperty("partOfSpeech")]
        public string PartOfSpeech { get; set; }

        [JsonProperty("definitions")]
        public Definition[] Definitions { get; set; }
    }
}
