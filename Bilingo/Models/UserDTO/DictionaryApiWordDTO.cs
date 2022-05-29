using System.Text.Json.Serialization;

namespace Bilingo.Models.UserDTO
{
    public class DictionaryApiWordDTO
    {
        [JsonPropertyName("word")] public string WordName { get; set; }
        [JsonPropertyName("meanings")] public IList<DictionaryApiMeaning> Meanings { get; set; }
    }

    public class DictionaryApiMeaning
    {
        [JsonPropertyName("partOfSpeech")] public string PartOfSpeech { get; set; }

        [JsonPropertyName("definitions")] public IList<DictionaryApiDefinition> Definitions { get; set; }
    }

    public class DictionaryApiDefinition
    {
        [JsonPropertyName("definition")] public string Definition { get; set; }

        [JsonPropertyName("example")] public string Example { get; set; }

        [JsonPropertyName("synonyms")] public IList<string?>? Synonyms { get; set; }

        [JsonPropertyName("antonyms")] public IList<string?>? Antonyms { get; set; }
    }
}
