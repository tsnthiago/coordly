# ProductAPI

## Project Description

This project is a RESTful API developed with **ASP.NET Core 8** and **Entity Framework Core 8**, providing CRUD operations (Create, Read, Update, Delete) for managing products. The application follows good architecture practices, with a clear separation of concerns between **Models**, **Repositories**, and **Controllers**.

## Features

- Full product management (create, list, update, delete)
- Pagination in the product listing
- Input data validation
- Global exception handling
- Interactive documentation with **Swagger**
- Unit and integration tests

## Technologies Used

- **ASP.NET Core 8.0**
- **Entity Framework Core 8.0**
- **SQLite** (database)
- **Swagger/OpenAPI** for documentation
- **xUnit** for unit and integration tests
- **Moq** for mocking in unit tests

## Project Structure
```

ProductAPI/
├── Controllers/
│ └── ProductsController.cs
├── Middleware/
│ └── GlobalExceptionHandlingMiddleware.cs
├── Models/
│ ├── AppDbContext.cs
│ ├── Product.cs
│ └── ProductsResponse.cs
├── Repositories/
│ ├── IProductRepository.cs
│ └── ProductRepository.cs
├── Migrations/
├── Program.cs
├── appsettings.json
├── ProductAPI.csproj
└── ProductAPI.http
ProductAPI.Tests/
├── ProductsControllerTests.cs
└── ProductAPI.Tests.csproj
ProductAPI.IntegrationTests/
├── CustomWebApplicationFactory.cs
├── ProductApiIntegrationTests.cs
├── TestOperation.cs
└── ProductAPI.IntegrationTests.csproj

````

## Prerequisites
- **.NET 8.0 SDK**
- A code editor (recommended: **Visual Studio Code** or **Visual Studio**)

## How to Run the Project Locally

1. Clone the repository:
   ```bash
      git clone https://github.com/your-username/ProductAPI.git
      cd ProductAPI
   ````

2. Restore the dependencies:

   ```bash
   dotnet restore
   ```

3. Apply the migrations to create the database:

   ```bash
   cd ProductAPI
   dotnet ef database update
   ```

4. Run the project:

   ```bash
   dotnet run
   ```

   The API will be available at:

   - https://localhost:5001
   - http://localhost:5000

## Accessing Swagger

After starting the application, access the **Swagger UI** at:

- https://localhost:5001/swagger
- http://localhost:5000/swagger

## Running the Tests

### Unit Tests:

```bash
dotnet test ProductAPI.Tests/ProductAPI.Tests.csproj
```

### Integration Tests:

```bash
dotnet test ProductAPI.IntegrationTests/ProductAPI.IntegrationTests.csproj
```

## API Endpoints

- `GET /api/products` — List all products (with pagination)
- `GET /api/products/{id}` — Get a specific product
- `POST /api/products` — Create a new product
- `PUT /api/products/{id}` — Update an existing product
- `DELETE /api/products/{id}` — Remove a product

### Product Model

```json
{
  "productID": 0,
  "name": "string",
  "price": 0,
  "stockQuantity": 0
}
```

- `productID`: Automatically generated
- `name`: Required, maximum of 100 characters
- `price`: Required, must be greater than zero
- `stockQuantity`: Required, must be greater than or equal to zero

## Error Handling

The API uses a **global middleware** for exception handling, ensuring consistent responses in case of errors.

## Database Configuration

The project uses **SQLite** as the database. The connection string is configured in the `appsettings.json` file. To switch to another database, modify the connection string and install the appropriate provider.

## API Documentation

The full API documentation is available through **Swagger UI**. Access `/swagger` after starting the application to view all endpoints, data models, and test operations.

## Tests

- **Unit Tests**: Located in `ProductAPI.Tests`, focusing on testing the controller logic in isolation.
- **Integration Tests**: Located in `ProductAPI.IntegrationTests`, testing the full flow of the API, including interaction with an in-memory database.

To add new tests, follow the existing pattern in the test files.

## Contributing

1. Fork the project
2. Create a branch for your feature:
   ```bash
   git checkout -b feature/AmazingFeature
   ```
3. Commit your changes:
   ```bash
   git commit -m 'Add some AmazingFeature'
   ```
4. Push to the branch:
   ```bash
   git push origin feature/AmazingFeature
   ```
5. Open a Pull Request

## Best Development Practices

- Maintain the separation of concerns between controllers, repositories, and models.
- Use **dependency injection** for loose coupling between components.
- Write tests for new features and maintain the existing test coverage.
- Follow the naming conventions of C# and .NET.
- Document new endpoints in Swagger.

## Troubleshooting

- **Migration issues**: If you encounter issues with migrations, try deleting the `Migrations` folder and the database file, then recreate the migrations with:
  ```bash
  dotnet ef migrations add InitialCreate
  ```
- **Compilation issues**: Verify all dependencies are installed correctly with `dotnet restore`.
- **Test failures**: Check if the in-memory database is being properly configured in the integration tests.

## License

This project is licensed under the MIT License. See the LICENSE file for more details.

## Contact

For questions, suggestions, or contributions, please open an issue in the GitHub repository or contact via [thiago_tsn@live.com].

## Acknowledgements

Special thanks to all contributors and the open-source community for the frameworks and tools that made this project possible.
