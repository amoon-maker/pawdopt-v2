# Pawdopt v2 — Current Project Status
**Audit date:** 2026-06-27  


## 1. Report Section Structure

> **D2 (Week 7 — Core features):** Adoption wizard · Full rehome wizard · User dashboards · CRUD operations · Messaging · Advanced filtering

---

## 2. Code Inventory (file:line citations)

### Models

| File | Key contents |
|---|---|
| `Models/ApplicationUser.cs:1` | `DisplayName`, `UserType`, `City`, `Province`, `CreatedAt` — extends `IdentityUser`. **No** `FirstName`, `LastName`, `VerificationCode`, or `VerifCodeExpiry` columns. |
| `Models/AdoptionApplication.cs:1` | FK `PetListingId` (required, NOT NULL after migration 2). FKs: `AdopterId → ApplicationUser`, `PetListingId → PetListing`. No `HardcodedPetId`, `PetName`, `FirstName`. |
| `Models/ApplicationStatus.cs:7` | Enum: `Pending, UnderReview, Approved, Rejected, Withdrawn, Cancelled`. Extension methods: `CanTransitionTo()`, `BadgeCss()`, `Label()`. |
| `Models/PetListing.cs:1` | Status stored as string: `"Draft"`, `"Pending"`, `"Approved"`, `"Rejected"`. No `"Live"`, `"Adopted"`, or `"Removed"` values. `DocumentsJson` (TEXT column, JSON array of strings) at line 47 — stores document-type keys, NOT file paths. `[NotMapped]` on `PhotoUrlsJson` only. |
| `Models/Notification.cs:1` | `UserId`, `Type`, `Title`, `Body`, `LinkUrl`, `IsRead`, `CreatedAt`. |
| `Models/Message.cs:1` | Soft-delete fields `DeletedForSender`, `DeletedForReceiver` (added in migration 3). |

### Services

| File | Key contents |
|---|---|
| `Services/ApplicationService.cs:20` | `ApplyStatusChangeAsync(app, newStatus, reviewNotes)` — single method used by both controllers. Line 23: `CanTransitionTo()` called first; returns `(false, errorMessage)` on invalid transition. Line 30: `BeginTransactionAsync()` wraps the approval path. Lines 37-42: queries competing Pending/UnderReview apps for same `PetListingId`. Line 46: sets competitors to `Cancelled`. Lines 70-71: `SaveChangesAsync()` + `CommitAsync()`. Line 73: `RollbackAsync()` on exception. |

### Controllers

| File | Key action(s) |
|---|---|
| `Controllers/HomeController.cs:21` | `RealPetIdOffset = 10000`. `GetApprovedPetDtosAsync()` filters `Status == "Approved"` at line 53. `AdoptionWizard(int petListingId)` at line 185 — requires `Status == "Approved"`. `SubmitApplication` at line 196 — duplicate guard at line 211. `PetDetail` at line 56 — queries `ExistingAppId`/`ExistingAppStatus` when user logged in. |
| `Controllers/ApplicationsController.cs:10` | All actions require `[Authorize]`. `UpdateStatus` (POST) at line 115 delegates entirely to `_appService.ApplyStatusChangeAsync`. Role scope enforced in `Index` at lines 35-50. |
| `Controllers/AdminController.cs:10` | `[Authorize(Roles = "Admin")]` on class. `ApproveListing` at line 39. `RejectListing` at line 67. `UpdateApplicationStatus` at line 146 — delegates to `_appService.ApplyStatusChangeAsync` (state machine enforced). |
| `Controllers/RehomeController.cs:1` | 7-step wizard (Steps 1–8 including confirm/publish). `SaveStep7` at line 283 — stores `string[]? docs` as JSON in `DocumentsJson`; **no file upload**. `Publish` at line 317 — sets `Status = "Pending"`, notifies admins + rehomer; **no check** that any document was uploaded. `DeleteListing` at line 357 — guards against delete if active applications exist. |
| `Controllers/AccountController.cs:66` | `Register` sets `EmailConfirmed = true` at line 80 — **no email verification step, no 6-digit code**. Signs in immediately after register at line 96. |
| `Controllers/MessagesController.cs:1` | `Send` at line 79 — persists message, notifies receiver (type: `new_message`). `DeleteConversation` at line 114 — soft-delete per-side. |

