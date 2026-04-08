# Lab Unity — Pong

Projet Unity pour le lab d'initiation au développement assisté par IA (Claude Code).

## Pré-requis

- **Unity 6** — version exacte : `6000.4.0f1` (Unity Hub > Installs > Install Editor > Archive)
- **Claude Code** — installé et fonctionnel ([documentation](https://docs.anthropic.com/en/docs/claude-code))
- **MCP for Unity** — le plugin qui connecte Claude Code à l'éditeur Unity
  ```bash
  # Installer le serveur MCP
  pip install mcpforunityserver
  ```

## Setup

### 1. Forker le repo

Chaque binôme fork ce repo sur GitHub, puis clone son fork :
```bash
gh repo fork liftia/lab-unity-pong --clone
cd lab-unity-pong
```

### 2. Ouvrir dans Unity

1. Ouvrir Unity Hub
2. **Add** > **Add project from disk** > sélectionner le dossier `lab-unity-pong`
3. Vérifier que la version `6000.4.0f1` est utilisée
4. Ouvrir le projet — la première ouverture peut prendre quelques minutes (import des assets)

### 3. Vérifier que le jeu tourne

1. Ouvrir la scène `Assets/Scenes/Bootloader_Scene.unity`
2. Appuyer sur **Play**
3. Joueur 1 : touches **W** / **S** — Joueur 2 : touches **flèche haut** / **flèche bas**
4. Le jeu doit fonctionner en mode 2 joueurs local

### 4. Configurer Claude Code + MCP

Créer un fichier `.mcp.json` à la racine du projet :
```json
{
  "mcpServers": {
    "unityMCP": {
      "command": "uvx",
      "args": ["--from", "mcpforunityserver", "mcp-for-unity"]
    }
  }
}
```

Dans Unity : **Window** > **MCP for Unity** pour vérifier que la connexion est active.

### 5. Lancer Claude Code

```bash
claude
```

Vérifier que Claude Code détecte le projet Unity et le serveur MCP.

## Exercices

Les exercices sont dans les **Issues** de ce repo. Faites-les dans l'ordre :

| # | Issue | Durée | Objectif |
|---|-------|-------|----------|
| 0 | **TS1** — Prise en main | 20 min | Comprendre l'architecture du projet |
| 1 | **US1** — Mode solo | 50 min | Implémenter une IA adverse |
| 2 | **US2** — Visuel néon | 70 min | Transformer l'esthétique du jeu |
| 3 | **US3** — Power-ups | 60 min | Ajouter des bonus en jeu |
| 4 | **US4** — Mode quiz | 70 min | Intégrer des questions éducatives |
| 5 | **US5** — Audio et polish | 45 min | Son procédural et finitions |

## Organisation du travail

- Travaillez **en binôme** sur un seul fork
- Utilisez **Claude Code** pour toutes les implémentations
- Chaque exercice a un temps d'implémentation, un temps d'observation/test, et un débrief collectif
- Suivez les critères d'acceptation de chaque issue

## Architecture du projet

Le projet est construit sur le pattern **Event-Driven avec ScriptableObjects** :

- `Assets/PaddleBall/` — Code et assets spécifiques au jeu
- `Assets/Core/` — Framework réutilisable (events, UI, utilities)
- Les systèmes communiquent via des `EventChannelSO` (publish/subscribe)
- La configuration du gameplay est dans des ScriptableObjects (`GameDataSO`, `LevelLayoutSO`)

La première tâche (TS1) vous guidera dans l'exploration détaillée de cette architecture.
