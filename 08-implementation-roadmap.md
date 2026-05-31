# Roadmap d'implémentation

## Phase 1 - Solution skeleton

- créer la solution .NET 10
- créer `AppHost`, `ServiceDefaults`, `Api`
- créer l'app Svelte 5 + Vite
- brancher Aspire avec PostgreSQL et Labelize

## Phase 2 - Foundations

- conventions de logs et ProblemDetails
- configuration, options typées, health checks
- migrations EF Core
- client HTTP Labelize

## Phase 3 - Vertical slices MVP

1. `PrinterProfiles`
2. `Templates`
3. `Preview`
4. `PrintJobs`

## Phase 4 - Frontend MVP

- pages `Templates`, `Workbench`, `Printers`, `History`
- éditeur de template
- preview pane
- connexion WebUSB
- impression

## Phase 5 - Qualité

- tests backend
- tests frontend
- E2E critiques
- télémétrie
- durcissement des validations

## Démo MVP attendue

1. Créer un template EPL.
2. Prévisualiser en PNG.
3. Connecter une imprimante Zebra.
4. Imprimer en EPL.
5. Réessayer avec un template ZPL.