### Views

| Path | Notes |
|---|---|
| `Views/Applications/Index.cshtml` | Uses `app.PetListing.Name`, `app.Adopter?.DisplayName`, `app-badge-*` CSS classes. |
| `Views/Applications/Details.cshtml` | Full application read-only view. |
| `Views/Applications/UpdateStatus.cshtml` | Status form for Rehomer/Admin. |
| `Views/Home/AdoptionWizard.cshtml` | 5-step wizard. `LISTING_ID` emitted by Razor at line 457. `renderReview()` reads sidebar DOM elements (not `pet` variable). Submission payload includes `petListingId: LISTING_ID`. |
| `Views/Home/PetDetail.cshtml` | Shows "applied" banner (status + link to `/Applications/Details/{id}`) when adopter has active application. Apply button hidden in that case. |
| `Views/Home/AdopterProfile.cshtml` | Rehomer sees incoming applications for their own listings. |
| `Views/Admin/Index.cshtml` | Admin panel — user management, listing approve/reject, application status override. |

### Migrations (applied in order)

| Migration | Date | What it does |
|---|---|---|
| `20260625153703_InitialSqlite` | 2026-06-25 | Full schema: all ASP.NET Identity tables, `PetListings`, `AdoptionApplications` (with legacy `HardcodedPetId`, `PetName`, `FirstName`; `PetListingId` nullable), `Messages` (without `DeletedFor*`), `Notifications`. |
| `20260625161527_NormaliseApplicationFKs` | 2026-06-25 | Drops `FirstName`, `HardcodedPetId`, `PetName`; makes `PetListingId` NOT NULL. |
| `20260629151244_AddMessageDeletedFlags` | 2026-06-29 | Adds `DeletedForSender`, `DeletedForReceiver` to `Messages`. |

**All three migrations applied; schema matches current models.**

### Config files

| File | Committed? | Contents |
|---|---|---|
| `appsettings.json` | Yes | `"DefaultConnection": "Data Source=pawdopt.db"`, `"AdminSeed": { "Email": "", "Password": "" }` (placeholder). |
| `appsettings.Development.json` | No (gitignored via `*.Development.json`) | `"AdminSeed": { "Email": "admin@pawdopt.ca", "Password": "Admin@2026!" }` — real credentials. |
| `Program.cs:85-86` | Yes | Fallback `?? "admin@pawdopt.ca"` / `?? "Admin@2026!"` — fallback only, not primary. |

---

## 3. ApplicationStatus State Machine

### As implemented in code (`Models/ApplicationStatus.cs:19-27`)

```
Pending     → UnderReview, Rejected
UnderReview → Approved, Rejected
Approved    → (terminal)
Rejected    → (terminal)
Withdrawn   → (terminal)  [adopter-initiated via ApplicationsController.Withdraw or HomeController.WithdrawApplication]
Cancelled   → (terminal)  [auto-set when a competing app is Approved]
```

Transition guard: `CanTransitionTo()` at `ApplicationService.cs:23` — called before every status change in both `ApplicationsController.UpdateStatus` and `AdminController.UpdateApplicationStatus`.

### As described in the UML class diagram (`Pawdopt_v2_UML_Diagrams.html:248`)

```
Status enum listed: Pending | Reviewing | Accepted | Rejected
```

### As described in the UML state diagram (`Pawdopt_v2_UML_Diagrams.html:432-488`)

```
Submitted → Under Review → Accepted → Contact initiated → Completed
                        ↓
                    Rejected (terminal)
Submitted → (cancel) → [back to start]
```

### MISMATCHES — application status

| Report term | Code term | Verdict |
|---|---|---|
| `Submitted` | `Pending` | **MISMATCH** — report must use `Pending` |
| `Reviewing` (class diagram) | `UnderReview` | **MISMATCH** — report must use `UnderReview` |
| `Accepted` | `Approved` | **MISMATCH** — report must use `Approved` |
| `Contact initiated` | _(does not exist)_ | **MISMATCH** — no such status in code |
| `Completed` | _(does not exist)_ | **MISMATCH** — no such status in code |
| `cancel` (adopter action) | `Withdrawn` | **MISMATCH** — action exists but status is `Withdrawn`, not `Cancelled` by adopter |
| `Cancelled` (auto on approval) | `Cancelled` | **Undocumented** — code has this; UML state diagram does not show it |

