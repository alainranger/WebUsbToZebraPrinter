# Flux de prévisualisation et d'impression

## Prévisualisation

### Chemin nominal (prévisualisation)

1. Le frontend récupère le template ou le brouillon courant.
2. Les variables sont substituées côté backend.
3. L'API construit une requête Labelize:
   - `Content-Type: application/epl` ou `application/zpl`
   - query `width`, `height`, `dpmm`, `output`
4. Labelize retourne `PNG` ou `PDF`.
5. L'API renvoie le binaire au frontend.
6. Le frontend affiche:
   - image inline pour `PNG`,
   - `iframe` ou download pour `PDF`.

## Impression

### Chemin nominal (impression)

1. Le frontend demande à l'utilisateur de connecter une imprimante Zebra via WebUSB.
2. Le frontend résout l'endpoint `out`.
3. Le frontend récupère le payload final auprès du backend ou depuis le template déjà résolu.
4. Le frontend envoie le contenu brut à l'imprimante.
5. Le backend journalise le job.

## Décision clé

La **prévisualisation** et l'**impression** n'utilisent pas obligatoirement le même format de sortie:

- preview: `PNG` ou `PDF`
- impression: **toujours EPL ou ZPL**

## Gestion EPL / ZPL

### Règle principale

Le système conserve le **langage source** du template et l'utilise pour l'impression.

### Fallback de conversion

Autorisé uniquement si:

- le profil d'imprimante l'exige,
- le profil l'autorise explicitement,
- la transformation est tracée,
- l'utilisateur voit le langage effectif envoyé.

## Recommandation sur Labelize

Labelize supporte nativement **EPL et ZPL**. Le rendu ne doit donc pas imposer une conversion préalable.

## Erreurs à traiter explicitement

- payload vide,
- dimensions invalides,
- syntaxe EPL/ZPL non rendable,
- périphérique USB non sélectionné,
- endpoint `out` introuvable,
- transfert USB non confirmé,
- profil d'imprimante incompatible avec le langage demandé.

## Cache preview

Option recommandée:

- clé de cache = hash(`language`, `content`, `dimensions`, `outputType`)
- courte durée
- invalidation naturelle par changement de hash
