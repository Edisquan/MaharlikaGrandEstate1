Application-Level Security Features
1. Authentication
ASP.NET Core Identity is used for user authentication

Login/Registration with Role-based Access (Admin, Buyer, Lister)

Google Authentication integrated for secure OAuth2-based login

2. Authorization (RBAC)
Role-Based Access Control (RBAC) ensures:

Admins can manage users and approve listings

Listers can create and manage their properties

Buyers can view, search, and reserve properties
[Authorize(Roles = "Admin")]
[Authorize(Roles = "Lister")]
3. Input Validation & Model Binding
Data Annotations and server-side validation prevent invalid inputs:

[Required], [StringLength], [Range], etc.

Prevents overposting attacks using [Bind] and ViewModels

4. Anti-Forgery Protection
CSRF tokens are included in all forms via:

html
Copy
Edit
@Html.AntiForgeryToken()
Ensures only legitimate users can submit sensitive forms.

5. Password Security
Passwords are hashed and salted using ASP.NET Identity

Enforces password strength policies (min length, special characters)

6. Session Management
Authentication cookie expiration settings configured

Logout functionality clears sessions securely

7. Error Handling
Custom error pages prevent leaking internal stack traces to users

Try-Catch used during database operations to catch exceptions gracefully

8. Rate Limiting & Brute Force Prevention (Optional)
Can be integrated using middleware like ASP.NET RateLimiter or external services (e.g., Cloudflare)
Database-Level Security Features
1. Entity Framework Core + Migrations
All database access is managed via EF Core, which prevents SQL Injection by design

No raw SQL is exposed directly to clients

2. Secure Relationships & Keys
Foreign key constraints and relationships ensure referential integrity

For example:

csharp
Copy
Edit
public string ListerId { get; set; }
[ForeignKey("ListerId")]
3. Nullable Fields for Optional Data
Properties like BuyerId, ReservedById are nullable to prevent insertion errors and enforce logical state

4. Data Validation on Save
EF Core’s ModelState.IsValid ensures data saved is consistent and safe

5. Field-Level Access Control (via Logic)
Admin-only fields (e.g., IsApproved, IsArchived) are only modifiable by Admins via controller checks
Hosting & Deployment Security
1. Secure File Paths
Image uploads are validated for type and saved with unique names to prevent path traversal

2. FTP Publishing (Temporary)
If using MonsterASP.NET FTP:

Make sure long file paths are avoided

Use secure passwords and change them after deployment

3. Production Builds
Deployed builds are set to Release configuration to prevent debugging info exposure

Optional Improvements You Can Mention
HTTPS enforcement (can be configured in production)

Logging and Audit Trails

Database Encryption at rest (if using Azure SQL or other managed DB)

Input sanitization for HTML fields (like property descriptions)

