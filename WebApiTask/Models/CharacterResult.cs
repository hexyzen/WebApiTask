using Newtonsoft.Json;

namespace WebApiTask.Models
{
    public class CharacterResult
    {
        [JsonProperty("results")]
        public Character[] Results { get; set; }
    }
}
