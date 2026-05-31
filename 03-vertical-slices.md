# Découpage en vertical slices

## Backend

### Slice `Templates`

Responsabilités:

- CRUD des templates.
- Gestion des versions.
- Variables et valeurs par défaut.
- Validation métier du langage et des dimensions.

Endpoints initiaux:

- `GET /api/templates`
- `POST /api/templates`
- `GET /api/templates/{id}`
- `PUT /api/templates/{id}`

### Slice `Preview`

Responsabilités:

- Valider les requêtes de rendu.
- Résoudre le payload final.
- Appeler Labelize.
- Retourner PNG ou PDF.

Endpoints initiaux:

- `POST /api/preview/render`
- `POST /api/templates/{id}/render`

### Slice `PrinterProfiles`

Responsabilités:

- Gérer les caractéristiques des imprimantes supportées.
- Déclarer les langages acceptés.
- Définir le fallback autorisé ou non.

Endpoints initiaux:

- `GET /api/printer-profiles`
- `POST /api/printer-profiles`
- `PUT /api/printer-profiles/{id}`

### Slice `PrintJobs`

Responsabilités:

- Enregistrer une intention d'impression.
- Conserver l'audit du payload effectivement envoyé.
- Suivre le statut métier du job.

Endpoints initiaux:

- `POST /api/print-jobs`
- `GET /api/print-jobs/{id}`
- `GET /api/print-jobs`

### Slice `Health`

Responsabilités:

- Readiness du backend.
- Vérification de dépendance vers Labelize et PostgreSQL.

## Frontend

### Slice `templates`

- liste des templates,
- éditeur EPL/ZPL,
- formulaire de variables,
- métadonnées de dimensions.

### Slice `preview`

- aperçu PNG/PDF,
- bascule de format,
- affichage des erreurs de rendu,
- annulation/retry.

### Slice `printers`

- sélection du profil d'imprimante,
- connexion WebUSB,
- diagnostic endpoint/interface,
- état de disponibilité.

### Slice `print-jobs`

- résumé avant impression,
- historique local de session,
- retour d'erreur détaillé.

## Règles de conception

- Chaque slice possède ses propres DTOs.
- Pas de dossier global `Services` fourre-tout.
- Le code partagé reste minimal:
  - primitives métier,
  - infrastructure commune,
  - utilitaires transverses très ciblés.

## Ordre d'implémentation

1. `PrinterProfiles`
2. `Templates`
3. `Preview`
4. `PrintJobs`
5. `Health` et observabilité
