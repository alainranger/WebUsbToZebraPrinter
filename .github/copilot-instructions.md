# Copilot instructions for this repository

## Repository state

This repository is currently a **blueprint/specification repo**, not a generated application yet. The authoritative sources are the planning documents at the root (`README.md`, `ai-context.json`, `01-product-requirements.md` through `12-reference-notes.md`, and `adrs/`).

There is **no verified build, test, or lint command yet** because the .NET/Aspire/Svelte solution described by the docs has not been scaffolded in this repository. There is also no runnable single-test command yet. If a future session generates the solution skeleton, update this file with the real commands from the resulting manifests and test projects instead of inventing them.

## High-level architecture

- The target system is a 4-part topology:
  - **Svelte 5 + Vite SPA** for UI, preview display, and WebUSB device interaction
  - **ASP.NET Core 10 API** for templates, validation, preview orchestration, printer profiles, print job audit, and persistence
  - **Labelize** as a self-hosted rendering service for EPL/ZPL to PNG/PDF
  - **PostgreSQL** for templates, printer profiles, and print/audit history
- Aspire is expected to orchestrate `ZebraLabels.AppHost`, `ZebraLabels.ServiceDefaults`, `ZebraLabels.Api`, `ZebraLabels.Web`, `labelize`, and `postgres`.
- The **frontend is the only place allowed to talk to USB**. The backend must never use `navigator.usb`, choose a device, or send bytes to a printer.
- The **backend is the only place allowed to orchestrate preview rendering**. The browser should not call Labelize directly; preview requests go through the API.
- Preview and printing intentionally use different outputs:
  - **Preview** returns `PNG` or `PDF`
  - **Printing** sends the final raw **EPL or ZPL** payload

## Slice-first structure

- Organize both backend and frontend by **vertical slices / features**, not generic technical layers.
- Backend slices are expected to start with:
  - `PrinterProfiles`
  - `Templates`
  - `Preview`
  - `PrintJobs`
  - `Health`
- Frontend slices are expected to mirror the product flow:
  - `templates`
  - `preview`
  - `printers`
  - `print-jobs`
- Preferred backend structure:
  - `src/ZebraLabels.Api/Features/<Slice>/<Endpoint>/...`
  - `src/ZebraLabels.Api/Domain/...`
  - `src/ZebraLabels.Api/Infrastructure/Persistence|Preview|Transformations/...`
- Preferred frontend structure:
  - `src/features/...`
  - `src/lib/api`
  - `src/lib/webusb`
  - `src/lib/ui`

## Key conventions to preserve

- **One endpoint = one folder** in backend slices.
- Keep **DTOs/request/response/validator/handler/tests** local to the slice or endpoint; avoid shared catch-all folders.
- Use **plural slice names**.
- Shared code should stay minimal. Do **not** introduce broad `Common` or `Services` dumping grounds.
- Use **value objects** for important domain primitives such as label dimensions and language capabilities.
- Prefer **immutable records** for contracts and use `sealed` where appropriate.
- Use **TimeProvider** instead of ad hoc time access.
- Inject dependencies explicitly.
- Return backend errors via **ProblemDetails / problem+json**.
- Do not add broad silent catches or implicit success-shaped fallbacks.
- Do not add a repository abstraction layer if **EF Core** already covers the need.

## Printing and preview rules

- Preserve the template's **source language** (`EPL` or `ZPL`) through the workflow.
- Printing should use the **source language by default**.
- `EPL -> ZPL` conversion is a **controlled fallback only** when the printer profile explicitly allows it.
- Any conversion must be **explicit, logged, and visible** in the effective language sent.
- Labelize already supports **native EPL and ZPL rendering**; do not introduce a default pre-render conversion step.
- The API should validate preview constraints such as dimensions, payload size, and output type before calling Labelize.

## Frontend-specific conventions

- Use **Svelte 5 runes** (`$state`, `$derived`, `$effect`) for local state.
- Keep global stores minimal and only for truly shared state.
- Isolate all WebUSB code inside **`lib/webusb`**.
- Keep display components as presentation-focused as possible; USB logic should not live in UI components.
- The frontend API client should centralize `fetch` behavior and handle `problem+json`.
- Clean up Blob/object URLs and subscriptions explicitly.
- Always add an existing CSS class to buttons: `primary`, `secondary`, or `danger`. Never leave a button without a class, as it would inherit the raw browser style.

## Domain expectations

- Core domain concepts expected by the docs:
  - `LabelTemplate`
  - `TemplateVariable`
  - `PrinterProfile`
  - `PrintJob`
  - `PrintPayload`
  - `LabelDimensions`
- Important invariants:
  - dimensions must stay positive
  - supported `dpmm` values are constrained
  - template raw content cannot be empty
  - printer `PreferredLanguage` must be supported by its capabilities

## Implementation order

When scaffolding or extending the app, prefer this sequence unless a task explicitly says otherwise:

1. `PrinterProfiles`
2. `Templates`
3. `Preview`
4. `PrintJobs`
5. `Health`, observability, and hardening

## Testing expectations once code exists

- Backend: unit tests for domain/services, validation tests, endpoint integration tests per slice, and contract tests around the Labelize client
- Frontend: component tests for critical UI, logic tests for WebUSB modules with doubles/mocks, and E2E coverage for the workbench flow
- Keep test organization aligned with slices so a feature can be understood and changed end-to-end without crossing many folders
