

\# SmartShopAI - AI-Powered Smart Shopping System



\## 1. Project Overview



SmartShopAI is an ASP.NET Core Web API application

integrating Google Gemini AI to support product

description generation, customer review sentiment

analysis, and AI-powered shopping assistance.



\## 2. Technologies



\- C# / ASP.NET Core Web API (.NET 10)

\- Entity Framework Core

\- Microsoft SQL Server

\- Google Gemini API

\- RESTful API

\- HTML, CSS, JavaScript

\- Visual Studio 2026



\## 3. Main Features



\### Product Management

\- Retrieve products from SQL Server

\- View product information



\### AI Product Description

\- Generate product descriptions using Gemini AI

\- Save generated descriptions to the database



\### AI Review Analysis

\- Analyze customer review sentiment

\- Classify reviews as Positive, Neutral, or Negative

\- Store AI analysis results



\### AI Shopping Assistant

\- Accept customer questions

\- Generate AI-powered shopping responses



\### Review Analytics

\- Calculate average product ratings

\- Count positive, neutral, and negative reviews

\- Display product review statistics



\## 4. Project Structure



SmartShopAI/

\- Controllers/

\- Data/

\- DTOs/

\- Models/

\- Services/

\- wwwroot/

\- Program.cs

\- appsettings.json

\- SmartShopAI.http



\## 5. Database



Database: SMARTSHOP



Main tables:

\- Product

\- Review



Database access is implemented using

Entity Framework Core and SQL Server.



\## 6. Configuration



The application requires:



\- .NET 10 SDK

\- SQL Server

\- A valid Google Gemini API key



Configure the SQL Server connection string

in appsettings.json or user secrets.



Configure the Gemini API key using

.NET User Secrets:



dotnet user-secrets set "Gemini:ApiKey" "YOUR\_API\_KEY"



Configure a Gemini model that is available

to your Google AI Studio account:



dotnet user-secrets set "Gemini:Model" "YOUR\_MODEL\_ID"



Do not commit API keys or passwords to GitHub.



\## 7. How to Run



1\. Clone the repository.

2\. Open SmartShopAI.sln or the solution file

&#x20;  in Visual Studio.

3\. Restore NuGet packages.

4\. Configure SQL Server and Gemini API credentials.

5\. Build the solution.

6\. Run the application using HTTPS.



\## 8. API Endpoints



| Method | Endpoint | Description |

|--------|----------|-------------|

| GET | /api/test/products | Get products |

| POST | /api/products/1/generatedescription | Generate AI product description |

| POST | /api/reviews/analyze | Analyze customer review |

| POST | /api/assistant/chat | AI shopping assistant |

| GET | /api/reports/reviews | Product review analytics |



\## 9. Sample Requests



\### Analyze Review



POST /api/reviews/analyze



{

&#x20; "productId": 1,

&#x20; "customerName": "Nguyen Van A",

&#x20; "content": "Excellent camera and performance",

&#x20; "rating": 5

}



\### AI Assistant



POST /api/assistant/chat



{

&#x20; "sessionId": "session001",

&#x20; "message": "Which phone has the best camera?"

}



\## 10. Notes



\- AI features require an active Gemini API key.

\- Gemini responses depend on model availability.

\- Some API calls may fail temporarily due to

&#x20; external service limits or high demand.

\- SQL Server must be configured before running.



\## 11. Author



SmartShopAI - ASP.NET Core and Gemini AI Project



Academic project for learning AI integration

with Web API and SQL Server.



