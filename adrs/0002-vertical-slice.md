# ADR 0002 - Vertical slice architecture

## Statut

Acceptée

## Décision

Le backend et le frontend sont organisés par **features** et non par couches techniques globales.

## Pourquoi

- lecture plus directe du flux métier,
- meilleure testabilité,
- moindre couplage transversal,
- alignement avec la préférence utilisateur.

## Conséquences

- duplication légère acceptable entre slices,
- shared code maintenu au strict minimum.
