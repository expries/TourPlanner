using System;
using System.Collections.Generic;
using TourPlanner.Domain.Models;

namespace TourPlanner.WPF.State
{
    public interface ITourListObservable
    {
        public void Subscribe(Action<List<Tour>> subscription);

        public void Update(List<Tour> newTours);

        public void Save(Tour tour);
    }
}