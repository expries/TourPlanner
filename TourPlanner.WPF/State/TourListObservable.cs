using System;
using System.Collections.Generic;
using TourPlanner.Domain.Models;

namespace TourPlanner.WPF.State
{
    public class TourListObservable : ITourListObservable
    {
        private readonly List<Action<List<Tour>>> _subscriptions;
        
        private List<Tour> _tours;

        public TourListObservable()
        {
            this._tours = new List<Tour>();
            this._subscriptions = new List<Action<List<Tour>>>();
        }

        public void Subscribe(Action<List<Tour>> subscription)
        {
            if (!this._subscriptions.Contains(subscription))
            {
                this._subscriptions.Add(subscription);
            }
        }
        
        public void Update(List<Tour> newTours)
        {
            this._tours = newTours;
            Notify();
        }

        public void Save(Tour tour)
        {
            var newTours = this._tours.FindAll(x => x.TourId != tour.TourId);
            newTours.Add(tour);
            Update(newTours);
        }

        private void Notify()
        {
            this._subscriptions.ForEach(subscription => subscription(this._tours));
        }
    }
}