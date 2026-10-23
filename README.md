# SmartShopAI

## 1. Introduction

SmartShopAI is a simple shopping application developed using ASP.NET Core Web API and Google Gemini AI.

The application helps users generate product descriptions, analyze customer reviews, and ask questions about products.

## 2. Technologies Used

- C# and ASP.NET Core Web API (.NET 10)
- Entity Framework Core
- SQL Server
- Google Gemini AI
- HTML, CSS, JavaScript
- Visual Studio 2026

## 3. Main Features

**1. Generate Product Description**

- Use Gemini AI to generate a product description of about 80 words.
- Save the description to the database.

**2. Analyze Customer Reviews**

- Analyze customer reviews using Gemini AI.
- Classify reviews as Positive, Negative, or Neutral.
- Save sentiment, score, and summary to SQL Server.

**3. AI Shopping Assistant**

- Answer customer questions about products.
- Remember previous questions using a session ID.
- Provide a simple chat interface.

**4. Review Reports**

- Show total reviews and average ratings.
- Count positive, negative, and neutral reviews.
- Display product statistics on a dashboard.

## 4. Database

Database name: `SMARTSHOP`

Main tables:
- Product
- Review

The application uses Entity Framework Core to connect to SQL Server.

## 5. How to Run

1. Open the project in Visual Studio.
2. Configure the SQL Server connection string.
3. Add the Gemini API Key using .NET User Secrets.
4. Build and run the project.

## 6. API Endpoints

| Method | API | Description |
|---|---|---|
| GET | `/api/test/products` | Get products |
| POST | `/api/products/{id}/generatedescription` | Generate product description |
| POST | `/api/reviews/analyze` | Analyze customer review |
| POST | `/api/assistant/chat` | Chat with AI |
| GET | `/api/reports/reviews` | Get review statistics |

## 7. Web Pages

**Review Dashboard:**

`https://localhost:7126/reports.html`

This page shows product reviews, average ratings, and sentiment statistics.

**AI Shopping Assistant:**

`https://localhost:7126/chat.html`

This page allows users to chat with Gemini AI about products.

## 8. Gemini AI Prompts

**Product Description:**

Gemini acts as a marketing copywriter and creates an attractive product description of about 80 words.

**Review Analysis:**

Gemini analyzes customer reviews and returns JSON containing sentiment, score, and summary.

**Shopping Assistant:**

Gemini answers customer questions using product information from the database and conversation history.

## 9. Notes

- A valid Gemini API Key is required.
- Do not upload API Keys or passwords to GitHub.
- Gemini responses may take some time depending on the API service.

## 10. Author

SmartShopAI - ASP.NET Core and Gemini AI Practical Exam Project.
