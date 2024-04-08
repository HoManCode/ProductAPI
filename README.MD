# Product API

This repository contains an ASP.NET Core Product API implementing CRUD operations for managing products. The API adheres to best practices, utilizes appropriate design patterns, and includes unit tests.

## Instructions

1. **Clone Repository**: 
- Clone this repository to your local machine.

    ```bash
    git clone https://github.com/HoManCode/ProductAPI.git
    ```

2. **Run API**: 
- Navigate to the project directory.
- Build and run the API using the following commands:


    ```bash
    cd ProductAPI
    dotnet run
    ```
3. **Test API**:
- Use a tool like Postman or curl to test the API endpoints.
- Ensure to follow the API documentation provided below for endpoint details.

4. **Unit Tests**:

- Execute unit tests using the following command:

    ```bash
    cd ProductAPI
    dotnet test
    ```
## API Documentation
GET /api/products?MinPrice=v&MaxPrice=v&Page=v&Size=v: Retrieves requested products.<br>GET /api/products/{id}: Retrieves a product by its ID.<br>POST /api/products: Creates a new product.<br>PUT /api/products/{id}: Updates an existing product.<br>DELETE /api/products/{id}: Deletes a product by its ID.

## Technologies Used
Framework (ASP.NET Core C#)<br>Database Operations (Entity Framework Core) <br>Unit Testing (xUnit, Moq)<br>API Documentation (Swagger)<br>Logging (Microsoft.Extensions.Logging)

## Contributing
Contributions are welcome! If you find any issues or have suggestions, feel free to open an issue or create a pull request.

## License
This project is licensed under the MIT License.