using System.Collections.Generic;
using QuestPDF.Drawing;
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
            throw new System.NotImplementedException();
        }
    }
}