### PetListing status mismatches

| Report term | Code term | Verdict |
|---|---|---|
| `Pending Review` | `"Pending"` | **MISMATCH** — name differs |
| `Live` | `"Approved"` | **MISMATCH** — name differs |
| `Adopted` | _(does not exist)_ | **MISMATCH** — no such status; approval of an application does not update the listing status |
| `Removed` | _(does not exist)_ | **MISMATCH** — listings are hard-deleted (`_context.PetListings.Remove`), not set to Removed |

---

## 4. Business Rule Verification

### FR-5 — Vaccination document upload required before listing goes live

**Verdict: NOT IMPLEMENTED.**

The UML diagram marks this with ★ as a "shelter client requirement." The class presentation (slide 3) states: "mandatory vaccination document upload." User story US-9 states: "upload vaccination document before listing is submitted for admin review."

What the code actually does:

- `RehomeController.Documents` (GET, line 273) — renders a step-7 view.
- `RehomeController.SaveStep7` (line 283) — receives `string[]? docs`, a checkbox selection of document *type keys* (e.g., `"vaccination"`, `"health-cert"`). Stores as JSON text in `PetListing.DocumentsJson`. **No file is uploaded. No actual document reaches the server.**
- `RehomeController.Publish` (line 317) — sets status to `"Pending"` and saves. **There is no check** that `DocumentsJson` is non-null, non-empty, or that any file was uploaded. A listing can be published with `DocumentsJson = null`.
- The UML class diagram shows a separate `Documents` table with `FileUrl` and `VerificationStatus` columns. **This table does not exist.** There is no `Documents` entity, no `DocumentsController`, and no file-upload endpoint for vaccination records.

The report must not claim that FR-5 is met. The implemented behaviour is: the rehomer checks boxes to indicate *which document types they have*, with no enforcement.

### FR-6 — Admin approval gate before listings appear on Adopt

**Verdict: IMPLEMENTED.**

- `RehomeController.Publish:322` — sets `Status = "Pending"`.
- `AdminController.ApproveListing:47` — sets `Status = "Approved"`, sets `ApprovedAt`.
- `HomeController.GetApprovedPetDtosAsync:55` — filters `l.Status == "Approved"`. Only Approved listings appear in the Adopt grid.
- `HomeController.AdoptionWizard:189` — `FirstOrDefaultAsync(l => l.Id == petListingId && l.Status == "Approved")`. Wizard throws 404 if listing is not Approved.

### Competitor auto-cancellation on approve (transaction)

**Verdict: IMPLEMENTED.** `Services/ApplicationService.cs:30-77`.

- `BeginTransactionAsync()` at line 30.
- Competing apps (same `PetListingId`, status `Pending` or `UnderReview`, different `Id`) queried at lines 37-42.
- Each set to `Cancelled` and notified at lines 44-56.
- `SaveChangesAsync` + `CommitAsync` at lines 70-71.
- `RollbackAsync` in catch at line 75.

### Email verification (US-3 — 6-digit code)

**Verdict: NOT IMPLEMENTED.**

- `AccountController.Register:80` — `EmailConfirmed = true` is set directly on the new user object.
- `SignInAsync` is called immediately at line 96, before any verification.
- No verification token, no 6-digit code, no email is sent. The user story US-3 ("verify email with 6-digit code before accessing the platform") is not met.

---

## 5. Notification Matrix

Who is notified on which events (verified by reading the source):

