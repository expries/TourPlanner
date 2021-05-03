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

        private static void ConfigureServices(IServiceCollection services)
        {
            var configuration = GetConfiguration();
            services.AddSingleton<IConfiguration>(_ => configuration);
            
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<NewTourViewModel>();
            services.AddSingleton<HomeViewModel>();

            services.AddSingleton<INavigator, Navigator>();

            services.AddSingleton<ITourRepository, TourRepository>();
            services.AddSingleton<IMapRepository, MapRepository>();
            services.AddSingleton<ITourService, TourService>();
            services.AddSingleton<IMapService, MapService>();
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