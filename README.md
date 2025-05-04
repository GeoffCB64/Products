**Products API**<br>
• Demonstrates a Minimal Web API in .NET Core.<br>
<br>
**Usage**<br>
• See document ProductsWebApiOverview.docx for a guide on using the API and for screenshots on each stage working.<br>
• Health check requires no authorisation.<br>
• Login requires a username and password to retrieve a bearer token which must be added to Swagger or Postman.  Usernames:<br>
&nbsp;&nbsp;&nbsp;&nbsp;username1 pass123<br>
&nbsp;&nbsp;&nbsp;&nbsp;username2 letmein<br>
&nbsp;&nbsp;&nbsp;&nbsp;admin     adminpass<br>
• Once authorized in Swagger or Postman, products can be added and then retreieved.<br>
<br>
**Technologies used**<br>
• .NET 8<br>
• JWT Authentication<br>
• ER-Core in-memory database<br>
• xUnit for unit and integration tests<br>
• Docker (Linux container)<br>
• Swagger UI<br>
<br>
**Features**<br>
• Health check endpoint<br>
• Add products<br>
• Filter products by colour<br>
• JWT-protected endpoints for managing products<br>
• Login endpoint for token generation<br>
• Includes unit and integration tests<br>
• Docker support for containerized deployment<br>
