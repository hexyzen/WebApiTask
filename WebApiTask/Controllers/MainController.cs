using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Newtonsoft.Json;
using System.Net.Http;
using WebApiTask.Models;

namespace WebApiTask.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MainController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [ResponseCache(CacheProfileName = "Default14days")]
        [HttpPost("check-person")]
        public async Task<ActionResult<bool>> CheckPerson(string personName, string episodeName)
        {
            var episodeResponse = await _httpClientFactory.CreateClient().GetAsync($"https://rickandmortyapi.com/api/episode?name={episodeName}");
            episodeResponse.EnsureSuccessStatusCode();
            var episodeJson = await episodeResponse.Content.ReadAsStringAsync();
            var episodeResult = JsonConvert.DeserializeObject<EpisodeResult>(episodeJson);
            var episode = episodeResult.Results[0];

            if (episode == null)
            {
                return NotFound();
            }

            foreach (var characterUrl in episode.characters)
            {
                var characterResponse = await _httpClientFactory.CreateClient().GetAsync(characterUrl);
                var characterJson = await characterResponse.Content.ReadAsStringAsync();
                var character = JsonConvert.DeserializeObject<Person>(characterJson);

                if (character.Name.Equals(personName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        [ResponseCache(CacheProfileName = "Default14days")]
        [HttpGet("person")]
        public async Task<IActionResult> GetCharacter([FromQuery] string name)
        {
            var response = await _httpClientFactory.CreateClient().GetAsync($"https://rickandmortyapi.com/api/character/?name={name}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var characterResult = JsonConvert.DeserializeObject<CharacterResult>(content);

            var character = characterResult.Results[0];
            var originResponse = await _httpClientFactory.CreateClient().GetAsync(character.Origin.Url);

            if (!originResponse.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var originJson = await originResponse.Content.ReadAsStringAsync();
            var origin = JsonConvert.DeserializeObject<Location>(originJson); //AutoMapper need to use
            var result = new
            {
                character.Name,
                character.Status,
                character.Species,
                character.Type,
                character.Gender,
                Origin = new
                {
                    origin.Name,
                    origin.Type,
                    origin.Dimension
                }
            };

            return Ok(result);
        }

    }
}
