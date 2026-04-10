# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

RP Weave is an open-source TTRPG world-building platform. It lets users create campaigns with PDF/Markdown materials and chat with an AI about campaign content using semantic search.

- **Backend**: ASP.NET Core 9.0 (C#) — `/src/RpWeave.Server/`
- **Frontend**: Angular 20 (TypeScript/SCSS) — `/src/RpWeave.Client/`
- **Databases**: MongoDB (data), Qdrant (vector embeddings)
- **LLM**: Ollama (local inference for embeddings and chat)

## Commands

### Frontend

```bash
cd src/RpWeave.Client
npm start              # Dev server at localhost:4200
npm run build          # Production build
npm test               # Run all Karma/Jasmine tests
ng test --include='**/foo.component.spec.ts'  # Run single test file
```

### Backend

```bash
cd src/RpWeave.Server
dotnet build           # Build solution
dotnet run --project RpWeave.Server.Api  # Run API (port 8080)
dotnet test            # Run tests (no test projects yet)
```

### Demo Environment (Docker)

```bash
cd demo
docker compose -f docker-compose.demo.yml up
```
Services: API (5000), UI (5010), MongoDB (27017), Mongo-Express (8081), Ollama (11434), Qdrant (6333).

Required env vars: `AuthenticationTokenSecret`, `AuthenticationRefreshTokenSecret`, `PublicBaseUrl`, `OllamaReasoningModelName`, `OllamaEmbeddingsModelName`, `RpWeaveApiBaseUrl`, and MongoDB/Qdrant/Ollama host names.

## Architecture

### Backend Projects

The solution has 7 projects with this dependency flow:

```
API → Orchestrations → Integrations (Ollama, Qdrant)
API → Data (MongoDB)
All → Core (shared abstractions)
```

- **`RpWeave.Server.Api`** — Web API entry point. Features are organized as vertical slices under `Features/` (Campaign, User, Ai/Prompt, Settings), each containing Controller, Handler, Request, Response, and Validator classes.
- **`RpWeave.Server.Core`** — Shared abstractions: `Result<T>`, `ValueResult<T>`, `Error`, `ErrorCodes` enum, and DI extensions.
- **`RpWeave.Server.Data`** — MongoDB repositories for `CampaignEntity`, `ChapterEntity`, `AppUser`, and refresh tokens.
- **`RpWeave.Server.Orchestrations`** — Two orchestrators:
  - `BookBreakdownOrchestrator`: Extracts text from PDF/Markdown, chunks it, and stores embeddings in Qdrant.
  - `ChatOrchestrator`: Multi-step pipeline (Classification → VectorSearch → Editing → Writing) to answer questions using campaign content.
- **`RpWeave.Server.Integrations.Ollama`** — Ollama HTTP client for chat completions and embeddings.
- **`RpWeave.Server.Integrations.Qdrant`** — Qdrant client for storing and querying vectors.
- **`RpWeave.Server.Integrations.Tesseract`** — OCR integration (currently unused).

### Backend Patterns

- Handlers return `Result<T>` / `ValueResult<T>` — never throw for domain errors. Check `ErrorCodes` enum for standardized error values.
- Auth uses JWT access tokens + refresh tokens, with optional OpenID Connect support.
- `appsettings.json` / `appsettings.Development.json` configure Ollama URL and model names.

### Frontend

Angular 20 with Material Design and Bootstrap 5.

- `core/clients/rpweave.client.ts` — Single HTTP client for all API calls.
- `core/interceptors/authentication.interceptor.ts` — Attaches JWT to requests; handles token refresh.
- `core/guards/` — `auth.guard` (login required) and `admin.guard` (admin role).
- `features/` — One directory per route: `login`, `signup`, `campaigns`, `campaign-details`, `dashboard`, `change-password`, `landing`, `settings`.
- `proxy.conf.json` — Dev proxy routes `/api` to the backend.

### CI/CD

GitHub Actions (`.github/workflows/docker-push.yml`) builds and pushes Docker images to Docker Hub on every push to `main`. Images are tagged `edge` and `YYYY.MM.DD-<sha>` for `cooldruid12/rpweave.api` and `cooldruid12/rpweave.ui`.
