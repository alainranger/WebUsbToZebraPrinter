# Notes issues des références

## POC `EplToPdfConverter`

Le POC fournit trois idées réutilisables:

1. un **EplBuilder** pour générer rapidement de l'EPL,
2. un **convertisseur EPL -> ZPL** simple,
3. une démo **WebUSB** côté navigateur.

## Ce qu'il faut reprendre

- la logique que le navigateur est responsable de WebUSB,
- l'idée d'un builder ou service de construction de payload,
- l'existence d'un adaptateur EPL -> ZPL de fallback.

## Ce qu'il faut changer

- supprimer toute dépendance à **Labelary**,
- ne pas limiter la preview au ZPL,
- ne pas mélanger démonstration et architecture cible,
- structurer le backend par vertical slices,
- formaliser les profils d'imprimantes et les jobs d'impression.

## Labelize

Labelize est le moteur de rendu retenu car il:

- supporte **ZPL et EPL**,
- produit **PNG et PDF**,
- fonctionne en **HTTP microservice**,
- peut être **self-hosted**.

