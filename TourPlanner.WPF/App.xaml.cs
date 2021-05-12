using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using log4net.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TourPlanner.BL.Services;
using TourPlanner.DAL;
using TourPlanner.DAL.Repositories;
using TourPlanner.Domain;
using TourPlanner.Domain.Models;
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

        private static void ConfigureServices(IServiceCollection services)
        {
            XmlConfigurator.Configure();

            DatabaseConnection.MapEnum<TourType>("tour_type");
            DatabaseConnection.MapEnum<Difficulty>("difficulty_type");
            DatabaseConnection.MapEnum<WeatherCondition>("weather_type");
            
            var configuration = GetConfiguration();
            services.AddSingleton<IConfiguration>(_ => configuration);
            
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<NewTourLogViewModel>();
            services.AddSingleton<NewTourViewModel>();
            services.AddSingleton<HomeViewModel>();

            services.AddSingleton<INavigator, Navigator>();

            services.AddSingleton<ITourRepository, TourRepository>();
            services.AddSingleton<IMapRepository, MapRepository>();
            services.AddSingleton<ITourService, TourService>();
            services.AddSingleton<IMapService, MapService>();
            services.AddSingleton<IReportService, ReportService>();
        }
        
        private static IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            
            return builder.Build();
        }

        private void OnStartUp(object sender, StartupEventArgs e)
        {
            var mainWindow = this._serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}