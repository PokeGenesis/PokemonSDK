---
sidebar_position: 4
---

# pokeforge fakemon

Gérer les sprites Fakemon: lister les parties disponibles et assembler des sprites composites.

## list-parts

Liste toutes les images de parties trouvées dans un répertoire, groupées par catégorie.

```bash
pokeforge fakemon list-parts [--parts-dir <chemin>]
```

| Option | Défaut | Description |
|--------|--------|-------------|
| `--parts-dir` | `assets/fakemon-parts/` | Répertoire contenant les images PNG de parties |

### Exemple de sortie

```
pokeforge fakemon list-parts

body/
  dragon_base.png
  serpent_base.png
  felin_base.png

head/
  electric_head.png
  fire_head.png
  psychic_head.png

tail/
  spiked_tail.png
  flame_tail.png
```

## assemble

Assemble un sprite Fakemon composite en superposant des images de parties.

```bash
pokeforge fakemon assemble \
  --identifier <id> \
  --parts-dir <chemin> \
  --output <répertoire> \
  [--body <partie>] \
  [--head <partie>] \
  [--tail <partie>] \
  [--views front,back,overworld,portrait,icon]
```

| Option | Description |
|--------|-------------|
| `--identifier` | Identifiant Fakemon (ex. `dragon-electrique`) |
| `--parts-dir` | Répertoire des images de parties |
| `--output` | Répertoire de sortie (sprites sauvegardés en `fk_{identifiant}_{vue}.png`) |
| `--body` | Nom du fichier de la partie corps (sans extension) |
| `--head` | Nom du fichier de la partie tête |
| `--tail` | Nom du fichier de la partie queue (optionnel) |
| `--views` | Vues à générer séparées par virgule (défaut : `front,back,overworld,portrait,icon`) |

### Exemple

```bash
pokeforge fakemon assemble \
  --identifier dragon-electrique \
  --body dragon_base \
  --head electric_head \
  --tail spiked_tail \
  --parts-dir assets/fakemon-parts/ \
  --output assets/sprites/fakemons/
```

Fichiers de sortie conformes à la convention D-16 :

```
assets/sprites/fakemons/
  fk_dragon-electrique_front.png
  fk_dragon-electrique_back.png
  fk_dragon-electrique_overworld.png
  fk_dragon-electrique_portrait.png
  fk_dragon-electrique_icon.png     ← 32×32 pixels
```
