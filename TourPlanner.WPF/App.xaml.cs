using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TourPlanner.BL.Services;
using TourPlanner.DAL.Repositories;
using TourPlanner.WPF.State;
using TourPlanner.WPF.ViewModels;

namespace TourPlanner.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;
    
        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            this._serviceProvider = services.BuildServiceProvider();
        }
        
        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<NewTourViewModel>();
            services.AddSingleton<HomeViewModel>();

            services.AddSingleton<INavigator, Navigator>();

            services.AddScoped<ITourRepository, TourRepository>();
            services.AddScoped<ITourService, TourService>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<ILocationService, LocationService>();
        }

        private void OnStartUp(object sender, StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            
            var configuration = builder.Build();
            string connectionString = configuration.GetConnectionString("defaultConnectionString");
            
            var mainWindow = this._serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}