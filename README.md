# 🛍️ E-Commerce Platform

A full-stack e-commerce web application built as an ITI  project. Features a modern Angular frontend, ASP.NET Core 8 Web API backend, and SQL Server database. Supports customers, sellers, and admins with complete shopping flow including guest checkout, wallet payments, promo codes, and email notifications.

---

## 📋 Table of Contents

- [Features](#-features)
- [Tech Stack](#-tech-stack)
- [Live Demo](#-live-demo)
- [Screenshots](#-screenshots)
- [Project Structure](#-project-structure)
- [Getting Started](#-getting-started)
- [API Documentation](#-api-documentation)
- [Test Accounts](#-test-accounts)
- [Team](#-team)
- [License](#-license)

---

## ✨ Features

### 👤 User Management
- Email & phone registration with confirmation emails
- Login with JWT authentication
- Forgot password & reset via secure email links
- Multi-role system (Customer, Seller, Admin)
- Profile management (personal info, addresses, password, wallet)
- Soft delete for user accounts (admin)

### 🛍️ Shopping Experience
- Product browsing with images, descriptions, and prices
- Categories and subcategories
- Real-time stock availability tracking
- Search by name with filtration (price, category, rating)
- Sorting (newest, price low-to-high, price high-to-low, rating)
- Pagination for product lists
- Product reviews and ratings

### 🛒 Cart & Checkout
- Add/remove items with quantity adjustment
- Persistent cart for logged-in users
- **Guest checkout option** (no account required)
- Multiple payment methods:
  - 💵 Cash on Delivery
  - 💳 Credit Card
  - 🅿️ PayPal
  - 👛 Wallet (in-app balance)
- Promo codes with discount validation
- Order summary with full price breakdown (subtotal, shipping, tax, discount, total)
- Free shipping for orders over $500

### 📦 Order Management
- Order placement with email confirmation
- Order history with detailed view
- Order tracking with status updates (Pending → Processing → Shipped → Delivered)
- Wishlist & favorites

### 👨‍💼 Admin Panel
- Dashboard with statistics (revenue, orders, users, monthly sales)
- User management (approve, restrict, soft delete)
- Product & category CRUD
- Order management with status updates
- Promo code management
- Banner & homepage content management

### 🏪 Seller Panel
- Seller registration & profile setup
- Product listing & inventory management
- Order processing
- Earnings overview

### 🎨 UI/UX
- 🌗 **Dark mode / Light mode** with system preference detection
- Responsive design (mobile, tablet, desktop)
- Modern, clean interface with smooth animations
- Toast notifications for user feedback
- Loading states and skeleton screens

---

## 🛠️ Tech Stack

### Frontend
- **Angular 18+** (standalone components, signals)
- **TypeScript**
- **RxJS** for reactive state management
- **Angular Router** with guards
- **CSS variables** for theming

### Backend
- **ASP.NET Core 8** Web API
- **Entity Framework Core** (Code First)
- **JWT Bearer Authentication**
- **BCrypt** for password hashing
- **SMTP** (Gmail) for email notifications

### Database
- **Microsoft SQL Server** (2019/2022)

### Architecture
- **Repository Pattern** with `IGenericRepository<T>`
- **Service Layer** for business logic
- **DTO Pattern** for data transfer
- **Dependency Injection** throughout

### Deployment
- **Frontend:** MonsterASP.NET / Vercel
- **Backend:** runasp.net / Render
- **Database:** Somee.com / Azure SQL

---

## 🌐 Live Demo

| Component | URL |
|-----------|-----|
| **Frontend** | [https://ecommerce129angular.runasp.net](https://ecommerce129angular.runasp.net) |
| **Backend API** | [https://ecommerce129.runasp.net](https://ecommerce129.runasp.net) |
| **Swagger Docs** | [https://ecommerce129.runasp.net/swagger](https://ecommerce129.runasp.net/swagger) |

---

## 📁 Project Structure

```
Ecommerce-Project/
├── backend/
│   └── backend/
│       ├── Controllers/         # API endpoints
│       │   ├── AuthController.cs
│       │   ├── ProductsController.cs
│       │   ├── CartController.cs
│       │   ├── OrdersController.cs
│       │   ├── AdminController.cs
│       │   ├── SellerController.cs
│       │   └── ...
│       ├── Services/            # Business logic
│       ├── Repositories/        # Data access (Interfaces + Implementations)
│       ├── Models/              # EF entities
│       ├── DTOs/                # Data transfer objects
│       ├── Data/                # AppDbContext
│       ├── Helpers/             # JwtHelper, EmailHelper
│       ├── Migrations/          # EF migrations
│       ├── Program.cs
│       └── appsettings.json
│
└── frontend/
    └── src/
        ├── app/
        │   ├── core/
        │   │   ├── guards/      # authGuard, adminGuard, sellerGuard
        │   │   ├── interceptors/ # auth, error, loading
        │   │   ├── models/      # TS interfaces
        │   │   └── services/    # HTTP services
        │   ├── features/
        │   │   ├── auth/        # login, register, forgot password
        │   │   ├── home/        # landing page
        │   │   ├── products/    # list, details, search
        │   │   ├── cart/
        │   │   ├── checkout/    # with guest mode
        │   │   ├── orders/      # list, details, tracking
        │   │   ├── profile/     # info, addresses, wallet
        │   │   ├── wishlist/
        │   │   ├── admin/       # dashboard + management UIs
        │   │   └── seller/      # dashboard + product mgmt
        │   ├── shared/
        │   │   ├── components/  # header, footer, product-card, etc.
        │   │   ├── pipes/
        │   │   └── directives/
        │   ├── app.routes.ts
        │   └── app.component.ts
        ├── environments/
        └── styles.css
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) and npm
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB or full)
- [Angular CLI](https://angular.dev/) (`npm install -g @angular/cli`)

### Backend Setup

```bash
# Clone the repository
git clone https://gitlab.com/ahmeddabish-group/ecommerce-project.git
cd ecommerce-project/backend/backend

# Restore dependencies
dotnet restore

# Update connection string in appsettings.json
# "DefaultConnection": "Server=.;Database=angular_project;Trusted_Connection=True;..."

# Apply migrations to create the database
dotnet ef database update

# Run the backend
dotnet run --launch-profile https
```

The API will start at `https://localhost:7012` and `http://localhost:5085`.

### Frontend Setup

```bash
cd ../../frontend

# Install dependencies
npm install

# Start dev server
ng serve
```

The Angular app will start at `http://localhost:4200`.

### Configuration

Edit `backend/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=angular_project;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "YourSecretKey_AtLeast32CharsLong",
    "Issuer": "EcommerceBackend",
    "Audience": "EcommerceFrontend"
  },
  "Email": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-gmail-app-password",
    "From": "your-email@gmail.com"
  },
  "Frontend": {
    "BaseUrl": "http://localhost:4200"
  }
}
```

> ⚠️ **Important:** For Gmail SMTP, you need to use an [App Password](https://myaccount.google.com/apppasswords) — not your regular Gmail password.

---

## 📚 API Documentation

Once the backend is running, Swagger UI is available at:

```
https://localhost:7012/swagger
```

### Main Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/Auth/register` | Register new user |
| `POST` | `/api/Auth/login` | Login & get JWT |
| `POST` | `/api/Auth/forgot-password` | Send reset link |
| `POST` | `/api/Auth/reset-password` | Reset password with token |
| `GET` | `/api/Products` | List products (with filters) |
| `GET` | `/api/Products/{id}` | Product details |
| `GET` | `/api/Categories` | List categories |
| `POST` | `/api/Cart/items` | Add to cart |
| `POST` | `/api/Orders` | Place order (authenticated) |
| `POST` | `/api/Orders/guest` | Place guest order |
| `POST` | `/api/PromoCode/validate` | Validate promo code |
| `GET` | `/api/Admin/dashboard` | Admin stats (admin only) |
| `GET` | `/api/Seller/dashboard` | Seller stats (seller only) |

---

## 🔐 Test Accounts

The database is seeded with test accounts you can use right away:

| Role | Email | Password |
|------|-------|----------|
| **Admin** | admin@test.com | `Test@123` |
| **Seller** | seller@test.com | `Test@123` |
| **Customer** | customer@test.com | `Test@123` |

---

## 👥 Team

This project was built collaboratively by 3 developers:

| Developer | Responsibility |
|-----------|----------------|
| **Mohamed Ashraf** | Auth, User Management, Wishlist, Reviews, App Foundation |
| **Yasser Adel** | Products, Cart, Checkout, Orders, Payments, Promo Codes |
| **Ahmed Abdelaty** | Admin Panel, Seller Panel, Banners, Categories Management |

---

## 🎯 Key Achievements

- ✅ Complete end-to-end e-commerce flow
- ✅ Three distinct user roles with separate dashboards
- ✅ Guest checkout without forced registration
- ✅ Email notifications (confirmation, reset password)
- ✅ Wallet payment system
- ✅ Promo code system with validation
- ✅ Soft delete for data integrity
- ✅ Dark mode with system preference detection
- ✅ Fully responsive on all devices
- ✅ JWT-based authentication with role guards
- ✅ Deployed to production

---

## 🌟 Bonus Features Implemented

- ✅ Promo codes & discounts
- ✅ Wallet payments
- ✅ Guest checkout
- ✅ Order tracking
- ✅ Email notifications
- ✅ Dark/Light mode
- ✅ Soft delete

---

## 📸 Screenshots

> _Add screenshots of your key pages here:_
> - Home page (light + dark mode)
> - Product details
> - Cart & Checkout
> - Admin Dashboard
> - Seller Dashboard

---

## 🤝 Contributing

This is a graduation project. For any improvements or bug reports, please open an issue or contact the team.

### Development Workflow

- Each developer works on their own feature branch
- Branches: `feature/dev1-auth-user`, `feature/dev2-shopping`, `feature/dev3-admin-seller`
- All changes go through Pull Requests with code review
- Direct pushes to `main` are not allowed

---

## 📄 License

This project is built for educational purposes as part of the ITI (Information Technology Institute) graduation program.

---

## 🙏 Acknowledgments

- **ITI (Information Technology Institute)** for the curriculum and guidance
- **Angular Team** for the amazing framework
- **Microsoft** for ASP.NET Core
- All open-source libraries that made this possible

---

<div align="center">

**Built with ❤️ by the team**

Made as part of ITI  Project · 2026

</div>
