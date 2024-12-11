using Microsoft.EntityFrameworkCore;
using CoffeeChart.Models;

namespace CoffeeChart.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<CoffeeConsumption> CoffeeConsumption { get; set; }
    }
}
