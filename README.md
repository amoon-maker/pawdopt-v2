# 🐾 Pawdopt v2 — Pet Adoption & Rehoming Management System

> A modern, trustworthy web platform connecting animals with loving homes across Quebec.

---

## 📌 About the Project

**Pawdopt v2** is a full-stack web application built for animal shelters and rescue organizations in Quebec. It centralizes pet discovery, adoption applications, rehoming submissions, and document verification in one secure, bilingual platform — replacing fragmented email and social media workflows with a structured, transparent digital system.

This project is developed as part of **System Development (420-940-VA)** at **Vanier College**, Summer 2026.

---

## 👥 Team

| Name | Student ID | Role |
|------|-----------|------|
| Isabela Sierra | 6358655 | Rehome side — Rehome wizard, Rehomer dashboard |
| Amina Ayad Al Halabieh | 6367025 | Adopt side — Pet listings, Adoption wizard, Favourites |

> Shared: Landing page, Authentication, Admin panel, Care guides
> Instructor: **Pejman Azadi**

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend | C# ASP.NET Core MVC |
| Database | SQL Server + Entity Framework Core |
| Frontend | Razor Views, HTML5, CSS3 |
| Auth | ASP.NET Core Identity |
| Version Control | GitHub |
| IDE | Visual Studio Code |

---

## ✨ Features

### 👤 User Roles
- **Guest** — Browse pet listings freely without an account
- **Adopter** — Register, apply for pets, manage favourites and applications
- **Rehomer** — List pets with required health documents, manage incoming requests
- **Administrator** — Review and approve listings, manage users, oversee platform

### 🔑 Authentication
- Registration with role selection (Adopter or Rehomer)
- 6-digit email verification on signup
- Role-based redirection after login
- Protected routes — unauthenticated users redirected to login

### 🐶 Adopt Side *(Amina)*
- Browse and filter pet listings by species, breed, age, and location
- Pet detail pages with full info and call to action
- Multi-step adoption application wizard (5 steps)
- Adopter favourites list with persistent storage

### 🏠 Rehome Side *(Isabela)*
- Multi-step rehome listing wizard (6 steps)
- Mandatory vaccination/health document upload before submission
- Rehomer dashboard with listing status (Pending / Live / Rejected)
- Adoption request management with notification badges

### ⚙️ Admin Panel
- Review and approve or reject pet listings and documents
- User management

### 🌐 Global Features
- EN / FR language toggle on all pages
- Dark / Light mode switcher
- Responsive design (375px mobile → 1440px desktop)
- Floating support chat bot button

---

## 🗄️ Database Schema
Users               — Id, FirstName, LastName, Email, PasswordHash, Role, CreatedAt
Pets                — Id, Name, Type, Breed, Age, Gender, Size, Location, Description, PhotoUrl, Status, RehomerId
Applications        — Id, PetId, AdopterId, Status, SubmittedAt
RehomeSubmissions   — Id, PetId, OwnerId, Reason, SubmittedAt (draft per step)
Documents           — Id, PetId, OwnerId, DocumentType, FileUrl, VerificationStatus, UploadedAt
Favourites          — Join table: UserId ↔ PetId

---

## 🗺️ Pages

| Page | Description |
|------|-------------|
| Landing Page | Home screen — introduces Pawdopt |
| Listings Page | Browse all available pets with filters |
| Pet Detail Page | Full pet info + adoption CTA |
| Adopt Wizard | 5-step adoption application |
| Rehome Wizard | 6-step rehome listing with document upload |
| Adopter Dashboard | Applications, favourites, messages |
| Rehomer Dashboard | Listings status, adoption requests |
| Admin Panel | Document review, user management |
| Care Guide | Articles for dogs and cats |
| About / Contact | Platform info and contact details |
| FAQ | Frequently asked questions |
| 404 Page | Custom not-found page |

---

## 📅 Deliverables

| Milestone | Week | Scope |
|-----------|------|-------|
| Deliverable I | Week 5 | Auth, guest browsing, adoption wizard, rehome wizard steps 1–3 |
| Deliverable II | Week 7 | Document verification, rehomer dashboard, admin review |
| Deliverable III | Week 11 | Messaging, notifications, full admin panel, polish |

---

## 🎨 Design

- **Primary color:** `#1A6B8A` (teal)
- **Background:** `#F5F7F9` (light grey)
- **Typography:** Clean, accessible, bilingual (EN/FR)
- **Layout:** Card-based pet listings, step progress bars on all wizards

---

## 🚀 Getting Started

```bash
# Clone the repository
git clone https://github.com/YOUR-REPO-URL

cd pawdopt-v2

dotnet restore

dotnet ef database update

dotnet run
```

---

## 📄 License

This project is developed for academic purposes at Vanier College. All rights reserved © 2026 Isabela Sierra & Amina Ayad Al Halabieh.
