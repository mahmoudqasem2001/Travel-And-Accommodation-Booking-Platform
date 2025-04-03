# Travel and Accommodation Booking Platform API

This API provides a range of endpoints designed for the management of various hotel-related tasks, including booking handling, hotel and city information management, and offering guest services.

## Key Features

### User Authentication
- **User Registration**: Allows new users to create accounts by providing necessary information.
- **User Login**: Enables registered users to log in securely to access booking features.

### Global Hotel Search
- **Search by Various Criteria**: Users can search for hotels using criteria such as hotel name, room type, room capacities, price range, and other properties through text fields.
- **Comprehensive Search Results**: Provides users with detailed information about hotels matching their search criteria.

### Image Management
- **Management of Images and Thumbnails**: Allows for the addition, deletion, and updating of images associated with cities, hotels, and rooms.

### Popular Cities Display
- **Display Most Visited Cities**: Showcases popular cities based on user traffic, allowing users to explore trending destinations easily.

### Email Notifications
- **Booking Confirmation Emails**: Sends confirmation emails to users upon successful booking, containing essential information such as total price, hotel location on the map, and other relevant details.
- **Enhanced User Communication**: Facilitates effective communication with users, keeping them informed about their bookings.

### Admin Interface
- **Search, Add, Update, and Delete Entities**: Provides administrators with full control over system entities, enabling efficient management of cities, hotels, rooms, and other components.
- **Streamlined Administrative Tasks**: Simplifies administrative tasks through a user-friendly interface, enhancing system maintenance.

## Endpoints

### Amenities
| HTTP Method | Endpoint             | Description                        |
|------------|---------------------|------------------------------------|
| GET        | /api/amenities       | Retrieve a page of amenities      |
| POST       | /api/amenities       | Create a new amenity              |
| GET        | /api/amenities/{id}  | Get an amenity specified by ID    |
| PUT        | /api/amenities/{id}  | Update an existing amenity        |

### Auth
| HTTP Method | Endpoint                   | Description                         |
|------------|---------------------------|-------------------------------------|
| POST       | /api/auth/login           | Processes a login request          |
| POST       | /api/auth/register-guest  | Processes registering a guest      |

### Bookings
| HTTP Method | Endpoint                          | Description                                         |
|------------|----------------------------------|-----------------------------------------------------|
| POST       | /api/user/bookings              | Create a new booking for the current user         |
| GET        | /api/user/bookings              | Get a page of bookings for the current user       |
| DELETE     | /api/user/bookings/{id}         | Delete an existing booking specified by ID        |
| GET        | /api/user/bookings/{id}         | Get a booking specified by ID for the current user |
| GET        | /api/user/bookings/{id}/invoice | Get the invoice of a booking specified by ID as PDF for the current user |

### Cities
| HTTP Method | Endpoint                      | Description                                   |
|------------|------------------------------|-----------------------------------------------|
| GET        | /api/cities                  | Retrieve a page of cities                    |
| POST       | /api/cities                  | Create a new city                            |
| GET        | /api/cities/trending         | Returns TOP N most visited cities           |
| PUT        | /api/cities/{id}             | Update an existing city specified by ID     |
| DELETE     | /api/cities/{id}             | Delete an existing city specified by ID     |
| PUT        | /api/cities/{id}/thumbnail   | Set the thumbnail of a city specified by ID |

### Hotels
| HTTP Method | Endpoint                        | Description                                      |
|------------|--------------------------------|------------------------------------------------|
| GET        | /api/hotels                    | Retrieve a page of hotels                      |
| POST       | /api/hotels                    | Create a new hotel                            |
| GET        | /api/hotels/search             | Search and filter hotels                      |
| GET        | /api/hotels/featured-deals     | Retrieve N hotel featured deals               |
| GET        | /api/hotels/{id}               | Get hotel by ID                               |
| PUT        | /api/hotels/{id}               | Update an existing hotel                      |
| DELETE     | /api/hotels/{id}               | Delete an existing hotel                      |
| PUT        | /api/hotels/{id}/thumbnail     | Set the thumbnail of a hotel specified by ID  |

## Architecture

### Clean Architecture
#### External Layers:
- **Web**: Controllers for handling requests and managing client-server communication.
- **Infrastructure**: Manages external resources such as databases, email service, PDF generation, image service, and auth management.

#### Core Layers:
- **Application Layer**: Implements business logic and orchestrates interactions between components.
- **Domain Layer**: Contains fundamental business rules and entities, independent of external concerns like databases or user interfaces.

### Technology Stack Overview
#### Technologies Used
- **C#**: Main programming language.
- **ASP.NET Core**: Framework for building high-performance, cross-platform web APIs.

#### Database
- **Entity Framework Core**: Streamlined object-relational mapping (ORM) within the .NET ecosystem.
- **SQL Server**: Reliable and scalable backend database management.

#### Image Storage
- **Firebase Storage**: Cloud-based storage for images, offering scalability, reliability, and seamless API integration.

## API Documentation and Design
- **Swagger/OpenAPI**: For API specification and documentation.
- **Swagger UI**: Provides a user-friendly interface for API interaction.

### Authentication and Authorization
- **JWT (JSON Web Tokens)**: For secure transmission of information between parties.

### Security
- **Data Encryption**: Password hashing using `Microsoft.AspNet.Identity.IPasswordHasher`.

### API Versioning
This API uses the `Asp.Versioning.Mvc` library for header-based versioning. Users can specify API versions using the `x-api-version` header.

#### Example Usage:
```sh
curl -X GET "localhost:8080/api/cities" -H "x-api-version: 1.0"
```
If no version is specified, the API defaults to the latest version.

## Setup Guide
This guide provides instructions on setting up an existing ASP.NET API project.

### Prerequisites
- **.NET 8 SDK** installed on your system.
- Running **SQL Server** instance with a database.

### Step-by-Step Guide
1. **Clone the Repository**
   ```sh
   git clone https://github.com/mahmoudqasem2001/Travel-And-Accommodation-Booking-Platform.git
   ```

2. **Navigate to the Project Directory**
   ```sh
   cd AccommodationBookingPlatform\AccommodationBookingPlatform.Api
   ```

3. **Configure appsettings.json**
   Update the `appsettings.json` file with your SQL Server connection string:
   ```json
   {
     "ConnectionStrings": {
       "SqlServer": "<your_connection_string>"
     }
   }
   ```

4. **Run the API Locally**
   ```sh
   dotnet run
   ```

   The API will be accessible at `http://localhost:8080`.

   The Swagger UI can be accessed at `http://localhost:8080/swagger`.

   5. **Admin Credintials**
  
      - Email: admin@bookinghotel.com
      - Password: Admin1234
      


