using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Diagnostics;
using TourPlanner.Domain.Models;

namespace TourPlanner.Domain.Documents
{   
    public class TourReport : IDocument
    {
        public Tour Model { get; }

        public TourReport(Tour model)
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
                stack.Item().Row(row =>
                {
                    row.RelativeColumn().Stack(stack =>
                    {
                        stack.Item().Text("Tour Report", TextStyle.Default.Size(18));
                        stack.Item().PaddingTop(10);
                        
                        stack.Item().Text($"Von: {this.Model.From}");
                        stack.Item().Text($"Bis: {this.Model.To}");

                        stack.Item().PaddingTop(7);

                        stack.Item().Text($"Name: {this.Model.Name}");
                        stack.Item().Text($"Typ: {this.Model.Type}");
                        stack.Item().Text($"Distanz: {this.Model.Distance}km");
                        stack.Item().Text($"Vergangene Touren: {this.Model.TourLogs.Value.Count}");

                        stack.Item().PaddingTop(7);

                        stack.Item().Text("Beschreibung:");
                        stack.Item().Text($"{this.Model.Description}");
                    });

                    row.ConstantColumn(235).Stack(stack =>
                    {
                        stack.Item().Image(this.Model.Image);
                    });
                });
                
                stack.Item().PaddingTop(15);
                stack.Item().Text("Toureinträge", TextStyle.Default.Size(16));
                stack.Item().PaddingTop(15);

                stack.Item().BorderBottom(1).Padding(5).Row(row =>
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
                
                foreach (var log in this.Model.TourLogs.Value)
                {
                    stack.Item().BorderBottom(1).BorderColor("CCC").Padding(5).Row(row =>
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
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.AlignCenter().PageNumber("Seite {number}");
        }
    }
}