# Zebra Label Platform Blueprint

Ce dossier contient un **pack de cadrage prêt pour une IA** afin de générer une application fullstack d'impression d'étiquettes Zebra via **WebUSB** avec **.NET 10**, **ASP.NET Core 10**, **Aspire 13** et **Svelte 5 avec runes**.

## Décisions déjà prises

- **Architecture**: vertical slice architecture côté backend et frontend.
- **Prévisualisation**: **Labelize self-hosted**, jamais une API externe non contrôlée.
- **Langages**: support natif **EPL** et **ZPL**.
- **Impression**: envoi du langage source à l'imprimante via **WebUSB dans le navigateur**.
- **Serveur**: responsable des templates, de la validation, du rendu proxy vers Labelize, des profils d'imprimantes et de l'audit.
- **Client**: responsable de la connexion USB, de la sélection du périphérique et de l'envoi des octets au device autorisé.

## Fichiers clés

- `ai-context.json`: résumé machine-readable pour outiller une IA.
- `01-product-requirements.md`: périmètre fonctionnel et non fonctionnel.
- `02-architecture.md`: architecture cible et topologie Aspire.
- `03-vertical-slices.md`: découpage par slices.
- `05-api-contracts.yaml`: contrat HTTP de départ.
- `06-frontend-spec.md`: conventions Svelte 5 et UX WebUSB.
- `07-preview-and-printing.md`: flux détaillés de preview et d'impression.
- `09-quality-bar.md`: définition du niveau de qualité attendu.
- `10-ai-implementation-prompts.md`: prompts prêts à l'emploi pour une IA.

## Ordre recommandé pour une IA

1. Lire `README.md`, `ai-context.json`, `02-architecture.md`, `03-vertical-slices.md`.
2. Générer la solution et les projets selon `11-solution-structure.md`.
3. Implémenter d'abord les slices `PrinterProfiles`, `Templates`, `Preview`, puis `PrintJobs`.
4. Brancher Labelize via Aspire.
5. Ajouter WebUSB côté frontend.
6. Terminer par les tests, la télémétrie et les gardes de sécurité.

## Test