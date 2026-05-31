# Prompts pour une IA de génération de code

## Prompt 1 - Générer la solution

```text
Génère une solution fullstack nommée ZebraLabels avec:
- .NET 10
- ASP.NET Core 10
- Aspire 13
- Svelte 5 avec runes et Vite
- PostgreSQL
- Labelize self-hosted comme moteur de preview

Contraintes:
- architecture vertical slice
- support EPL et ZPL
- impression via WebUSB dans le navigateur uniquement
- pas de dépendance à une API externe pour la preview
- code production-ready, tests inclus

Lis et respecte:
- ai-context.json
- 02-architecture.md
- 03-vertical-slices.md
- 09-quality-bar.md
- 11-solution-structure.md
```

## Prompt 2 - Générer le backend

```text
Implémente le backend ASP.NET Core 10 du projet ZebraLabels.

Exigences:
- vertical slices: PrinterProfiles, Templates, Preview, PrintJobs
- EF Core + PostgreSQL
- ProblemDetails cohérent
- HttpClient typed vers Labelize
- health checks PostgreSQL + Labelize
- OpenTelemetry prêt pour Aspire

Ne crée pas de couches génériques inutiles.
Chaque slice doit contenir ses endpoints, requests, validators, handlers, mappings et tests.
```

## Prompt 3 - Générer le frontend

```text
Implémente le frontend Svelte 5 avec runes.

Exigences:
- SPA Vite
- modules templates, preview, printers, print-jobs
- module WebUSB isolé dans lib/webusb
- UX claire sur les erreurs WebUSB et de rendu
- TypeScript strict

Le frontend ne doit jamais appeler Labelize directement.
```

## Prompt 4 - Générer l'intégration Labelize

```text
Implémente une intégration backend vers Labelize self-hosted.

Contraintes:
- utiliser Content-Type application/epl ou application/zpl
- supporter PNG et PDF
- accepter width, height, dpmm
- transformer les erreurs de Labelize en ProblemDetails métier
- ajouter tests d'intégration simulant les réponses HTTP de Labelize
```

## Prompt 5 - Générer WebUSB

```text
Implémente un module WebUSB orienté Zebra.

Contraintes:
- requestDevice avec filtres vendor/product issus des PrinterProfiles
- découverte d'interface et endpoint out
- envoi d'un payload brut EPL ou ZPL
- erreurs typées et messages UI actionnables
- aucun accès WebUSB côté serveur
```
