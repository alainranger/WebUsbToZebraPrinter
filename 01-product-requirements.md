# Exigences produit

## Vision

Fournir une application web interne capable de:

1. créer ou éditer des étiquettes en **EPL** ou **ZPL**,
2. les **prévisualiser localement** sans dépendance externe,
3. les **imprimer via WebUSB** sur une imprimante Zebra,
4. conserver un historique minimal des templates et des impressions.

## Acteurs

- **Opérateur**: imprime des étiquettes à partir de templates existants.
- **Superviseur**: gère les templates, profils d'imprimantes et dimensions par défaut.
- **Développeur**: ajoute des slices, des commandes EPL/ZPL et des tests.

## Cas d'usage principaux

### UC1 - Gérer un template

- Créer un template nommé.
- Choisir le langage source: `EPL` ou `ZPL`.
- Définir largeur, hauteur, dpmm.
- Saisir le corps brut ou une version paramétrée avec variables.
- Versionner la modification.

### UC2 - Prévisualiser une étiquette

- Saisir des valeurs de variables.
- Demander un rendu `PNG` ou `PDF`.
- Obtenir un aperçu cohérent avec le langage source.
- Voir clairement les erreurs de syntaxe ou de dimensions.

### UC3 - Connecter une imprimante Zebra

- Déclencher `navigator.usb.requestDevice`.
- Sélectionner un périphérique autorisé.
- Vérifier la présence d'un endpoint `out`.
- Persister la préférence de profil côté navigateur, jamais le handle USB côté serveur.

### UC4 - Imprimer

- Imprimer depuis un template ou un brouillon.
- Envoyer **le payload EPL ou ZPL brut** via WebUSB.
- Journaliser le job d'impression côté backend avec statut `Submitted`, `Sent`, `Failed`.

### UC5 - Gérer les profils d'imprimantes

- Définir `vendorId`, `productId`, `languageCapabilities`, `preferredLanguage`, `dpmm`, dimensions par défaut.
- Choisir si une conversion explicite `EPL -> ZPL` est autorisée.

## Contraintes non fonctionnelles

- **Offline-friendly** pour la prévisualisation en environnement interne.
- **Pas d'API tierce** pour le rendu.
- **Sécurité**: l'impression doit rester initiée par l'utilisateur depuis le navigateur.
- **Traçabilité**: journaliser les previews et impressions.
- **Testabilité**: séparation claire entre logique métier, intégrations et UI.
- **Maintenabilité**: slices verticales, code explicite, pas de couches génériques inutiles.

## Hors périmètre du MVP

- Gestion avancée des rôles et authentification.
- Impression réseau via socket TCP ou USB natif côté serveur.
- Designer visuel WYSIWYG complet.
- Gestion de files d'impression distribuées.

## Critères d'acceptation MVP

- Un utilisateur peut créer un template EPL ou ZPL.
- Il peut prévisualiser ce template sans quitter l'application.
- Il peut connecter une imprimante Zebra compatible WebUSB.
- Il peut imprimer un template avec variables substituées.
- Les erreurs de rendu ou d'impression remontent avec un message actionnable.
