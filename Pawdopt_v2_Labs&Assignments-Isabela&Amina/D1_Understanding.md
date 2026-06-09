# Deliverable 1 — My Understanding

**Course:** System Development (420-940-VA) — Summer 2026  
**Team:** Amina Ayad Al Halabieh & Isabela Sierra  
**Project:** Pawdopt v2 — Pet Adoption & Rehoming Platform

---

## What Deliverable 1 Is Asking For

D1 is worth 6% and it's essentially a working prototype that proves our system can do the core things we promised in our user stories. It doesn't have to be perfect or have every single feature — it just needs to show that the structure works and that we understand what we're building.

The main things D1 requires:

1. **Guest browsing** — Anyone can browse and filter pets without needing an account
2. **Authentication** — Users can register, verify their email, and log in to the right dashboard
3. **Role-based dashboards** — Adopters and rehomers each land on their own dashboard after logging in
4. **Pet listing page** — A real listing page with filters
5. **Rehome wizard (steps 1–3)** — Rehomers can start listing a pet (Isabela's part)
6. **Document upload** — Vaccination proof can be uploaded (Isabela's part)
7. **Connected to a real database** — SQL Server in the background

---

## What We've Built So Far

### My part (Adopt side) — Amina

| Feature | Status | Notes |
|---------|--------|-------|
| Landing page | Hero, steps, featured pets, FAQ, footer |
| Adopt listing page  | Full filter sidebar, search, pagination |
| Pet detail page  | Story, traits, health badges, Apply button |
| Adoption wizard (5 steps) | Full form, validation, confirmation |
| Adopter profile/dashboard  | Tabs: overview, applications, saved pets, settings |

### Shared
| EN/FR language toggle | Some pages, both languages |
| Dark/light mode | Some persists across pages, logo changes too |
| Page routing | Some pages connected to each other |

### Isabela's part (Rehome side)

| Feature | Status | Notes |
|---------|--------|-------|
| Rehome landing page 
| Rehome wizard steps 1–3 
| Document upload 

### What's Still Missing for Full D1

| Missing | Why | Plan |
|---------|-----|------|
| SQL Server connection | Needed Docker setup — timing challenge | we will connect in D2 sprint |
| Real authentication (login/register) | Depends on DB being live | Build with DB in D2 |
| Email verification (6-digit code) | Also needs backend/email service | D2 |
| Real data from database | Currently using mock static data | Replace in D2 |

---

## How the System Works Right Now

Even without the database connected yet, the **front-end flow is complete and functional**:

```
Landing page
    ↓
[Adopt Now] or [See all pets]
    ↓
Adopt Listing page (with live filters, search, pagination)
    ↓
[More Info] on any pet card
    ↓
Pet Detail page (story, health, traits, rehomer info)
    ↓
[Apply to Adopt]
    ↓
Adoption Wizard (5 steps with validation)
    ↓
Confirmation page → "View My Applications" → Adopter Profile
```

The filters work in real-time in the browser. Favourites are saved in `localStorage`. The wizard validates each step before moving forward. Everything is bilingual and supports dark mode.

---

## Why the Database Isn't Connected Yet

This is an honest limitation to flag. We planned to use SQL Server with Docker, but getting Docker running consistently on both Mac (Amina) and Windows (Isabela) during the early weeks took longer than expected. This is a real-world infrastructure challenge and we documented it in our agile notes.

Our solution: complete the full front-end so that all the UI, routing, and logic is ready, then connect the database in D2 without having to redo any of the front-end work. The data models match exactly what's in our class diagram (Users, Pets, Applications, Documents tables).

---

## What Our System Demonstrates

Even in its current state, our prototype shows:

- **We understand the full user flow** — from browsing, to filtering, to applying, to tracking
- **Role separation** — The adopter side and rehomer side are clearly separated (and even different team members worked on each)
- **A complete adoption workflow** — The 5-step wizard collects everything a real adoption application would need
- **Professional UI** — Dark mode, bilingual, responsive on mobile, accessible
- **Agile collaboration** — We used separate Git branches, PR reviews for schema changes, and clear task ownership

---

## What's Next (D2 — Week 7)

- Connect SQL Server (Docker confirmed working on both machines)
- Build the real authentication (registration + login + role redirect)
- Wire the adopt wizard to actually save to the database
- Full rehome wizard (steps 4–8)
- In-platform messaging system
- Admin approval for listings

---

*Prepared by Amina & Isabela — June 2026*
