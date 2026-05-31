# ADR 0001 - Prévisualisation self-hosted

## Statut

Acceptée

## Décision

La prévisualisation repose sur **Labelize self-hosted** et transite via le backend.

## Pourquoi

- pas de dépendance à un service externe non contrôlé,
- support EPL et ZPL,
- maîtrise des performances, de la confidentialité et du versioning.

## Conséquences

- un conteneur supplémentaire est requis dans Aspire,
- l'API doit encapsuler les erreurs du moteur de rendu.
