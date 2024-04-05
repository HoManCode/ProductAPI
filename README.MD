# Product API

This repository contains a .NET Core Product API implementing CRUD operations for managing products. The API adheres to best practices, utilizes appropriate design patterns, and includes unit tests.

## Instructions

1.**Clone Repository**: 
Clone this repository to your local machine.

    
    git clone https://github.com/HoManCode/ProductAPI.git

2.**Run API**: 
- Navigate to the project directory.
- Build and run the API using the following commands:


    ```bash
    cd ProductAPI
    dotnet run

3.**Test API**:
- Use a tool like Postman or curl to test the API endpoints.
- Ensure to follow the API documentation provided below for endpoint details.

4.**Unit Tests**:

Execute unit tests using the following command:

    dotnet test

## API Documentation
GET /products: Retrieves all products.<br>GET /products/{id}: Retrieves a product by its ID.<br>POST /products: Creates a new product.<br>PUT /products/{id}: Updates an existing product.<br>DELETE /products/{id}: Deletes a product by its ID.

## Technologies Used
C# .NET 8 Core<br>Entity Framework Core (Database Operations)<br>xUnit (Unit Testing)<br>Swagger (API Documentation)<br>Microsoft.Extensions.Logging (Logging)

## Contributing
Contributions are welcome! If you find any issues or have suggestions, feel free to open an issue or create a pull request.

## License
This project is licensed under the MIT License.