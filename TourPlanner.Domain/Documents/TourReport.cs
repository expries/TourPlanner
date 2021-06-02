using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using TourPlanner.Domain.Models;
using IContainer = QuestPDF.Infrastructure.IContainer;

namespace TourPlanner.Domain.Documents
{
    public static class TourReportExtension
    {
        public static void DoStuff(this IContainer container)
        {
            container.Text("dd");
        }
    }
    
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
                        stack.Item().DoStuff();
                        stack.Item().Text($"Tour: {this.Model.Name}", TextStyle.Default.Size(20));
                        stack.Item().Text($"Von: {this.Model.From}");
                        stack.Item().Text($"Bis: {this.Model.To}");
                        stack.Item().Text($"Typ: {this.Model.Type}");
                        stack.Item().Text($"Distanz: {this.Model.Distance}");
                        stack.Item().Text($"Beschreibung: {this.Model.Description}");
                        stack.Spacing(5);
                    });
                });

                stack.Item().PaddingTop(40);

                stack.Item().BorderBottom(1).Padding(5).Row(row =>
                {
                    row.RelativeColumn().Text("Datum");
                    row.RelativeColumn().Text("Distanz");
                    row.RelativeColumn().Text("Dauer");
                    row.RelativeColumn().Text("Bewertung");
                    row.RelativeColumn().Text("Schwierigkeit");
                    row.RelativeColumn().Text("Wetter");
                    row.RelativeColumn().Text("Temperatur");
                });
                
                foreach (var log in this.Model.TourLogs.Value)
                {
                    stack.Item().BorderBottom(1).BorderColor("CCC").Padding(5).Row(row =>
                    {
                        row.RelativeColumn().Text(log.Date);
                        row.RelativeColumn().Text($"{log.Distance}km");
                        row.RelativeColumn().Text($"{log.Duration}min");
                        row.RelativeColumn().Text($"{log.Rating}/5");
                        row.RelativeColumn().Text(log.Difficulty);
                        row.RelativeColumn().Text(log.Weather);
                        row.RelativeColumn().Text($"{log.Temperature}°C");
                    });
                }
            });
        }
    }
}