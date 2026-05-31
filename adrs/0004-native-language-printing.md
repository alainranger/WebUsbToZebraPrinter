# ADR 0004 - Impression dans le langage natif

## Statut

Acceptée

## Décision

Un template est imprimé prioritairement dans son **langage source** (`EPL` ou `ZPL`).

## Pourquoi

- éviter les écarts entre preview, intention métier et payload final,
- minimiser les conversions et les pertes de fidélité,
- rendre les comportements plus prévisibles.

## Conséquences

- la conversion EPL -> ZPL devient un fallback contrôlé,
- le profil d'imprimante doit exposer ses capacités.
