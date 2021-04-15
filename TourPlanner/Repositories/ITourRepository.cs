﻿using System.Collections.Generic;
using TourPlanner.Models;

namespace TourPlanner.Repositories
{
    public interface ITourRepository
    {
        public List<Tour> GetTours();

        public void SaveTour(Tour tour);
    }
}