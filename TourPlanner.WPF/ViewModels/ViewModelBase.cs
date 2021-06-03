using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace TourPlanner.WPF.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual object Context { get; set; }

        public virtual void OnNavigation(object context)
        {
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        protected static void DisplayError(string error)
        {
            MessageBox.Show(error, "Fehler", MessageBoxButton.OK);
        }
    }
}