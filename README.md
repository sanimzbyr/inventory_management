# Inventoria (ASP.NET Core MVC + PostgreSQL)

This is a working starter for the inventory web app you described. It includes:

- ASP.NET Core 8 MVC + Identity (Google & Facebook OAuth hooks)
- PostgreSQL via EF Core (Npgsql)
- Inventories with tabs: Items, Discussion (SignalR), General (auto‑save), Custom ID builder, Fields
- Custom ID generator supporting Fixed, Random (6/9 digits, 20‑bit hex), GUID, DateTime (format), Sequence
- Items with editable custom IDs (unique per inventory via composite index)
- Admin: add/remove admin, block/unblock, delete users
- Full‑text search (simple ILIKE for now; can switch to `to_tsvector` later)
- Two UI languages: English + Bengali; light/dark theme hook in CSS
- Bootstrap 5 UI with table views and toolbar actions (no per‑row buttons)

## Prereqs

- .NET 8 SDK
- PostgreSQL 14+

## 1) Configure database

Create a database (default connection is `inventoria_db`). Update **appsettings.json** or use environment variables.

## 2) Apply EF Core migrations

```bash
cd src/Inventoria
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
```

> The DbContext defines all models and a unique composite index on (InventoryId, CustomId).

## 3) Social login

Put your credentials in **appsettings.json** or use [dotnet user-secrets]:

```bash
dotnet user-secrets init
dotnet user-secrets set "Authentication:Google:ClientId" "<id>"
dotnet user-secrets set "Authentication:Google:ClientSecret" "<secret>"
dotnet user-secrets set "Authentication:Facebook:AppId" "<id>"
dotnet user-secrets set "Authentication:Facebook:AppSecret" "<secret>"
```

## 4) Run

```bash
dotnet run --project src/Inventoria/Inventoria.csproj
```

The seeder creates an admin user: **admin@inventoria.local / Admin#12345**.

## Notes

- Inventory *General* tab auto‑saves every 8 seconds (optimistic concurrency via rowversion is enabled on Inventory and Item).
- The **Custom ID** tab is a simple drag‑reorder builder with a live example and persistence.
- Discussion tab uses SignalR; new posts appear in ~real‑time.
- The "no per‑row buttons" rule is honored — rows are clickable; bulk actions belong to toolbars.
- For a production‑grade full‑text search, create a materialized `tsvector` column and query with `plainto_tsquery`.

## Roadmap (nice‑to‑have, not required to run)

- Access list UI (grant write access by user/email with autocomplete)
- Item fields UI with drag‑and‑drop + constraints/regex
- Likes on items
- Tag cloud on home page
- Export to CSV/Excel
- Better localization and theme switcher with user preference
