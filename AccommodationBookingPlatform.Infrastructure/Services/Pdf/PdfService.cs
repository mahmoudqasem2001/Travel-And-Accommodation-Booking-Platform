using Domain.Interfaces.Services;
using NReco.PdfGenerator;

namespace AccommodationBookingPlatform.Infrastructure.Services.Pdf;

public class PdfService : IPdfService
{
    public async Task<byte[]> GeneratePdfFromHtmlAsync(string html, CancellationToken cancellationToken = default)
    { 
        /// <summary>
            /// This is a synchronous operation, so Task.Run is used to prevent blocking in an async context.
        /// </summary>
        return await Task.Run(() =>
        {
          
            var htmlToPdfConverter = new HtmlToPdfConverter();
        
            return htmlToPdfConverter.GeneratePdf(html);
        }, cancellationToken);
    }
}