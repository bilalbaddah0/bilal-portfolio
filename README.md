# Bilal Baddah — Developer Portfolio

A production-oriented personal portfolio built with ASP.NET Core MVC, C#, Entity
Framework Core, SQL Server, ASP.NET Core Identity, Bootstrap, and JavaScript.

The public site presents Bilal's skills, experience, education, and projects. Its
contact form writes messages to SQL Server without reloading the page. A protected
admin dashboard supports search, read/unread filtering, pagination, message
details, status changes, and deletion.

## Features

- Responsive one-page developer portfolio
- Central profile configuration in `appsettings.json`
- Server- and client-side contact validation
- SQL Server message storage through Entity Framework Core
- Rate-limited contact endpoint with an anti-bot honeypot
- ASP.NET Core Identity authentication with no public registration
- Admin-only inbox at `/admin`
- Search, filters, pagination, read/unread status, and delete confirmation
- Secure cookies, anti-forgery validation, security headers, and production error handling
- Downloadable CV endpoint with a friendly missing-file state
- Docker configuration for Vercel and direct Visual Studio publishing support for Azure

## Requirements

- Visual Studio with the **ASP.NET and web development** workload
- .NET 10 SDK
- SQL Server LocalDB for development (installed with relevant Visual Studio workloads)

## Open in Visual Studio

1. Open `BilalPortfolio.slnx`.
2. Wait for Visual Studio to restore NuGet packages.
3. Open **Tools → NuGet Package Manager → Package Manager Console**.
4. Run:

   ```powershell
   Update-Database
   ```

5. Configure the private admin account as described below.
6. Press **F5** to run with the debugger or **Ctrl+F5** to run without it.

## Database setup

Development uses SQL Server LocalDB:

```text
Server=(localdb)\mssqllocaldb;
Database=BilalPortfolioDb;
Trusted_Connection=True;
MultipleActiveResultSets=true;
TrustServerCertificate=True;
```

The connection string is under `ConnectionStrings:DefaultConnection` in
`appsettings.json`. For another SQL Server instance, change the server, database,
and authentication settings. Never commit a production password.

Equivalent Entity Framework CLI commands:

```powershell
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

## Create the first admin safely

The application creates or updates the administrator only when both values below
are available. They belong in .NET User Secrets, not source control.

In Visual Studio, right-click the project and choose **Manage User Secrets**, then
use this structure:

```json
{
  "Admin": {
    "Email": "your-admin-email@example.com",
    "Password": "choose-a-long-unique-password"
  }
}
```

Alternatively, from the project directory:

```powershell
dotnet user-secrets set "Admin:Email" "your-admin-email@example.com"
dotnet user-secrets set "Admin:Password" "choose-a-long-unique-password"
```

Run the application once after applying migrations. The account receives the
`Admin` role and can sign in at `/account/login`. Remove the password from User
Secrets after the account is created if you do not want startup to keep it
available. There is deliberately no public registration page.

## Update portfolio content

Edit the `Portfolio` section in `appsettings.json` to replace the centralized:

- Email address
- GitHub URL
- LinkedIn URL
- University, degree, and education years

Project and experience copy is in `Views/Home/Index.cshtml`. Replace placeholder
project links and screenshots there as real projects become available.

## Add the CV

Place the current PDF here:

```text
wwwroot/files/Bilal-Baddah-CV.pdf
```

The site serves it through `/cv`. If the file is missing, the visitor sees a
friendly message instead of a broken download.

## Production configuration

Set these values through the host's environment-variable or secret settings:

```text
ConnectionStrings__DefaultConnection
Admin__Email
Admin__Password
ASPNETCORE_ENVIRONMENT=Production
```

Apply database migrations as a deployment step. Do not make a production web
instance responsible for changing its own schema on every startup.

## Deploy from Visual Studio to Azure

Azure App Service with Azure SQL is the simplest fit for ASP.NET Core and SQL
Server:

1. Right-click the project and choose **Publish**.
2. Select **Azure → Azure App Service**.
3. Create or select the App Service.
4. Under **Service Dependencies**, connect an Azure SQL database.
5. Add the admin secrets in the App Service configuration.
6. Apply the EF migration through the publish profile or a controlled deployment step.
7. Publish and test the public contact flow and the private admin dashboard.

## Deploy to Heroku

Heroku officially supports .NET 8 and later. The project is ready for its .NET
buildpack and already listens on the `PORT` environment variable supplied by
Heroku. Run these commands from the folder that contains `BilalPortfolio.slnx`.

1. Create a hosted SQL Server database first. Azure SQL is the simplest match.
   LocalDB runs only on this computer and cannot be used by a Heroku dyno.
2. Apply the existing Entity Framework migration to the hosted database from a
   trusted computer before starting the production app.
3. Install Git and the Heroku CLI, sign in, and initialize the repository if it
   is not already a Git repository:

   ```powershell
   git init
   git add .
   git commit -m "Initial portfolio"
   heroku login
   ```

4. Create the app. The requested name must be globally available:

   ```powershell
   heroku create bilal-baddah-portfolio
   ```

5. Store production settings in Heroku config vars. Replace all example values;
   never commit the real database password or admin password:

   ```powershell
   heroku config:set ASPNETCORE_ENVIRONMENT=Production
   heroku config:set ConnectionStrings__DefaultConnection="<Azure SQL ADO.NET connection string>"
   heroku config:set Admin__Email="<private admin email>"
   heroku config:set Admin__Password="<long unique admin password>"
   ```

6. Deploy and inspect the application:

   ```powershell
   git push heroku HEAD:main
   heroku open
   heroku logs --tail
   ```

Heroku provides a default address ending in `.herokuapp.com`; it is not a `.app`
or `.com` domain. To use a purchased domain, add it with
`heroku domains:add www.example.com`, run `heroku domains` to get the DNS target,
and create the matching DNS record at the domain registrar.

Heroku hosting is paid. The low-cost Eco dyno plan sleeps after inactivity. The
external Azure SQL database and a custom `.com` domain can have separate costs.

## Deploy to Vercel

Vercel does not provide a native .NET runtime. This repository includes
`Dockerfile.vercel` and `vercel.json` so Vercel can run the application as a
container service.

1. Push this folder to GitHub.
2. Import the repository in Vercel with this folder as the project root.
3. Add `ConnectionStrings__DefaultConnection` for an external SQL Server or Azure
   SQL database; LocalDB cannot run in the Vercel container.
4. Add the admin environment variables.
5. Deploy and verify the migration has already been applied to the production database.

The container is stateless, so uploaded files and the database must live outside
the container. Azure App Service is still the easier deployment choice for this
specific ASP.NET Core + SQL Server stack.

## Custom `.com` domain

Purchase the domain from Vercel, Azure, Cloudflare, or another registrar. In the
chosen hosting dashboard, add the domain to the deployed application and copy the
exact DNS records shown there into the registrar's DNS settings. Keep the free host
URL until the custom domain and HTTPS certificate are verified.

## Security notes

- Razor encodes contact content; do not render submitted messages with `Html.Raw`.
- EF Core parameterizes database queries.
- Every state-changing MVC request requires an anti-forgery token.
- Login attempts use lockout protection.
- The contact endpoint is rate limited and request-size limited.
- Passwords, connection strings, and deployment credentials do not belong in Git.

## Useful routes

| Route | Purpose |
|---|---|
| `/` | Public portfolio |
| `/contact` | Contact form POST endpoint |
| `/cv` | CV download |
| `/account/login` | Administrator login |
| `/admin` | Protected message dashboard |
