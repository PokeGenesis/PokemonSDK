---
phase: 11-documentation
plan: 03
subsystem: docs
tags: [docusaurus, guides, i18n, fr, en, docs, battle-engine, lua, plugins, assets, rendering, tts, fakemons]

requires:
  - phase: 11-02
    provides: Tutorial 30min EN+FR, sidebar structure, numberPrefixParser pattern

provides:
  - "8 EN guide pages covering all 7 major SDK subsystems"
  - "8 FR guide mirrors with bilingual parity"
  - "Guides sidebar category (between Tutorial and Packages)"
  - "FR sidebar label in current.json"

affects: "11-04 (API reference) — guide links established, advanced/ cross-links tested"

tech-stack:
  added: []
  patterns:
    - "Links from guides/index.md to OTHER guides use doc-ID style: guides/battle-engine (not ./battle-engine)"
    - "Cross-category links (guides -> advanced) use relative file path: ../advanced/narration-plugin"
    - "Links to tutorial from guides: avoid hyperlink (URL math breaks with docs routeBasePath)"
    - "Em dashes in fenced CLI output blocks are the ONLY valid exception per CLAUDE.md §11"

key-files:
  created:
    - docs/docs/guides/index.md
    - docs/docs/guides/battle-engine.md
    - docs/docs/guides/lua-scripting.md
    - docs/docs/guides/plugins.md
    - docs/docs/guides/asset-pipeline.md
    - docs/docs/guides/rendering-hd.md
    - docs/docs/guides/tts-narration.md
    - docs/docs/guides/fakemons.md
    - docs/i18n/fr/docusaurus-plugin-content-docs/current/guides/index.md
    - docs/i18n/fr/docusaurus-plugin-content-docs/current/guides/battle-engine.md
    - docs/i18n/fr/docusaurus-plugin-content-docs/current/guides/lua-scripting.md
    - docs/i18n/fr/docusaurus-plugin-content-docs/current/guides/plugins.md
    - docs/i18n/fr/docusaurus-plugin-content-docs/current/guides/asset-pipeline.md
    - docs/i18n/fr/docusaurus-plugin-content-docs/current/guides/rendering-hd.md
    - docs/i18n/fr/docusaurus-plugin-content-docs/current/guides/tts-narration.md
    - docs/i18n/fr/docusaurus-plugin-content-docs/current/guides/fakemons.md
  modified:
    - docs/sidebars.ts
    - docs/i18n/fr/docusaurus-plugin-content-docs/current.json

key-decisions:
  - "guides/index.md internal links use doc-ID style (guides/battle-engine), confirmed working by build"
  - "Cross-category links use ../ relative: ../advanced/narration-plugin"
  - "Tutorial link removed from guides/index intro (URL resolution breaks with routeBasePath=docs)"

patterns-established:
  - "From guides/index.md, link to other guides with doc-ID (guides/X), link cross-category with ../category/page"
  - "Never link from docs page to tutorial/index — use plain text mention instead"

duration: ~25min
started: 2026-06-13T20:31:00Z
completed: 2026-06-13T20:56:00Z
---

# Phase 11 Plan 03: Guides Section EN+FR Summary

**Bilingual Guides section (16 pages) covering 7 SDK subsystems: build exits 0 EN+FR, 0 broken links, 0 em dashes.**

## Performance

| Metric | Valeur |
|--------|--------|
| Durée | ~25 min |
| Démarré | 2026-06-13 ~20h31 |
| Complété | 2026-06-13 ~20h56 |
| Tâches | 3 complétées |
| Fichiers modifiés | 18 (16 créés + 2 modifiés) |

## Acceptance Criteria Results

| Critère | Statut | Notes |
|---------|--------|-------|
| AC-1: 8 pages EN guides avec vrai contenu | Pass | index + 7 topics, code C# et Lua réels, 0 placeholder |
| AC-2: 8 pages FR miroirs avec parité bilingue | Pass | même structure, code en anglais, prose en français |
| AC-3: Guides category dans sidebar + build passe | Pass | `BASE_URL=/ npm run build` exit 0 EN+FR, commit `2f10338` |

