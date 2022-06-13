using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WeatherAPI.Models;
using WeatherAPI.Repositories;
using System.Net.Http.Json;

namespace WeatherAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherRepository _weatherRepository;
        private readonly IHttpClientFactory _clientFactory;

        public WeatherController(IWeatherRepository weatherRepository, IHttpClientFactory clientFactory)
        {
            _weatherRepository = weatherRepository;
            _clientFactory = clientFactory;
        }

        [HttpGet("{name}")]
        [Authorize(Roles = "Administrator,Guest")]
        public async Task<ActionResult<WeatherModel>> GetWeather(string name)
        {
            return await _weatherRepository.Get(name);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<WeatherModel>> PostWeather(WeatherModel model)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://api.weatherapi.com/v1/current.json?key= 90da7c4066da4051a93105148221206&q=Warsaw&aqi=no");
            var client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);
            if(response.IsSuccessStatusCode)
            {
                model = await response.Content.ReadFromJsonAsync<WeatherModel>();
            }

            var newWeather = await _weatherRepository.Create(model);
            return CreatedAtAction(nameof(GetWeather), new { name = newWeather.location.name }, newWeather);
        }

        [HttpPut]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> PutWeather(string name, [FromBody] WeatherModel model)
        {
            if (name != model.location.name)
            {
                return BadRequest();
            }

            await _weatherRepository.Update(model);

            return NoContent();
        }

        [HttpDelete("{name}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Delete(string name)
        {
            var weatherToDelete = await _weatherRepository.Get(name);
            if (weatherToDelete == null)
                return NotFound();

            await _weatherRepository.Delete(weatherToDelete.location.name);
            return NoContent();
        }

    }
}
