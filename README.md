# Coi Ship Mod

Mod de vaisseaux spatiaux pour Captain of Industry, version cible : 0.8.8d update 4.3.

## Objectif

Le mod ajoute un arbre de recherche dédié aux vaisseaux spatiaux, inspiré de la logique de fabrication des fusées du jeu :
- recherche racine : Vaisseaux spatiaux
- sous-recherches : cargo, scientifique, militaire
- coûts avancés inspirés de la chaîne de production du jeu
- extension simple pour le futur ajout de vrais prototypes de vaisseau

## Fichiers du projet

- `CoiShipMod.csproj` : projet .NET pour la compilation du mod.
- `CoiShipMod.cs` : point d'entrée du mod.
- `SpacecraftPrototypeData.cs` : données de recherche et progression du mod.
- `manifest.json` : déclaration du mod pour le chargeur du jeu.
- `Assets/CoiShipMod/` : emplacement prévu pour icônes et modèles.

## Philosophie de progression

- Atelier de vaisseaux spatiaux : équivalent de l’atelier de fusées
- Cargo spatial : transport / logistique
- Vaisseau scientifique : exploration / recherche
- Vaisseau militaire : défense / projection spatiale

## Coûts de référence

- Cargo spatial : fer 800, aluminium 500, pièces 350, électronique 220, carburant 300
- Vaisseau scientifique : fer 650, aluminium 450, électronique 500, cuivre 220, carburant 350, pièces 280
- Vaisseau militaire : fer 1200, aluminium 700, électronique 700, cuivre 300, carburant 500, pièces 420

## Compilation

Le projet ne hardcode plus un chemin absolu fixe. Il essaie automatiquement les emplacements standards de Steam, puis utilise la variable d’environnement `COI_MANAGED_DIR` si elle est définie.

```powershell
$env:COI_MANAGED_DIR = "C:\Program Files (x86)\Steam\steamapps\common\Captain of Industry\Captain of Industry_Data\Managed"
dotnet build CoiShipMod.csproj -c Release
```

Ou simplement :

```powershell
dotnet build CoiShipMod.csproj -c Release
```

## Note importante

Ce projet est conçu comme une base propre et directement exploitable pour la création d’un mod COI. La structure de recherche et la logique de coûts sont finalisées pour 0.8.8d update 4.3. La partie “prototype exact de vaisseau” doit ensuite être alignée sur les types réels du jeu pour les objets de production si tu veux remplacer les stubs par les vraies entités runtime.