| Event | Who is notified | Type string | Source |
|---|---|---|---|
| Rehomer publishes listing | All admins | `"new_listing"` | `RehomeController.Publish:327` |
| Rehomer publishes listing | Rehomer themselves | `"listing_submitted"` | `RehomeController.Publish:335` |
| Admin approves listing | Rehomer | `"listing_approved"` | `AdminController.ApproveListing:52` |
| Admin rejects listing | Rehomer | `"listing_rejected"` | `AdminController.RejectListing:79` |
| Adopter submits application | Adopter | `"app_submitted"` | `HomeController.SubmitApplication:252` |
| Adopter submits application | Rehomer of that listing | `"new_application"` | `HomeController.SubmitApplication:264` |
| Application approved | Approved adopter | `"app_approved"` | `ApplicationService.cs:58` |
| Competing app auto-cancelled | Each cancelled adopter | `"app_cancelled"` | `ApplicationService.cs:47` |
| Application status changed (non-approve) | Adopter | `"app_status_change"` | `ApplicationService.cs:72` |
| Admin changes application status | Adopter (via service) | `"app_status_change"` | `AdminController.UpdateApplicationStatus:161` |
| New message sent | Message receiver | `"new_message"` | `MessagesController.Send:99` |

**Answer to the explicit question: Are rehomers notified when they submit a listing?**  
YES. `RehomeController.Publish:335` adds a notification to `listing.RehomerId` with type `"listing_submitted"` and body "Your listing is awaiting admin review."

---

## 6. Reconciliation Table

| Requirement / Feature | In report docs? | In code (file:line) | Match? | Action needed |
|---|---|---|---|---|
| Database: SQLite, local file | No (presentation says SQL Server) | `appsettings.json:4`, `Program.cs:11` | **MISMATCH** | Update report: DB switched to SQLite (`pawdopt.db`), not SQL Server |
| AdminSeed password out of source | Not mentioned | `appsettings.Development.json:13` (gitignored) | Undocumented | Add to report: credentials now in gitignored dev config |
| 3 migrations applied | Not mentioned | `Migrations/` directory | Undocumented | Add to report: list all 3 migrations and what each does |
| FR-5: Vaccination doc upload (mandatory) | ★ in UML, US-9, slide 3 | **Not enforced** — `RehomeController.Publish:317` has no gate; `DocumentsJson` = checkbox keys only | **MISMATCH** | Report must NOT claim FR-5 is met; describe what is actually implemented (checkbox selection, no file upload) |
| FR-6: Admin approval gate | UML use case, slide 4 | `AdminController.ApproveListing:47`, `HomeController:55` | **MATCH** | Report may claim FR-6 is fully implemented |
| Email verification, 6-digit code (US-3) | slide 5, UML use case | `AccountController.Register:80` sets `EmailConfirmed = true` directly, no code sent | **MISMATCH** | Report must NOT claim US-3 is met; describe actual: immediate sign-in, no verification step |
| ApplicationStatus: `Pending` | Absent — UML says `Submitted` | `Models/ApplicationStatus.cs:8` | **MISMATCH** | Report must use `Pending` not `Submitted` |
| ApplicationStatus: `UnderReview` | UML class diagram says `Reviewing` | `Models/ApplicationStatus.cs:9` | **MISMATCH** | Report must use `UnderReview` not `Reviewing` |
| ApplicationStatus: `Approved` | UML says `Accepted` | `Models/ApplicationStatus.cs:10` | **MISMATCH** | Report must use `Approved` not `Accepted` |
| ApplicationStatus: `Withdrawn` | UML says `cancel` (action) | `Models/ApplicationStatus.cs:12` | **MISMATCH** | Report must use `Withdrawn` |
| ApplicationStatus: `Cancelled` (auto) | Not shown in UML | `Models/ApplicationStatus.cs:13`, `ApplicationService.cs:46` | **Undocumented** | Add to report: system auto-sets Cancelled on all competing apps when one is Approved |
| ApplicationStatus: `Contact initiated` | UML state diagram | Does not exist in code | **MISMATCH** | Remove from report |
| ApplicationStatus: `Completed` | UML state diagram | Does not exist in code | **MISMATCH** | Remove from report |
| PetListing status: `"Pending"` | UML calls it `Pending Review` | `RehomeController.Publish:322` | **MISMATCH** (name only) | Report must use `"Pending"` not `"Pending Review"` |
| PetListing status: `"Approved"` | UML calls it `Live` | `HomeController:55`, `AdminController:47` | **MISMATCH** (name only) | Report must use `"Approved"` not `"Live"` |
| PetListing status: `"Adopted"` | UML shows terminal state | Does not exist in code | **MISMATCH** | Remove from report; approval of application does NOT set listing status |
| PetListing status: `"Removed"` | UML shows optional state | Does not exist; listings are hard-deleted | **MISMATCH** | Remove from report; note hard-delete with active-application guard |
| `Documents` table (separate entity) | UML class diagram | Does not exist; `DocumentsJson` TEXT column only | **MISMATCH** | Remove Documents table from class diagram description; describe `DocumentsJson` |
| `RehomeSubmissions` table | UML class diagram | Does not exist; replaced by `PetListings` | **MISMATCH** | Remove from report; one `PetListings` table serves both Rehome wizard and Adopt page |
| `Users.FirstName`, `Users.LastName` | UML class diagram | Not in `ApplicationUser`; uses `DisplayName` | **MISMATCH** | Update class diagram description to `DisplayName` |
| `Users.VerificationCode`, `VerifCodeExpiry` | UML class diagram | Not in `ApplicationUser` or any table | **MISMATCH** | Remove from report (email verification not implemented) |
| Competitor auto-cancellation on approve | Not mentioned in docs | `ApplicationService.cs:37-56` | **Undocumented** | Add to report as key business rule |
| `ApplicationService` (shared service) | Not mentioned | `Services/ApplicationService.cs:20` | **Undocumented** | Add to report: single shared service enforces state machine for both Rehomer and Admin paths |
| `+10,000 offset` (real pet IDs in JS grid) | Not mentioned | `HomeController.cs:21` | **Undocumented** | Add to report: real listings merged with demo pets using offset to prevent ID collisions |
| In-platform messaging | D2 scope (slide 6) | `Controllers/MessagesController.cs:1`, `Views/Messages/` | **MATCH** | Confirm implemented in report |
| Role-based dashboard | D2 scope, US-5 | `ApplicationsController:35-50`, `HomeController.AdopterProfile:151` | **MATCH** | Confirm implemented |
| Adoption wizard (5-step) | D2 scope, US-7 | `Views/Home/AdoptionWizard.cshtml`, `HomeController.AdoptionWizard:185` | **MATCH** | Confirm implemented |
| Pet detail "already applied" state | Not mentioned | `HomeController.PetDetail:56`, `Views/Home/PetDetail.cshtml` | **Undocumented** | Add to report: adopters see status banner + link on pet detail if already applied |
| Admin panel: user management, listing approve/reject | D3 scope in slide 6 | `AdminController.cs:1` | **Undocumented for D2** | Note: admin panel was implemented ahead of D3 schedule |

