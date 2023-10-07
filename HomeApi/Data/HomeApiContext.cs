using HomeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeApi.Data;

public class HomeApiContext : DbContext
{
    public DbSet<TempEntry> TempEntries => Set<TempEntry>();

    public HomeApiContext(DbContextOptions<HomeApiContext> options): base(options)
    {
        
    }
}