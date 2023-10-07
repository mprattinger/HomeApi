using HomeApi.Data;
using HomeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeApi.Services;

public interface ITempService
{
    Task Add(string location, double temp, double humidity);
    Task<TempEntry> GetCurrentTemp(string location);
    Task<List<TempEntry>> GetCurrentDayTemp(string location);
}

public class TempService : ITempService
{
    private readonly ILogger<TempService> _logger;
    private readonly HomeApiContext _context;
    private List<TempEntry> _temps = new();
    
    public TempService(ILogger<TempService> logger, HomeApiContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task Add(string location, double temp, double humidity)
    {
        var entry = new TempEntry()
        {
            Location = location,
            Temperature = temp,
            Humidity = humidity,
            MeasueredAt = DateTime.Now
        };

        _context.TempEntries.Add(entry);
        await _context.SaveChangesAsync();
    }

    public async Task<TempEntry> GetCurrentTemp(string location)
    {
        var temp = await _context
            .TempEntries
            .Where(x => x.Location == location)
            .OrderByDescending(x => x.MeasueredAt)
            .FirstOrDefaultAsync();
        
        if (temp == null)
        {
            return new();   
        }
        
        return await Task.FromResult(temp);
    }

    public async Task<List<TempEntry>> GetCurrentDayTemp(string location)
    {
        var ts = await _context.TempEntries
            .Where(x => (x.MeasueredAt >= DateTime.Now.Date && x.MeasueredAt < DateTime.Now.Date.AddDays(1)) &&
                        x.Location == location)
            .ToListAsync();

        return await Task.FromResult(ts);
    }
}