using System.Windows;
using TourPlanner.WPF.ViewModels;

namespace TourPlanner.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel model)
        {
            InitializeComponent();
            this.DataContext = model;
        }
    }
}