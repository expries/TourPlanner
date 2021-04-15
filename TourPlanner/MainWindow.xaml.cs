using Microsoft.Extensions.DependencyInjection;
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
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<NewTourViewModel>();
            services.AddSingleton<HomeViewModel>();

            services.AddSingleton<INavigator, Navigator>();

            services.AddScoped<ITourRepository, TourRepository>();
            services.AddScoped<ITourService, TourService>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<ILocationService, LocationService>();
        }
    }
}