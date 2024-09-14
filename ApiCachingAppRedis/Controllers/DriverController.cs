using ApiCachingAppRedis.Data;
using ApiCachingAppRedis.Models;
using ApiCachingAppRedis.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiCachingAppRedis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DriverController : ControllerBase
    {
        private readonly ICacheService _cacheService;
        private readonly AppDbContext _dbContext;
        public DriverController(ICacheService cacheService, AppDbContext dbContext)
        {
            _cacheService = cacheService;
            _dbContext = dbContext;
        }

        [HttpGet("drivers")]
        public async Task<IActionResult> Get()
        {
            var cacheData = _cacheService.GetData<IEnumerable<Driver>>("drivers");
            if(cacheData != null && cacheData.Count() > 0 )
            {
                return Ok(cacheData);
            }
            cacheData = await _dbContext.Drivers.ToListAsync();
            var expiryTime = DateTimeOffset.Now.AddSeconds(30);
            _cacheService.SetData<IEnumerable<Driver>>("drivers", cacheData, expiryTime);
            return Ok(cacheData);
        }
        [HttpPost("AddDriver")]
        public async Task<IActionResult> Post(Driver driver)
        {
          var addedObj = await _dbContext.Drivers.AddAsync(driver);
            var expiryTime = DateTimeOffset.Now.AddSeconds(30);
            _cacheService.SetData<Driver>($"driver{driver.Id}", addedObj.Entity, expiryTime);
            await _dbContext.SaveChangesAsync();
            return Ok(addedObj.Entity);
        }

        [HttpDelete("DeleteDriver")]
        public async Task<IActionResult> Delete(int id)
        {
            var exists = _dbContext.Drivers.FirstOrDefaultAsync(x => x.Id == id);
            if (exists != null)
            {
                _dbContext.Remove(exists);
                _cacheService.RemoveData($"driver{id}");
                await _dbContext.SaveChangesAsync();
                return NoContent();
            }
            return NotFound();


        }

    }
}
