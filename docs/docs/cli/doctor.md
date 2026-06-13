---
sidebar_position: 3
---

# pokeforge doctor

Health-check command — verifies all runtime dependencies and reports what is missing.

## Usage

```bash
pokeforge doctor [--db <path>]
```

| Flag | Default | Description |
|------|---------|-------------|
| `--db` | `data/PokemonSDK.db` | Path to the SQLite database to inspect |

## Checks

| Check | What it verifies |
|-------|-----------------|
| SDL2 | `libSDL2` shared library present and loadable |
| OpenAL | `libopenal` shared library present (audio) |
| Piper | `piper` binary on PATH (TTS narration) |
| aplay | `aplay` command available (Linux audio playback) |
| Database | SQLite file exists and all expected tables are present |
| Migrations | EF Core migrations are applied and up to date |

## Example output

```
pokeforge doctor

✓ SDL2          libSDL2-2.0.so.0 found
✓ OpenAL        libopenal.so.1 found
✓ Piper         piper 1.2.0 found at /usr/local/bin/piper
✗ aplay         not found — install alsa-utils (apt install alsa-utils)
✓ Database      data/PokemonSDK.db — 12 tables, migrations current
```

Exit code is `0` if all checks pass, `1` if any check fails — suitable for CI.

## Fix suggestions

`doctor` prints a fix suggestion for each failed check:

```
✗ SDL2    not found — install libsdl2-dev (apt install libsdl2-dev)
```
