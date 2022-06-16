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
using WeatherAPI.Utilities;
using System.Xml.Serialization;
using System.Diagnostics;

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

        [HttpGet("Retrive")]
        [Authorize(Roles = "Administrator,Guest")]
        public async Task<ActionResult<WeatherModel>> GetWeather([FromBody]string name)
        {
            return await _weatherRepository.Get(name);
        }

        [HttpPost("Update")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<WeatherModel>> PostWeather([FromBody]string name)
        {
            await _weatherRepository.Delete(name);
            config c = null;
            var xmlSerializer = new XmlSerializer(typeof(config));
            using (var reader = new System.IO.StreamReader("ConfigXMLFile.xml"))
            {
                c = (config)xmlSerializer.Deserialize(reader);
            }
            WeatherModel model = new WeatherModel();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{c.api[0].@base}?key= {c.api[0].key}&q={name}&aqi=no");
            var client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);
            if(response.IsSuccessStatusCode)
            {
                model = await response.Content.ReadFromJsonAsync<WeatherModel>();
            }

            var newWeather = await _weatherRepository.Create(model);
            return CreatedAtAction(nameof(GetWeather), new { name = newWeather.location.name }, newWeather);
        }


        [HttpDelete("Remove")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Delete([FromBody] string name)
        {
            var weatherToDelete = await _weatherRepository.Get(name);
            if (weatherToDelete == null)
                return NotFound();

            await _weatherRepository.Delete(weatherToDelete.location.name);
            return NoContent();
        }

    }
}
