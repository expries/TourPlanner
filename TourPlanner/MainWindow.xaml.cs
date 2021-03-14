using Microsoft.Extensions.DependencyInjection;
using TourPlanner.Factories;
using TourPlanner.Repositories;
using TourPlanner.Services;
using TourPlanner.State;
using TourPlanner.ViewModels;

namespace TourPlanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var services = new ServiceCollection();
            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();
            DataContext = serviceProvider.GetService<MainViewModel>();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<MainViewModel, MainViewModel>();
            services.AddScoped<HomeViewModel, HomeViewModel>();
            services.AddScoped<RouteViewModel, RouteViewModel>();

            services.AddScoped<IViewModelFactory, ViewModelFactory>();
            services.AddScoped<INavigator, Navigator>();

            services.AddScoped<IRouteRepository, RouteRepository>();
            services.AddScoped<IRouteService, RouteService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<ILocationRepository, LocationRepository>();
        }
    }
}