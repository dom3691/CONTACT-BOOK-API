# Contact Book API

ASP.NET Core (`net5.0`) Web API for managing user contacts (JWT auth, ASP.NET Core Identity,
EF Core + SQL Server, Cloudinary image upload, email). This repo contains only the `API`
web project (`API.csproj`, `Program.cs`, `Startup.cs`, `Controllers/`). Swagger UI is served in
Development at `/swagger`; the app listens on `https://localhost:5001` / `http://localhost:5000`.

## Cursor Cloud specific instructions

- Toolchain: .NET 5 SDK (`dotnet --version` → `5.0.x`). Standard commands: `dotnet restore API.csproj`,
  `dotnet build API.csproj`, `dotnet run --project API.csproj`. There is no README, test project, or
  lint config in this repo.
- Non-obvious runtime dep: .NET 5 requires **OpenSSL 1.1** (`libssl1.1`), which Ubuntu 24.04 does not
  ship (it has OpenSSL 3). Without it, `dotnet restore`/`build` fail with `No usable version of libssl
  was found`. `libssl1.1` is installed in this environment; if you hit that error, reinstall it.
- **KNOWN BLOCKER — repo is incomplete and does NOT build/run as-is.** `API.csproj` references
  `..\BusinessLogic\BusinessLogic.csproj` and `..\Data\Data.csproj` (which in turn expect a `Model`
  project), and `Startup.cs` + the controllers use types from `Model`, `Data`, `Data.DTO`, and
  `BusinessLogic.*`. Those three projects were never committed (single commit; only `API.csproj` is in
  git history) and exist nowhere on disk. `dotnet build` therefore fails with ~36 `CS0246`/`CS0234`
  errors. The application cannot be built or run until those projects are added to the repository.
  This is a missing-source problem, not a toolchain problem — do not attempt to "fix" it by
  reconstructing the projects from scratch unless explicitly asked.
- Config caveats (relevant only once the missing projects are present): the connection string in
  `appsettings.json` targets a Windows SQL Server host (`Data Source=KARONWI; Integrated Security=True`)
  and must be repointed to a reachable SQL Server (e.g. a Linux `mcr.microsoft.com/mssql/server`
  container with SQL auth). `Startup.cs` reads JWT settings with spaced keys
  (`Configuration["JWTSettings : Audience"]`) that do not match the nested `JWTSettings` keys in
  `appsettings.json`.