## Accomplishments

- 16 pages de guides bilingues créées: parité complète EN/FR
- Guides category ajoutée à `sidebars.ts` (collapsed: false, entre Tutorial et Packages)
- Label FR "Guides" enregistré dans `current.json`
- Build Docusaurus validé: 0 erreur, 0 lien cassé, EN + FR

## Task Commits

| Tâche | Commit | Type | Description |
|-------|--------|------|-------------|
| Task 1+2+3 | `2f10338` | docs | 11-03 Wave 3 - guides section EN+FR (8 pages, sidebar, i18n) |

## Files Created/Modified

| Fichier | Changement | Objet |
|---------|------------|-------|
| `docs/docs/guides/index.md` | Créé | Vue d'ensemble, table des 7 guides |
| `docs/docs/guides/battle-engine.md` | Créé | BattleEngine 1v1, formules, MonoGame |
| `docs/docs/guides/lua-scripting.md` | Créé | MoonSharp SoftSandbox, flags, hot reload |
| `docs/docs/guides/plugins.md` | Créé | IBattlePlugin, Nuzlocke/Randomizer/Turbo |
| `docs/docs/guides/asset-pipeline.md` | Créé | Nommage D-16, AtlasPacker, SqliteSyncer |
| `docs/docs/guides/rendering-hd.md` | Créé | 480x270, xBR x4, 3-pass Draw, DayNight |
| `docs/docs/guides/tts-narration.md` | Créé | PiperNarrationPlugin, sdk.tts Lua API |
| `docs/docs/guides/fakemons.md` | Créé | fk_ naming, FakemonAssemblyPipeline, D-22 |
| `docs/i18n/fr/.../guides/*.md` (x8) | Créé | Miroirs FR complets |
| `docs/sidebars.ts` | Modifié | Ajout catégorie Guides |
| `docs/i18n/fr/.../current.json` | Modifié | Ajout label "Guides" |

## Deviations from Plan

### Summary

| Type | Nombre | Impact |
|------|--------|--------|
| Auto-fixed | 3 | Essentiels, sans impact scope |
| Déférés | 0 | |

### Auto-fixed Issues

**1. Liens internes guides/index.md: guides/X vs ./X**
- Trouvé lors de: Task 3 (build, mais liens OK — déjà bons)
- Les liens `guides/battle-engine` dans l'index FONCTIONNENT (confirmé par build)
- Brève tentative erronée de changer en `./battle-engine` — revertée immédiatement

**2. Lien tutorial/index cassé depuis guides/index**
- Trouvé lors de: Task 3 build erreur
- Problème: `tutorial/index` depuis `/docs/guides` résout en `/docs/tutorial/index` mais la page est servie à `/docs/tutorial`
- Fix: suppression du lien hypertexte, mention en texte plat

**3. Lien advanced/narration-plugin cassé depuis tts-narration**
- Trouvé lors de: Task 3 build erreur
- Problème: `advanced/narration-plugin` résout en `/docs/guides/advanced/narration-plugin` (relatif au mauvais niveau)
- Fix: `../advanced/narration-plugin` (remonter un niveau)

## Issues Encountered

| Problème | Résolution |
|----------|------------|
| Build error "Broken link" tutorial/index | Supprimer le lien, mentionner en texte plat |
| Build error "Broken link" advanced/narration-plugin | Utiliser `../advanced/narration-plugin` |

## Next Phase Readiness

**Ready:**
- Guides section établie: 11-04 (API reference) peut suivre le même pattern
- Cross-links vers advanced/ validés (pattern `../advanced/X`)
- Parité EN/FR maintenue

**Concerns:**
- GitHub Pages non encore activé (étape manuelle repo Settings)

**Blockers:**
- Aucun

---
*Phase: 11-documentation, Plan: 03*
*Complété: 2026-06-13*
