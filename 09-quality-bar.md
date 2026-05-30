# Niveau de qualité attendu

## Backend

- Type safety maximale.
- DTOs distincts par endpoint.
- Value objects pour les primitives métier importantes.
- Erreurs remontées via `ProblemDetails`.
- Aucun `catch` large silencieux.
- Pas de `static helper` générique sans responsabilité métier claire.

## Frontend

- TypeScript strict.
- Pas de logique USB dans les composants d'affichage.
- États dérivés explicites.
- Nettoyage systématique des `Blob` URLs et abonnements.

## Architecture

- Pas de dossier `Common` tentaculaire.
- Pas de couche repository abstraite si EF Core suffit.
- Pas de conversion EPL -> ZPL implicite dans tous les flux.
- Une slice doit pouvoir se lire de haut en bas sans changer de dossier toutes les 30 secondes.

## Tests

### Backend

- unit tests sur services métier,
- tests de validation,
- tests d'intégration d'endpoint par slice,
- tests de contrat sur client Labelize.

### Frontend

- component tests sur les composants critiques,
- tests de logique sur le module WebUSB avec doubles/mocks,
- tests E2E sur le workbench.

## Observabilité

- logs structurés corrélables,
- traces distribuées Aspire,
- métriques simples:
  - temps de rendu,
  - taux d'échec de rendu,
  - taux d'échec d'impression.

## Definition of Done

- tests pertinents verts,
- documentation slice mise à jour si nécessaire,
- erreurs actionnables,
- aucun comportement critique laissé ambigu,
- instrumentation minimale en place.

