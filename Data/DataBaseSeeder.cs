using CoffeeChart.Models;

namespace CoffeeChart.Data
{
    public static class DatabaseSeeder
    {
        public static void Seed(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<DatabaseContext>();
                if (!context.CoffeeConsumption.Any())
                {
                    context.CoffeeConsumption.AddRange(
                        new CoffeeConsumption { Year = 2018, Consumption = 9 },
                        new CoffeeConsumption { Year = 2019, Consumption = 10 },
                        new CoffeeConsumption { Year = 2020, Consumption = 11 },
                        new CoffeeConsumption { Year = 2021, Consumption = 12 },
                        new CoffeeConsumption { Year = 2022, Consumption = 6 }
                    );
                    context.SaveChanges();
                }
            }
        }
    }
}
