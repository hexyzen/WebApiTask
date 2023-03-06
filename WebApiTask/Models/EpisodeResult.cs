using Newtonsoft.Json;

namespace WebApiTask.Models
{
    public class EpisodeResult
    {

        [JsonProperty("results")]
        public Episode[] Results { get; set; }
    }
}
    

