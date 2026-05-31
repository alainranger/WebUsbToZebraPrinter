# ADR 0003 - WebUSB dans le navigateur

## Statut

Acceptée

## Décision

L'impression USB est initiée uniquement depuis le frontend, via WebUSB.

## Pourquoi

- WebUSB est une API navigateur,
- le consentement utilisateur est requis,
- le serveur ne doit pas prendre la main sur un périphérique client.

## Conséquences

- la logique de connexion et d'envoi USB vit dans `lib/webusb`,
- l'API ne fait qu'orchestrer le contenu à imprimer et l'audit.
