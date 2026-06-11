# Pawdopt v2 — Project Summary

**Team:** Isabela Sierra & Amina  
**Tech stack:** ASP.NET Core MVC (.NET 10), Entity Framework Core, ASP.NET Identity, SQL Server (Docker)

---

## What is Pawdopt?

Pawdopt is a web platform that connects people who need to find a new home for their pet (rehomers) with people who want to adopt one. The goal is to make the process safer, more structured, and less stressful for everyone involved — including the animal.

---

## Features Overview

### Authentication (Amna)
- Users can **register** as either an Adopter or a Rehomer
- Secure **login and logout** using ASP.NET Identity
- Passwords are hashed automatically — never stored in plain text
- Role-based access: Adopter, Rehomer, Admin

---

### Landing Page & Navigation (Isabela)
- Main landing page introducing the platform
- Navigation between all sections
- Bilingual support (English / French) via a language toggle — all visible text can be switched without reloading the page

---

### About Us Page (Isabela)
- Describes the mission and values of Pawdopt
- Sections: story, values, what the platform does, commitment card
- Two call-to-action links: one pointing to the Adopt flow, one to the Rehome flow
- Fully bilingual (English / French translations wired into the i18n system)

---

### Care Guide (Amna)
- Educational section with pet care tips
- Covers different types of pets and care scenarios

---

### Adopt Flow (Amna)

**Browse pets (`/Home/Adopt`)**
- Grid of available pets with filters (species, size, gender, location)
- Each pet card shows name, breed, age, location and a badge (New / Urgent)
- Users can save pets to a favourites list (stored locally per browser)

**Pet Detail page (`/Home/PetDetail`)**
- Full profile of a pet: photos, personality traits, health info, story
- Lifestyle compatibility tags
- Button to start the adoption application

**Adoption Wizard (`/Home/AdoptionWizard`)**
- Multi-step form collecting information from the applicant:
  - Personal details (name, contact, location)
  - Living situation (home type, ownership, outdoor space)
  - Household (people, children, other pets)
  - Experience with pets
  - Hours alone, activity level
  - Notes
- Submitted applications are saved to the database
- Confirmation notification is sent to the adopter automatically

---

### Rehome Flow (Isabela)

**Landing page (`/Rehome/List`)**
- Explains the rehoming process
- Advisor profiles (Sophie, Marc) that guide the user before starting

**Multi-step listing wizard:**

| Step | Page | What it collects |
|------|------|-----------------|
| 1 | New Listing | Pet name, species, breed, age, gender, size, microchip, reason for rehoming |
| 2 | Images | Pet photos |
| 3 | Character | Personality traits, vaccinations, sterilized, activity level, ideal home |
| 4 | Key Facts | Health notes, training level |
| 5 | Location | City, province, postal code, pickup preferences |
| 6 | Story | Personal story about the pet in their own words |
| 7 | Documents | Vet records or supporting documents |
| 8 | Confirm & Publish | Review everything before submitting |

- Each step saves progress to the database (Draft status)
- On publish, the listing goes to **Pending** and admins are notified automatically
- Rehomers can see all their listings in their profile

---

### User Profile (Isabela)

The profile page (`/Home/AdopterProfile`) has five tabs:

**Overview tab**
- Stats: number of applications sent, pets saved, profile completion %
- Recent activity feed (applications submitted, listings created)

**My Applications tab**
- Full list of adoption applications with status (Pending, Under Review, Approved, Rejected, Withdrawn)
- Adopters can withdraw a pending application

**My Listings tab** *(Rehomer only)*
- All listings created by the user with their current status
- Link to continue any listing still in Draft

**Saved Pets tab**
- Pets the user hearted on the Adopt page
- Stored in browser localStorage, rendered dynamically

**Settings tab** *(Isabela — implemented POST endpoints)*
- **Personal Information form:** updates display name, phone number, city, and province — saved to the database via `_userManager.UpdateAsync()`
- **Change Password form:** validates current password, checks that new and confirm passwords match, updates via `_userManager.ChangePasswordAsync()`
- Success and error feedback messages shown inline after submitting
- After saving, the Settings tab stays open automatically (no need to re-navigate)

---

### Messaging System (both)
- Adopters and rehomers can message each other
- Inbox shows all conversations grouped by contact
- Conversation view shows full message history
- Sending a message creates a notification for the receiver
- Messages are marked as read when the conversation is opened

---

### Notifications (both)
- Bell icon in the navigation shows unread count (updates automatically)
- Notifications are created automatically for:
  - Application submitted (adopter receives)
  - Application status changed (adopter receives)
  - New message received
  - Listing approved or rejected (rehomer receives)
  - New listing submitted (admin receives)
- Users can mark individual notifications or all as read

---

### Admin Panel (both)
- Only accessible to users with the Admin role
- **Listings moderation:** approve or reject pending listings, add notes
- **Application management:** change the status of any adoption application
- All admin actions trigger a notification to the affected user

---

## Database Models

| Model | Description |
|-------|-------------|
| `ApplicationUser` | Extends Identity — adds DisplayName, UserType, City, Province, PhoneNumber |
| `PetListing` | All fields for a rehome listing (steps 1–8) |
| `AdoptionApplication` | Full adoption application form data |
| `Message` | Individual message between two users |
| `Notification` | In-app notification tied to a user |

---

## What Each Person Built

### Isabela
- Landing page and general navigation structure
- About Us page (content, layout, bilingual translations)
- Rehome wizard (all 8 steps, controller and views)
- User profile page (all tabs, layout)
- Profile settings save — **Update Profile** and **Change Password** (POST endpoints + form wiring)
- README and project documentation

### Amina
- Initial project setup and architecture
- Adopt page (pet grid, filters, favourites)
- Pet Detail page
- Adoption Wizard (multi-step form, submission to DB)
- Care Guide
- Logo and dark mode assets

