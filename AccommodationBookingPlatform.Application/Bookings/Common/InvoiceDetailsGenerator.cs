using AccommodationBookingPlatform.Domain.Entities;
using System.Text;

namespace AccommodationBookingPlatform.Application.Bookings.Common;

public static class InvoiceDetailsGenerator
{
    public static string GetInvoiceHtml(Booking booking)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.Append(
            """
        <!DOCTYPE html>
        <html lang="en">
        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>Booking Invoice</title>
            <style>
                body {
                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                    background-color: #1e1e2e;
                    color: #ffffff;
                    padding: 20px;
                    text-align: center;
                }
                .container {
                    max-width: 800px;
                    margin: auto;
                    background: #282a36;
                    padding: 20px;
                    border-radius: 10px;
                    box-shadow: 0 0 15px rgba(0, 0, 0, 0.3);
                }
                h1 {
                    color: #ffcc00;
                }
                table {
                    width: 100%;
                    border-collapse: collapse;
                    margin-top: 20px;
                    background: #3a3d4a;
                    border-radius: 8px;
                    overflow: hidden;
                }
                th, td {
                    border: 1px solid #444;
                    padding: 12px;
                    text-align: center;
                }
                th {
                    background-color: #ffcc00;
                    color: #1e1e2e;
                }
                .total {
                    font-size: 1.2em;
                    font-weight: bold;
                    margin-top: 20px;
                }
            </style>
        </head>
        """)
          .Append($"""
         <body>
             <div class="container">
                 <h1>Booking Invoice</h1>
                 <p><strong>Hotel:</strong> {booking.Hotel.Name}</p>
                 <p><strong>Location:</strong> {booking.Hotel.City.Name}, {booking.Hotel.City.Country}</p>
                 <p><strong>Check-in:</strong> {booking.CheckInDateUtc}</p>
                 <p><strong>Check-out:</strong> {booking.CheckOutDateUtc}</p>
                 <p><strong>Booking Date:</strong> {booking.BookingDateUtc}</p>
         """)
          .Append("""
        <table>
        <thead>
            <tr>
                <th>Room Number</th>
                <th>Room Type</th>
                <th>Price</th>
                <th>Discount %</th>
                <th>Final Price</th>
            </tr>
        </thead>
        <tbody>
        """);

        foreach (var invoiceRecord in booking.Invoice)
        {
            stringBuilder.Append($"""
         <tr>
             <td>{invoiceRecord.RoomNumber}</td>
             <td>{invoiceRecord.RoomClassName}</td>
             <td>${invoiceRecord.PriceAtBooking:F2}</td>
             <td>{invoiceRecord.DiscountPercentageAtBooking ?? 0}%</td>
             <td>${invoiceRecord.PriceAtBooking * (1 - (invoiceRecord.DiscountPercentageAtBooking ?? 0) / 100):F2}</td>
         </tr>
         """);
        }

        stringBuilder.Append($"""
       </tbody>
       </table>
       <p class="total">Total Price: ${booking.TotalPrice:F2}</p>
       </div>
       </body>
       </html>
       """);

        return stringBuilder.ToString();
    }
}