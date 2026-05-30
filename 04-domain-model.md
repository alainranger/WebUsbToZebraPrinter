# Modèle métier

## Enums

### `PrintLanguage`

- `Epl`
- `Zpl`

### `PrintJobStatus`

- `Draft`
- `Submitted`
- `Rendered`
- `Sent`
- `Failed`

## Value objects

### `LabelDimensions`

- `WidthMm`
- `HeightMm`
- `Dpmm`

Invariants:

- largeur > 0
- hauteur > 0
- dpmm dans une liste supportée (`6`, `8`, `12`, `24` par défaut)

### `UsbDeviceFilter`

- `VendorId`
- `ProductId`

### `LanguageCapabilities`

- `SupportsEpl`
- `SupportsZpl`
- `AllowEplToZplFallback`

## Entités

### `LabelTemplate`

- `Id`
- `Name`
- `Description`
- `SourceLanguage`
- `RawContent`
- `Dimensions`
- `Variables`
- `IsArchived`
- `Version`
- `CreatedAtUtc`
- `UpdatedAtUtc`

Invariants:

- `RawContent` non vide
- `SourceLanguage` cohérent avec le contenu attendu
- `Version` incrémentée sur chaque modification

### `TemplateVariable`

- `Name`
- `DisplayName`
- `IsRequired`
- `DefaultValue`
- `ExampleValue`
- `Order`

### `PrinterProfile`

- `Id`
- `Name`
- `UsbFilter`
- `Capabilities`
- `PreferredLanguage`
- `DefaultDimensions`
- `Notes`

Invariants:

- `PreferredLanguage` doit être supporté par `Capabilities`

### `PrintJob`

- `Id`
- `TemplateId`
- `PrinterProfileId`
- `RequestedLanguage`
- `EffectiveLanguage`
- `RenderedOutputType`
- `Status`
- `FailureReason`
- `SubmittedAtUtc`
- `SentAtUtc`

### `PrintPayload`

- `Content`
- `Checksum`
- `Language`

## Domain services

### `TemplateRenderingService`

- remplace les variables,
- valide que toutes les variables requises sont présentes,
- produit un `PrintPayload`.

### `PayloadTransformationService`

- exécute une transformation EPL -> ZPL uniquement quand explicitement autorisée,
- journalise toute transformation.

### `PreviewRequestPolicy`

- applique la taille max,
- choisit le format de rendu,
- impose les dimensions.

