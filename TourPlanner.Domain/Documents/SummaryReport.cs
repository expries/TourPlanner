using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using TourPlanner.Domain.Models;

namespace TourPlanner.Domain.Documents
{
    public class SummaryReport : IDocument
    {
        public List<Tour> Model { get; }

        public SummaryReport(List<Tour> model)
        {
            this.Model = model;
        }
        
        public DocumentMetadata GetMetadata()
        {
            return DocumentMetadata.Default;
        }

        public void Compose(IContainer container)
        {
            container
                .PaddingHorizontal(50)
                .PaddingVertical(50)
                .Page(page =>
                {
                    ComposeContent(page.Content().Container());
                    ComposeFooter(page.Footer().Container());
                });
        }

        private void ComposeContent(IContainer container)
        {
            container.Stack(stack =>
            {
                int tourCount = this.Model.Count;
                int tourLogCount = this.Model.Sum(tour => tour.TourLogs.Value.Count);
                double averagePace = this.Model.Average(tour => tour.TourLogs.Value.Sum(log => log.AverageSpeed));
                double totalDistance = this.Model.Sum(tour => tour.TourLogs.Value.Sum(log => log.Distance));
                double totalDuration = this.Model.Sum(tour => tour.TourLogs.Value.Sum(log => log.Duration));

                stack.Item().Text("Tour-Summary Report", TextStyle.Default.Size(18));
                stack.Item().PaddingTop(10);
                stack.Item().Text($"Touren: {tourCount}");
                stack.Item().Text($"Tourlogs: {tourLogCount}");
                stack.Item().Text($"Gesamtdistanz: {totalDistance}km");
                stack.Item().Text($"Gesamtdauer: {totalDuration}h");
                stack.Item().Text($"Durchschnittsgeschwindigkeit: {averagePace}km/h");
                
                stack.Item().PaddingTop(15);
                stack.Item().Text("Tourübersicht", TextStyle.Default.Size(16));
                stack.Item().PaddingTop(5);

                stack.Item().BorderBottom(1).Padding(5).Row(row =>
                {
                    row.RelativeColumn().Text("Name");
                    row.RelativeColumn(2).Text("Von");
                    row.RelativeColumn(2).Text("Bis");
                    row.RelativeColumn().Text("Typ");
                    row.RelativeColumn().Text("Bewertung");
                    row.RelativeColumn().Text("Loganzahl");
                });

                foreach (var tour in this.Model)
                {
                    stack.Item().BorderBottom(1).BorderColor("CCC").Padding(5).Row(row =>
                    {
                        var logs = tour.TourLogs.Value;
                        double averageRating = logs.Sum(log => log.Rating) / Math.Max(1, logs.Count);
                        averageRating = Math.Round(averageRating, 1);
                        
                        row.RelativeColumn().Text(tour.Name);
                        row.RelativeColumn(2).Text(tour.From);
                        row.RelativeColumn(2).Text(tour.To);
                        row.RelativeColumn().Text(tour.Type);
                        row.RelativeColumn().Text($"{averageRating}/5");
                        row.RelativeColumn().Text(logs.Count);
                    });
                }
                
                stack.Item().PaddingTop(20);
                stack.Item().Text("Einzeltouren", TextStyle.Default.Size(16));
                stack.Item().PaddingTop(20);
                
                this.Model.ForEach(tour => ComposeTour(stack, tour));
            });
        }

        private static void ComposeTour(StackDescriptor outerStack, Tour tour)
        {
            outerStack.Item().Row(row =>
            {
                row.RelativeColumn().Stack(stack =>
                {
                    stack.Item().Text("Tourdetails", TextStyle.Default.Size(14));
                    stack.Item().PaddingTop(10);
                    
                    stack.Item().Text($"Von: {tour.From}");
                    stack.Item().Text($"Bis: {tour.To}");

                    stack.Item().PaddingTop(7);

                    stack.Item().Text($"Name: {tour.Name}");
                    stack.Item().Text($"Typ: {tour.Type}");
                    stack.Item().Text($"Distanz: {tour.Distance}km");
                    stack.Item().Text($"Vergangene Touren: {tour.TourLogs.Value.Count}");

                    stack.Item().PaddingTop(7);

                    stack.Item().Text("Beschreibung:");
                    stack.Item().Text($"{tour.Description}");
                });

                row.ConstantColumn(235).Stack(stack =>
                {
                    stack.Item().Image(tour.Image);
                });

                if (tour.TourLogs.Value.Count == 0)
                {
                    outerStack.Item().PaddingBottom(25);
                    return;
                }
                
                outerStack.Item().PaddingTop(15);
                outerStack.Item().Text("Toureinträge", TextStyle.Default.Size(14));
                outerStack.Item().PaddingTop(10);

                outerStack.Item().BorderBottom(1).Padding(5).Row(row =>
                {
                    row.RelativeColumn().Text("Datum");
                    row.RelativeColumn().Text("Bewertung");
                    row.RelativeColumn().Text("Distanz");
                    row.RelativeColumn().Text("Dauer");
                    row.RelativeColumn().Text("km/h");
                    row.RelativeColumn().Text("Difficulty");
                    row.RelativeColumn().Text("Schwierigk");
                    row.RelativeColumn().Text("Wetter");
                    row.RelativeColumn().Text("C°");
                });
                
                foreach (var log in tour.TourLogs.Value)
                {
                    outerStack.Item().BorderBottom(1).BorderColor("CCC").Padding(5).Row(row =>
                    {
                        row.RelativeColumn().Text($"{log.Date.Day}.{log.Date.Month}.{log.Date.Year}");
                        row.RelativeColumn().Text($"{log.Rating}/5");
                        row.RelativeColumn().Text($"{log.Distance}km");
                        row.RelativeColumn().Text($"{log.Duration}min");
                        row.RelativeColumn().Text($"{log.AverageSpeed}");
                        row.RelativeColumn().Text($"{log.DangerLevel}/5");
                        row.RelativeColumn().Text(log.Difficulty);
                        row.RelativeColumn().Text(log.Weather);
                        row.RelativeColumn().Text($"{log.Temperature}");
                    });

                }
                
                outerStack.Item().PaddingBottom(25);
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.AlignCenter().PageNumber("Seite {number}");
        }
    }
}