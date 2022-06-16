using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherAPI.Models;

namespace WeatherAPI.Repositories
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly WeatherContext _context;

        public WeatherRepository(WeatherContext context)
        {
            _context = context;
        }

        public async Task<WeatherModel> Create(WeatherModel model)
        {
            _context.WeatherModel.Add(model);
            await _context.SaveChangesAsync();

            return model;
        }

        public async Task Delete(string name)
        {
            var weatherToDelete = await _context.WeatherModel.Where(o => o.location.name == name).FirstOrDefaultAsync();
            try
            {
                _context.WeatherModel.Remove(weatherToDelete);
            }
            catch (Exception)
            {

            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<WeatherModel>> Get()
        {
            return await _context.WeatherModel.ToListAsync();
        }

        public async Task<WeatherModel> Get(string name)
        {
            return await _context.WeatherModel.Include(x => x.location).Include(x => x.current).ThenInclude(x => x.condition).Where(o => o.location.name == name).FirstOrDefaultAsync();
        }

        public async Task Update(WeatherModel model)
        {
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
