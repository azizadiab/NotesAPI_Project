# Notes API 📝

A clean and secure **ASP.NET Core Web API** for managing notes with authentication, authorization, pagination, and search.

---

## 🚀 Features

- User Authentication & Authorization (JWT)
- Role-based access (Admin / User)
- CRUD operations for Notes
- Pagination (GetAllNotes / GetMyNotes)
- Search Notes (Title & Content)
- Global Exception Handling (Middleware)
- Unified API Response
- Clean Architecture (API / Business / Data / Security)
- SQL Server + Stored Procedures

---

## 🏗 Project Architecture

NotesAPI
│
├── NotesAPI → API Layer (Controllers, Middleware)
├── NotesAPI_Business → Business Logic
├── NotesAPI_Data → Data Access (ADO.NET + Stored Procedures)
├── NotesAPI_Security → Security (Hashing / JWT)


---

## 🔐 Authentication & Authorization

- JWT-based authentication
- Claims-based authorization
- Admin-only endpoints protected using `[Authorize(Roles = "Admin")]`

---

## 📄 API Response Format

All endpoints return a unified response format:

```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": {}
}
Errors are handled globally using Global Exception Middleware.
📚 Main Endpoints (Examples)

Notes

GET /api/notes/my?pageNumber=1&pageSize=10

GET /api/notes/all?pageNumber=1&pageSize=10 (Admin)

GET /api/notes/search?userId=1&search=note

POST /api/notes

PUT /api/notes/{id}

DELETE /api/notes/{id}

🛠 Technologies Used

C#

ASP.NET Core Web API

ADO.NET

SQL Server

Stored Procedures

JWT Authentication

Git & GitHub

📌 Future Improvements

Sorting

Refresh Tokens

Unit Testing

Docker support

👩‍💻 Author

Aziza Abdel Hamid Diab
GitHub: https://github.com/azizadiab


📌 ده README **مناسب للتوظيف** فعلًا.

---

# 2️⃣ GitHub Action (CI بسيط)

ده بيعمل:
- Build للمشروع
- يتأكد إن الكود بيشتغل

---

## 📁 إنشاء GitHub Action

### الخطوات:
1. في المشروع اعملي فولدر:

