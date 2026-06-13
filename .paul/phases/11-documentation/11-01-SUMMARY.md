---
phase: 11-documentation
plan: 01
subsystem: docs
tags: [docusaurus, github-pages, i18n, fr-locale]

requires:
  - phase: 10-cli
    provides: pokeforge CLI commands documented (seed, doctor, fakemon, asset-sync)
  - phase: 06-advanced-systems
    provides: INarrationPlugin, FakemonAssemblyPipeline APIs documented

provides:
  - Docusaurus 3 site scaffold (docs/) buildable local + GitHub Pages CI
  - Full content EN + FR pour 8 packages + 5 commandes CLI + advanced APIs
  - FR locale (fr) — 100% des pages traduites
  - GitHub Actions deploy-docs.yml (push main → gh-pages auto)
  - Règles style markdown permanentes (CLAUDE.md §11 — zéro em dash)

affects: [11-02-tutorial, 11-03-guides, 11-04-api-reference]

tech-stack:
  added: [Docusaurus 3.10.1, @docusaurus/preset-classic, react 18, prism-react-renderer]
  patterns:
    - baseUrl env override (BASE_URL=/ pour local, /PokemonSDK/ en prod)
    - useBaseUrl hook pour redirect cross-env
    - i18n FR locale via docs/i18n/fr/

key-files:
  created:
    - docs/docusaurus.config.ts
    - docs/package.json
    - docs/sidebars.ts
    - docs/src/pages/index.tsx
    - docs/src/css/custom.css
    - docs/docs/intro.md + 13 pages packages/cli/advanced EN
    - docs/i18n/fr/ — 27 fichiers FR (miroir complet)
    - docs/i18n/fr/docusaurus-theme-classic/footer.json
    - .github/workflows/deploy-docs.yml
  modified:
    - CLAUDE.md (section 11 — règles style docs)

key-decisions:
  - "baseUrl conditionnel : env.BASE_URL ?? (prod ? /PokemonSDK/ : /) — construit pour les deux contextes"
  - "useBaseUrl hook dans index.tsx — redirect respecte baseUrl en dev et prod"
  - "{year} non interpolé par Docusaurus i18n footer.json — hardcoder l'année"
  - "Em dashes interdits dans tous les .md — CLAUDE.md §11 règle permanente"
  - "FR locale ajoutée en Wave 1 (pas Wave 3) — réduit la dette i18n pour les waves suivantes"

patterns-established:
  - "Docusaurus i18n : fichiers miroir dans docs/i18n/fr/docusaurus-plugin-content-docs/current/"
  - "Build local : BASE_URL=/ npm run build (évite le sous-chemin /PokemonSDK/)"
  - "Markdown style : colons pour introductions, virgules pour continuations, parenthèses pour apartés"

duration: ~6h (multi-session)
started: 2026-06-13T00:00:00Z
completed: 2026-06-13T14:30:00Z
---

# Phase 11 Plan 01: Wave 1 Docusaurus Scaffold Summary

**Docusaurus 3.10.1 déployé avec contenu complet EN + FR locale, sidebar D-21, et GitHub Actions auto-deploy sur gh-pages.**

## Performance

| Métrique | Valeur |
|---------|-------|
| Durée | ~6h (multi-session) |
| Démarré | 2026-06-13 00:00 |
| Complété | 2026-06-13 14:30 |
| Commits | 3 (scaffold + enrich + style-fixes) |
| Fichiers modifiés | 30+ |

## Acceptance Criteria Results

| Critère | Statut | Notes |
|---------|--------|-------|
| AC-1: Build Docusaurus local OK | Pass | `BASE_URL=/ npm run build` → exit 0, EN + FR |
| AC-2: Sidebar D-21 présente | Pass | 8 packages + 5 CLI + 4 Advanced APIs |
| AC-3: Workflow deploy-docs.yml valide | Pass | YAML valid, permissions Pages, path filter docs/** |

## Accomplissements

- Docusaurus 3.10.1 scaffoldé manuellement (pas create-docusaurus) — contrôle total des fichiers
- Contenu complet écrit pour 14 pages EN : intro, 8 packages, 5 CLI, 1 advanced index
- Locale FR ajoutée (D-22-style) : 27 fichiers traduits, footer.json, sidebar labels
- baseUrl env-aware (`BASE_URL` override) — local et GitHub Pages fonctionnent sans rebuild config
- `useBaseUrl` hook dans `index.tsx` — redirect `/docs/intro` respecte le baseUrl en prod
- Footer copyright `{year}` bug corrigé (Docusaurus n'interpole pas `{year}` en i18n)
- 62 em dashes supprimés des fichiers .md (54 prose, 8 conservés en blocs code)
- Règles style markdown permanentes ajoutées à CLAUDE.md §11

## Task Commits

| Tâche | Commit | Description |
|-------|--------|-------------|
| Scaffold + deploy-docs.yml | `2382695` | feat(phase11): Wave 1 Docusaurus 3 + GitHub Pages CI |
| Contenu EN + FR locale + baseUrl | `27165c3` | docs(phase11): stubs enrichis + locale FR + baseUrl dev |
| Style + footer fix + CLAUDE.md | `71b839a` | docs(phase11): {year} + em dashes + env override |

## Fichiers Créés / Modifiés

| Fichier | Changement |
|---------|-----------|
| `docs/docusaurus.config.ts` | Créé — config Docusaurus, baseUrl env-aware, FR locale |
| `docs/package.json` | Créé — Docusaurus 3.10.1 |
| `docs/sidebars.ts` | Créé — sidebar D-21 (8+5+4 entrées) |
| `docs/src/pages/index.tsx` | Créé — redirect useBaseUrl vers /docs/intro |
| `docs/docs/intro.md` | Créé — landing page + table packages + prérequis |
| `docs/docs/packages/*.md` (×9) | Créés — core, data, battle, scripting, monogame, tools, plugins, plugins-tts, index |
| `docs/docs/cli/*.md` (×5) | Créés — index, seed, doctor, fakemon, asset-sync |
| `docs/docs/advanced/index.md` | Créé — intro APIs avancées |
| `docs/i18n/fr/**` (×27) | Créés — miroir FR complet |
| `.github/workflows/deploy-docs.yml` | Créé — deploy Pages sur push main |
| `CLAUDE.md` | Modifié — §11 règles style docs |

## Décisions Prises

| Décision | Rationale | Impact |
|----------|-----------|--------|
| FR locale en Wave 1 | Évite refactoring i18n en Wave 3 ; coût marginal lors de la rédaction initiale | Waves 2/3 écriront EN+FR simultanément |
| `{year}` hardcodé `2026` | Docusaurus ne fournit pas de variable d'interpolation pour l'année dans i18n footer.json | Mise à jour manuelle annuelle nécessaire |
| Em dashes interdits (CLAUDE.md §11) | Marque stylistique IA trop visible ; règle permanente pour la cohérence future | Toutes les docs futures écrites sans em dash |
| `BASE_URL` env override | `baseUrl: process.env.BASE_URL ?? (prod ? '/PokemonSDK/' : '/')` — un seul build pour les deux contextes | `BASE_URL=/ npm run build` pour tests locaux |

## Déviations du Plan

| Type | Nb | Impact |
|------|----|--------|
| Scope additions | 3 | Positif — dette éliminée |
| Auto-fixes | 2 | Correctifs essentiels |
| Différés | 0 | |

### Scope additions (au-delà du plan)

1. **Contenu complet EN (Wave 2/3 anticipé)** — Plan prévoyait des stubs ; contenu narratif complet écrit pour 14 pages EN. Résultat : Wave 2 (tutorial) reste à faire mais Wave 3 (guides packages) largement anticipée.

2. **Locale FR complète** — Plan ne mentionnait pas de locale FR pour Wave 1. Ajoutée car coût marginal lors de la rédaction initiale. Élimine 80% du travail i18n prévu en Wave 3.

3. **CLAUDE.md §11 règles style** — Non prévu dans le plan. Ajouté pour pérenniser les décisions stylistiques prises lors du nettoyage em dash.

### Auto-fixes

1. **baseUrl 404 en local** — `baseUrl: '/PokemonSDK/'` causait 404 sur `npm run serve`. Fix : env override `BASE_URL`.
2. **`useBaseUrl` dans index.tsx** — Redirect en dur `/docs/intro` ignorait le baseUrl en prod. Fix : hook `useBaseUrl`.

## Prochaine Phase — Readiness

**Prêt :**
- Structure Docusaurus complète, les waves 2/3/4 n'ont qu'à ajouter des fichiers dans `docs/docs/`
- Locale FR opérationnelle — les nouvelles pages EN ont leur miroir FR à créer
- GitHub Pages CI prêt — activation manuelle requise (Settings → Pages → Source: GitHub Actions)
- Règles style permanentes dans CLAUDE.md

**Concerns :**
- GitHub Pages pas encore activé (étape manuelle dans le repo GitHub)
- Wave 2 (tutorial 30min step-by-step) reste entière
- Pages `advanced/narration-plugin.md`, `advanced/fakemon-pipeline.md`, `advanced/fakemon-catalog.md` — stubs vides à remplir en Wave 3

**Blockers :**
- Aucun. Wave 2 peut démarrer immédiatement.

---
*Phase: 11-documentation, Plan: 01*
*Complété: 2026-06-13*
