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
            var navigator = serviceProvider.GetService<INavigator>();
            DataContext = new MainViewModel(navigator);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<INavigator, Navigator>();
            services.AddScoped<HomeViewModel, HomeViewModel>();
            services.AddScoped<RouteViewModel, RouteViewModel>();
            services.AddScoped<IViewModelFactory, ViewModelFactory>();
            services.AddScoped<IRouteRepository, RouteRepository>();
            services.AddScoped<IRouteService, RouteService>();
        }
    }
}