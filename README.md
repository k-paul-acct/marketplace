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
```json
"User"
{
  "UserName": "john_doe",
  "FirstName": "John",
  "LastName": "Doe",
  "Email": "john@example.com",
  "PasswordHash": "secure_password",
  "Phone":"88006565665"
  "RoleId": 2,
  "ImageURL" : "https://i.stack.imgur.com/ILTQq.png"
}

"Product"
{
  "Name": "Laptop Model XYZ",
  "Description": "Powerful laptop for all your needs",
  "CategoryId": 3,
  "Price": 999.99,
  "ImageURL": "https://example.com/laptop.jpg",
  "StockQuantity" : 100,
  "SellerUserID" : 2
}

"Order"
{
  "UserId": 1,
  "OrderDate": "2023-08-26T10:00:00",
  "TotalAmount": 499.99
}

 "OrderItem"
{
  "OrderId": 1, 
  "ProductId": 2, 
  "Quantity": 2,
  "UnitPrice": 249.99
}

 "Review"
{
  "ProductId": 3, 
  "UserId": 4,
  "Rating": 5,
  "Comment": "Great product! Highly recommended."
  "ImageURL" : "https://i.stack.imgur.com/ILTQq.png"
}

 "Category"
{
  "Name": "Laptops",
  "Description": "Category for various laptop models"
}

"Wishlist"
{
  "UserId": 1, 
  "ProductId": 2
}
```
