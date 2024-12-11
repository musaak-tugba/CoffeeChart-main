using System;
using System.Data.SQLite;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CoffeeChart.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string Username, string Password, string DatabaseName)
        {
            try
            {
                var connectionString = DatabaseName == "CoffeeChartNew.db"
                    ? _configuration.GetConnectionString("NewConnection")
                    : _configuration.GetConnectionString("DefaultConnection");

                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    var command = new SQLiteCommand("SELECT * FROM Users WHERE Username = @username AND Password = @password", connection);
                    command.Parameters.AddWithValue("@username", Username);
                    command.Parameters.AddWithValue("@password", Password);

                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        HttpContext.Session.SetString("DatabaseConnectionString", connectionString);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Geçersiz kullanıcı adı veya şifre.";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Veritabanı bağlantı hatası: " + ex.Message;
                return View();
            }
        }

        public IActionResult Index()
        {
            var chartData = GetChartData();

            ViewBag.ChartData = JsonConvert.SerializeObject(chartData);

            return View();
        }

      private List<CoffeeConsumption> GetChartData()
{
    var chartData = new List<CoffeeConsumption>();
    var connectionString = HttpContext.Session.GetString("DatabaseConnectionString");

    if (connectionString == null)
    {
        ViewBag.ErrorMessage = "Veritabanı bağlantı dizesi null.";
        return chartData;
    }

    using (var connection = new SQLiteConnection(connectionString))
    {
        if (connection.State != System.Data.ConnectionState.Open)
        {
            connection.Open();
        }

        var command = new SQLiteCommand("SELECT Year, Consumption FROM CoffeeConsumptionView", connection);
        
        using (var reader = command.ExecuteReader())
        {
            if (!reader.HasRows)
            {
                ViewBag.ErrorMessage = "Veritabanında Veri Yok";
                return chartData;
            }

            while (reader.Read())
            {
                var data = new CoffeeConsumption
                {
                    Year = Convert.ToInt32(reader["Year"]),
                    Consumption = Convert.ToInt32(reader["Consumption"])
                };
                chartData.Add(data);
            }
        }
    }

    return chartData;
}

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); 
            return RedirectToAction("Login", "Home");
        }
    }
    public class CoffeeConsumption
    {
        public int Year { get; set; }
        public int Consumption { get; set; }
    }
}
