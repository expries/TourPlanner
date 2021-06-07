using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using TourPlanner.BL.Services;
using TourPlanner.Domain.Exceptions;
using TourPlanner.Domain.Models;
using TourPlanner.WPF.State;
using TourPlanner.WPF.ValidationRules;

namespace TourPlanner.WPF.ViewModels
{
    public class NewTourLogViewModel : ValidatedViewModelBase
    {
        private readonly ITourService _tourService;

        private readonly ITourListObservable _tourListObservable;
        
        private TourLog _tourLog;

        private Tour _tour;

        private bool _saveTourLogTriggered;
        
        private bool _saveTourLogInProgress;

        [Required(ErrorMessage="Dieses Feld ist verpflichtend.")]
        public DateTime Date
        {
            get => this._tourLog.Date;
            set
            {
                this._tourLog.Date = value;
                OnPropertyChanged();
            }
        }

        [Required(ErrorMessage="Dieses Feld ist verpflichtend.")]
        [Greater(0, ErrorMessage="Die Dauer muss eine positive Zahl sein.")]
        public double Duration
        {
            get => this._tourLog.Duration;
            set
            {
                this._tourLog.Duration = value;
                OnPropertyChanged();
            }
        }
        
        [Required(ErrorMessage="Dieses Feld ist verpflichtend.")]
        [Greater(0, ErrorMessage="Die Distanz muss eine positive Zahl sein.")]
        public double Distance
        {
            get => this._tourLog.Distance;
            set
            {
                this._tourLog.Distance = value;
                OnPropertyChanged();
            }
        }

        [Required(ErrorMessage="Dieses Feld ist verpflichtend.")]
        [Range(1, 5, ErrorMessage="Die Bewertung muss im Wertebereich von 1 bis 5 liegen.")]
        public int Rating
        {
            get => this._tourLog.Rating;
            set
            {
                this._tourLog.Rating = value;
                OnPropertyChanged();
            }
        }

        [Required(ErrorMessage="Dieses Feld ist verpflichtend.")]
        [Range(-45, 45, ErrorMessage="Die Temperatur muss im Wertebereich von -45 bis +45 °C liegen.")]
        public double Temperature
        {
            get => this._tourLog.Temperature;
            set
            {
                this._tourLog.Temperature = value;
                OnPropertyChanged();
            }
        }

        [Required(ErrorMessage="Dieses Feld ist verpflichtend.")]
        [Range(1, 300, ErrorMessage="Die Durchschnittsgeschwindigkeit muss im Wertebereich von 1 bis 300 km/h liegen.")]
        public double AverageSpeed
        {
            get => this._tourLog.AverageSpeed;
            set
            {
                this._tourLog.AverageSpeed = value;
                OnPropertyChanged();
            }
        }

        [Required(ErrorMessage="Dieses Feld ist verpflichtend.")]
        [Range(1, 5, ErrorMessage="Das Gefahrenniveau muss im Wertebereich von 1 bis 5 liegen.")]
        public int DangerLevel
        {
            get => this._tourLog.DangerLevel;
            set
            {
                this._tourLog.DangerLevel = value;
                OnPropertyChanged();
            }
        }

        [Required(ErrorMessage="Dieses Feld ist verpflichtend.")]
        public Difficulty Difficulty
        {
            get => this._tourLog.Difficulty;
            set
            {
                this._tourLog.Difficulty = value;
                OnPropertyChanged();
            }
        }

        [Required(ErrorMessage="Dieses Feld ist verpflichtend.")]
        public WeatherCondition Weather
        {
            get => this._tourLog.Weather;
            set
            {
                this._tourLog.Weather = value;
                OnPropertyChanged();
            }
        }

        public List<Difficulty> Difficulties { get; } = new List<Difficulty>
        {
            Difficulty.VeryEasy,
            Difficulty.Easy,
            Difficulty.Medium,
            Difficulty.Hard,
            Difficulty.VeryHard
        };

        public List<WeatherCondition> WeatherConditions { get; } = new List<WeatherCondition>
        {
            WeatherCondition.Cloudy,
            WeatherCondition.Rainy,
            WeatherCondition.Sunny,
            WeatherCondition.Windy
        };

        public ICommand SaveTourLogCommand { get; }

        public NewTourLogViewModel(ITourService tourService, ITourListObservable tourListObservable)
        {
            this._tourService = tourService;
            this._tourListObservable = tourListObservable;
            this.SaveTourLogCommand = new RelayCommand(SaveTourLog, SaveTourLogCanExecute);
        }

        public void Accept(TourLog tourLog)
        {
            this._tour = tourLog.Tour.Value;
            this._tourLog = tourLog;
        }

        public void Accept(Tour tour)
        {
            this._tour = tour;
            this._tourLog = new TourLog(tour);
        }

        public async void SaveTourLog(object parameter)
        {
            try
            {
                this._saveTourLogTriggered = true;

                if (!SaveTourLogCanExecute(null))
                {
                    return;
                }

                this._saveTourLogInProgress = true;
                var log = await this._tourService.SaveTourLogAsync(this._tourLog);
                this._saveTourLogInProgress = false;
                
                this._tour.TourLogs.Value.RemoveAll(x => x.TourLogId == this._tourLog.TourLogId);
                this._tour.TourLogs.Value.Add(log);
                
                this._tourListObservable.Save(this._tour);
                Navigator.Instance.UpdateCurrentViewModel(ViewType.Home);
            }
            catch (BusinessException ex)
            {
                DisplayError(ex.Message);
                this._saveTourLogInProgress = false;
            }
        }

        public bool SaveTourLogCanExecute(object parameter)
        {
            if (!this._saveTourLogTriggered)
            {
                return true;
            }
            
            return Validate();
        }
    }
}