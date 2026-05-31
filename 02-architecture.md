# Architecture cible

## Vue d'ensemble

```text
+-------------------+       HTTPS        +------------------------+
| Svelte 5 Web App  | <----------------> | ASP.NET Core 10 API    |
| - runes           |                    | - vertical slices      |
| - WebUSB          |                    | - validation           |
| - preview UI      |                    | - persistence          |
+---------+---------+                    +-----------+------------+
          |                                           |
          | WebUSB                                    | HTTP interne
          v                                           v
+-------------------+                    +------------------------+
| Zebra Printer     |                    | Labelize service       |
| - EPL / ZPL       |                    | - render EPL / ZPL     |
| - USB endpoint    |                    | - PNG / PDF            |
+-------------------+                    +------------------------+
                                                      |
                                                      v
                                            +--------------------+
                                            | PostgreSQL         |
                                            | templates / audit  |
                                            +--------------------+
```

## Topologie Aspire 13

### Projets

- `ZebraLabels.AppHost`
- `ZebraLabels.ServiceDefaults`
- `ZebraLabels.Api`
- `ZebraLabels.Web`
- `labelize` comme container externe géré par Aspire
- `postgres` comme ressource Aspire

### Wiring attendu

- `AppHost` démarre `postgres`, `labelize`, `api`, `web`.
- `api` reçoit:
  - une chaîne de connexion PostgreSQL,
  - l'URL interne de `labelize`,
  - la télémétrie Aspire.
- `web` reçoit l'URL publique de `api`.

## Principes structurels

### 1. Vertical slice plutôt que couches génériques

Chaque fonctionnalité possède son dossier propre avec:

- endpoint(s),
- request/response,
- validation,
- handler,
- persistence,
- tests.

### 2. Le backend ne parle jamais USB

Le backend:

- ne manipule pas `navigator.usb`,
- ne choisit pas le device,
- ne pousse pas de bytes vers l'imprimante.

Le frontend est l'unique endroit où l'impression USB est autorisée.

### 3. Le backend contrôle la prévisualisation

Le frontend n'appelle pas Labelize directement.

Raisons:

- centraliser la configuration de dimensions,
- imposer les garde-fous,
- encapsuler le protocole Labelize,
- permettre le cache et l'observabilité,
- éviter une dépendance d'API directement exposée au navigateur.

### 4. Impression dans le langage source

- Un template EPL doit pouvoir être imprimé en EPL.
- Un template ZPL doit pouvoir être imprimé en ZPL.
- La conversion EPL -> ZPL n'est **pas** le chemin standard.
- La conversion n'est utilisée qu'en **fallback explicite** si le profil d'imprimante l'autorise.

## Composants backend

### API

- Minimal APIs ou endpoints explicites par slice.
- FluentValidation ou validateurs maison explicites.
- EF Core pour PostgreSQL.
- Intégration `HttpClient` typed pour Labelize.

### Domaine

- Value objects pour dimensions, langage, capacités.
- Pas d'anémie excessive: invariants portés par les types métier.

### Infrastructure

- EF Core mappings par slice.
- Adaptateur `LabelizeClient`.
- Adaptateur `PayloadTransformationService` pour conversions autorisées.

## Composants frontend

- Svelte 5 avec runes.
- Vite.
- TanStack Query ou abstraction simple maison pour appels API.
- Modules dédiés:
  - `features/templates`
  - `features/preview`
  - `features/printers`
  - `features/print-jobs`
  - `lib/webusb`

## Persistance recommandée

### PostgreSQL

Choisi par défaut pour:

- support multi-utilisateur,
- migrations robustes,
- usage naturel avec Aspire,
- meilleure trajectoire production.

## Observabilité

- OpenTelemetry via Aspire.
- Logs structurés sur:
  - preview demandée,
  - preview rendue,
  - conversion effectuée,
  - job d'impression déclaré.

## Sécurité

- Valider taille max des payloads.
- Journaliser les erreurs sans exposer la totalité des labels sensibles en clair.
- Restreindre les CORS au frontend connu.
- Ne jamais accepter un endpoint arbitraire de rendu côté client.
