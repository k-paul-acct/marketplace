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
for localhost:8080/api/user/create  
{
  "FirstName": "John1132",
  "LastName": "Doe1132",
  "Email": "johndo11133e@example.com",
  "PasswordHash": "hashedPassword1111333333",
  "RoleId": 2
} \n (post)
