---
sidebar_position: 3
---

# pokeforge doctor

Commande de diagnostic — vérifie toutes les dépendances runtime et signale ce qui manque.

## Utilisation

```bash
pokeforge doctor [--db <chemin>]
```

| Option | Défaut | Description |
|--------|--------|-------------|
| `--db` | `data/PokemonSDK.db` | Chemin vers la base SQLite à inspecter |

## Vérifications

| Vérification | Ce qu'elle contrôle |
|--------------|---------------------|
| SDL2 | Bibliothèque partagée `libSDL2` présente et chargeable |
| OpenAL | Bibliothèque partagée `libopenal` présente (audio) |
| Piper | Binaire `piper` dans le PATH (narration TTS) |
| aplay | Commande `aplay` disponible (lecture audio Linux) |
| Base de données | Le fichier SQLite existe et toutes les tables attendues sont présentes |
| Migrations | Les migrations EF Core sont appliquées et à jour |

## Exemple de sortie

```
pokeforge doctor

✓ SDL2          libSDL2-2.0.so.0 trouvé
✓ OpenAL        libopenal.so.1 trouvé
✓ Piper         piper 1.2.0 trouvé à /usr/local/bin/piper
✗ aplay         non trouvé — installez alsa-utils (apt install alsa-utils)
✓ Base de données  data/PokemonSDK.db — 12 tables, migrations à jour
```

Code de sortie `0` si toutes les vérifications passent, `1` si au moins une échoue — utilisable en CI.

## Suggestions de correction

`doctor` affiche une suggestion de correction pour chaque vérification échouée :

```
✗ SDL2    non trouvé — installez libsdl2-dev (apt install libsdl2-dev)
```
