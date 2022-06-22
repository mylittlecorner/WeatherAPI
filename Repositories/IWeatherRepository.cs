using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherAPI.Models;

namespace WeatherAPI.Repositories
{
    public interface IWeatherRepository
    {
        Task<List<String>> Get();
        Task<WeatherModel> Get(string name);
        Task<WeatherModel> Create(WeatherModel model);
        Task Update(WeatherModel model);
        Task Delete(string name);
    }
}