---

## 7. Status Summary by D2 Scope Area

### 7.1 Architecture / MVC Layout — **Done**

7 controllers, clean DI, no repository pattern, single `ApplicationDbContext`. Shared `ApplicationService` for status logic. Stack: ASP.NET Core 10 MVC + EF Core + SQLite + ASP.NET Core Identity.

### 7.2 Database Setup — **Done (with report mismatch)**

SQLite (`pawdopt.db`), 3 migrations applied, `Database.Migrate()` at startup, file gitignored.  
**Report fix required:** presentation and early UML docs reference SQL Server. Report must state the switch to SQLite.

### 7.3 AdoptionApplication CRUD — **Done**

Create: `HomeController.SubmitApplication` (wizard submission).  
Read: `ApplicationsController.Index` + `Details` (role-scoped).  
Update: `ApplicationsController.UpdateStatus` + `AdminController.UpdateApplicationStatus` (both via `ApplicationService`, state machine enforced).  
Withdraw: `ApplicationsController.Withdraw` (soft-delete, Pending/UnderReview only).  
**Report fix required:** rename all state names (Submitted→Pending, Accepted→Approved, Reviewing→UnderReview); add Cancelled and Withdrawn; remove Contact initiated / Completed.

### 7.4 Adopt ↔ Rehome Bridge — **Done (undocumented)**

