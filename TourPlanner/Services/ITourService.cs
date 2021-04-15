﻿using System.Collections.Generic;
using TourPlanner.Models;

namespace TourPlanner.Services
{
    public interface ITourService
    {
        public List<Tour> GetTours();

        public List<Tour> FindTours(string query);

        public void SaveTour(Tour tour);
    }
}