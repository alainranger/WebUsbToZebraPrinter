# Structure de solution recommandée

```text
src/
  ZebraLabels.AppHost/
  ZebraLabels.ServiceDefaults/
  ZebraLabels.Api/
    Features/
      PrinterProfiles/
        Create/
        Update/
        GetList/
      Templates/
        Create/
        Update/
        GetById/
        GetList/
      Preview/
        RenderRaw/
        RenderTemplate/
      PrintJobs/
        Create/
        GetById/
        GetList/
    Domain/
      Templates/
      Printers/
      Printing/
    Infrastructure/
      Persistence/
      Preview/
      Transformations/
    Program.cs
  ZebraLabels.Web/
    src/
      features/
        templates/
        preview/
        printers/
        print-jobs/
      lib/
        api/
        webusb/
        ui/
tests/
  ZebraLabels.Api.UnitTests/
  ZebraLabels.Api.IntegrationTests/
  ZebraLabels.Web.Tests/
```

## Conventions

- Noms de slices au pluriel.
- Un endpoint = un dossier.
- Une request = un fichier.
- Un handler = un fichier.
- Un validator = un fichier.
- Un mapping EF par agrégat.

## Conventions backend

- préférer des records immuables pour les contrats,
- préférer `sealed` quand approprié,
- utiliser `TimeProvider`,
- injecter les dépendances explicitement.

## Conventions frontend

- composants suffixés clairement (`Page`, `Pane`, `Card`, `Form`),
- logique API dans `lib/api`,
- logique WebUSB dans `lib/webusb`,
- types partagés par feature.