`RealPetIdOffset = 10000` merges demo pets and real approved listings in one JS array without ID collision. `AdoptionWizard` requires `petListingId` (real DB id); only Approved listings accepted.  
**Report fix required:** add this mechanism; it is not in any current report doc.

### 7.5 Rehome CRUD — **Partial (FR-5 not enforced)**

7-step wizard: Draft → Pending → Approved/Rejected (can resubmit from Rejected). Edit locked to Draft/Rejected only. Hard-delete blocked when active applications exist.  
**FR-5 gap:** Step 7 records document type checkboxes only; no file upload; no publish gate. Report must not claim this requirement is met.

### 7.6 Roles and Lifecycle — **Done**

3 roles (Admin, Adopter, Rehomer), seed-only Admin, self-registration limited to Adopter/Rehomer. Route-level `[Authorize(Roles = "...")]` throughout. Account lockout via `LockoutEnd = DateTimeOffset.MaxValue`.  
**Report fix required:** email verification (US-3) is not implemented; `AccountController.Register` skips the 6-digit code entirely.

### 7.7 Security / Hygiene — **Done (minor)**

Anti-forgery tokens on all POSTs; `AdminSeed` credentials moved to gitignored dev config; `*.db` gitignored; HTTPS enforced in non-dev. One note: the `Program.cs` fallback still contains the literal `"Admin@2026!"` string as a ?? default (line 86) — this is in committed source.

### 7.8 Build Health — **Done**

`dotnet build`: 0 errors, 2 warnings (both `NU1903` SQLite advisory — pre-existing, not actionable).

---

## 8. Known Gaps (code does something the report doesn't mention)

1. `ApplicationService` (shared service layer) — not in any report doc.
2. `Cancelled` application status (auto-set on competitor cancel) — not in UML.
3. `+10,000` real-pet ID offset for JS grid merging — not documented.
4. Rehomer notified on own listing submission — not mentioned in notification design.
5. Pet detail "already applied" banner — new feature, not in any doc.
6. 3rd migration `AddMessageDeletedFlags` — soft-delete per conversation side; not documented.
7. Admin panel implemented at D2 (originally D3 scope per slide 6).
8. `DeleteConversation` soft-delete (per-side) in `MessagesController` — not in any report doc.

---

## 9. Report Fixes Needed (report claims something code doesn't do)

These are **MISMATCHES** that must be corrected before submission:

1. **Database:** Change "SQL Server" to "SQLite (`pawdopt.db`)" everywhere, and remove Docker references from now on.
2. **FR-5:** State clearly that vaccination document upload is **not yet implemented**. Step 7 captures document type declarations only; no file is uploaded; no gate exists in `Publish`. The `Documents` table shown in the class diagram does not exist.
3. **US-3 (email verification):** State clearly that the 6-digit email verification step is **not implemented**. Registration signs the user in immediately with `EmailConfirmed = true`.
4. **Application status enum names:** Replace `Submitted → Reviewing → Accepted` with `Pending → UnderReview → Approved`. Remove `Contact initiated` and `Completed`. Add `Withdrawn` (adopter-initiated) and `Cancelled` (system auto-set).
5. **PetListing status names:** Replace `Pending Review` with `Pending`; replace `Live` with `Approved`. Remove `Adopted` and `Removed` (these states do not exist in code).
6. **Class diagram — Users table:** Replace `FirstName`, `LastName`, `VerificationCode`, `VerifCodeExpiry` with actual columns: `DisplayName`, `UserType`, `City`, `Province`, `CreatedAt`.
7. **Class diagram — Documents table:** Remove entirely. Replace with a note that `PetListing.DocumentsJson` stores a JSON array of document-type keys (text only, no file upload).
8. **Class diagram — RehomeSubmissions table:** Remove. Rehome submissions are stored in the `PetListings` table with `Status = "Draft"/"Pending"`.
9. **State diagram — Application lifecycle:** Remove `Contact initiated → Completed` path. Add `Cancelled` (auto) and `Withdrawn` (adopter) terminal states.
10. **State diagram — PetListing lifecycle:** Remove `Adopted` and `Removed` terminal states. `"Approved"` is effectively terminal (listings do not auto-transition to Adopted when an application is approved).
