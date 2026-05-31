# Spécification frontend

## Choix

- **Svelte 5 avec runes**
- **Vite**
- **SPA** simple, sans dépendance à un framework SSR
- TypeScript strict

## Structure recommandée

```text
src/
  app/
    App.svelte
    router.ts
  features/
    templates/
    preview/
    printers/
    print-jobs/
  lib/
    api/
    forms/
    webusb/
    ui/
  routes/
    templates/
    workbench/
    printers/
    history/
```

## Règles Svelte 5

- Utiliser les **runes** (`$state`, `$derived`, `$effect`) pour l'état local.
- Limiter les stores globaux aux cas partagés réels.
- Isoler `navigator.usb` dans `lib/webusb`.
- Garder les composants présentations purs quand possible.

## Vues principales

### `TemplatesPage`

- liste,
- création,
- archivage,
- duplication.

### `WorkbenchPage`

- éditeur source EPL/ZPL,
- panneau variables,
- panneau prévisualisation,
- panneau impression.

### `PrintersPage`

- profils d'imprimantes,
- diagnostic WebUSB,
- aide de configuration navigateur.

### `HistoryPage`

- derniers jobs,
- statut,
- erreurs.

## Services frontend

### `apiClient`

- encapsule `fetch`,
- gère `problem+json`,
- ajoute corrélation si nécessaire.

### `webUsbPrinterClient`

Responsabilités:

- `requestDevice(filters)`
- `open(device)`
- `claimOutEndpoint(device)`
- `send(content)`
- `disconnect()`

### `previewPresenter`

- gère l'URL blob de preview,
- nettoie les objets URL,
- permet retry/cancel.

## UX imposée

- Afficher clairement si le navigateur n'est pas compatible WebUSB.
- Afficher les dimensions et le langage du template courant.
- Désactiver l'impression tant que le device n'est pas connecté.
- Distinguer:
  - erreur de validation,
  - erreur de rendu,
  - erreur USB,
  - erreur de profil d'imprimante.

## Composants à générer en priorité

- `TemplateEditor.svelte`
- `TemplateVariablesForm.svelte`
- `PreviewPane.svelte`
- `PrinterConnectionCard.svelte`
- `PrintActionBar.svelte`
- `ProblemAlert.svelte`
