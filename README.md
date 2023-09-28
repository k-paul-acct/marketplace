# API for MarketplaceDB
<ul>
<li>Use the database dump script.sql to create the database.</li>
<li>The API listens on localhost:8080 by default, and you can change this value in Program.cs (line 16)</li>
</ul>

## Methods
There Are 6 types of methods:
<ul>
<li>POST: for /updatebyid/id and /getbyfields </li>
<li>PUT: for /create </li>
<li>DELETE: for /deletebyid/id </li>
<li>GET: for /getbyid/id and getall </li>
</ul>

Examples
// Пример структуры данных для сущности "User"
{
  "UserName": "john_doe",
  "FirstName": "John",
  "LastName": "Doe",
  "Email": "john@example.com",
  "Password": "secure_password",
  "Role": "User"
}

// Пример структуры данных для сущности "Product"
{
  "Name": "Laptop Model XYZ",
  "Description": "Powerful laptop for all your needs",
  "Category": "Laptops",
  "Price": 999.99,
  "ImageURL": "https://example.com/laptop.jpg"
}

// Пример структуры данных для сущности "Order"
{
  "UserId": 1, // Идентификатор пользователя, который разместил заказ
  "OrderDate": "2023-08-26T10:00:00",
  "TotalAmount": 499.99
}

// Пример структуры данных для сущности "OrderItem"
{
  "OrderId": 1, // Идентификатор заказа, к которому относится элемент заказа
  "ProductId": 2, // Идентификатор продукта в заказе
  "Quantity": 2,
  "UnitPrice": 249.99
}

// Пример структуры данных для сущности "Review"
{
  "ProductId": 3, // Идентификатор продукта, к которому оставлен отзыв
  "UserId": 4, // Идентификатор пользователя, оставившего отзыв
  "Rating": 5,
  "Comment": "Great product! Highly recommended."
}

// Пример структуры данных для сущности "Category"
{
  "Name": "Laptops",
  "Description": "Category for various laptop models"
}

// Пример структуры данных для сущности "Wishlist"
{
  "UserId": 1, // Идентификатор пользователя, создавшего список желаний
  "ProductIds": [1, 2, 3] // Список идентификаторов продуктов в списке желаний
}
