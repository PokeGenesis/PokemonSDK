---
phase: 11-documentation
plan: 02
subsystem: ui
tags: [docusaurus, tutorial, i18n, fr, en, docs]

requires:
  - phase: 11-01
    provides: Docusaurus 3 scaffold EN+FR, GitHub Pages CI, base URL fix, em dash rule

provides:
  - "30-minute getting-started tutorial (4 EN + 4 FR pages)"
  - "Tutorial sidebar category + FR i18n label"
  - "Bilingual docs covering: project scaffold, BattleEngine 1v1, LuaScriptEngine + badge"

affects: "11-03 (Guides), 11-04 (API reference) — sidebar structure established"

tech-stack:
  added: []
  patterns:
    - "Docusaurus numberPrefixParser: numeric prefix stripped from doc IDs (01-create.md → ID: create)"
    - "Index page links use doc-ID style (tutorial/create), not relative (./create), due to URL resolution"
    - "Tutorial step links use slug-only (./battle not ./02-battle)"

key-files:
  created:
    - docs/docs/tutorial/index.md
    - docs/docs/tutorial/01-create.md
    - docs/docs/tutorial/02-battle.md
    - docs/docs/tutorial/03-lua-badge.md
    - docs/i18n/fr/docusaurus-plugin-content-docs/current/tutorial/index.md
    - docs/i18n/fr/docusaurus-plugin-content-docs/current/tutorial/01-create.md
    - docs/i18n/fr/docusaurus-plugin-content-docs/current/tutorial/02-battle.md
    - docs/i18n/fr/docusaurus-plugin-content-docs/current/tutorial/03-lua-badge.md
  modified:
    - docs/sidebars.ts
    - docs/i18n/fr/docusaurus-plugin-content-docs/current.json

key-decisions:
  - "Doc IDs strip numeric prefix: sidebar items use tutorial/create not tutorial/01-create"
  - "Index page links use absolute doc IDs to bypass URL resolution quirk"
  - "Next: links use colon (not em dash) per CLAUDE.md §11"

patterns-established:
  - "Tutorial step files: numeric prefix for ordering (01-, 02-, 03-), links omit prefix"
  - "Index pages at directory boundary: link via doc ID, not relative path"

duration: ~30min
started: 2026-06-13T16:36:00Z
completed: 2026-06-13T16:42:00Z
---

# Phase 11 Plan 02: Tutorial 30min EN+FR Summary

**Bilingual 30-minute getting-started tutorial (8 pages) covering project scaffold, BattleEngine 1v1, and LuaScriptEngine badge — build exit 0 EN+FR.**

## Performance

| Metric | Valeur |
|--------|--------|
| Durée | ~30 min |
| Démarré | 2026-06-13 ~16h36 |
| Complété | 2026-06-13 16h42 |
| Tâches | 3 complétées |
| Fichiers modifiés | 10 (8 créés + 2 modifiés) |

## Acceptance Criteria Results

| Critère | Statut | Notes |
|---------|--------|-------|
| AC-1: 4 pages EN tutorial | Pass | index, 01-create, 02-battle, 03-lua-badge |
| AC-2: 4 pages FR i18n | Pass | miroir complet avec code commenté en FR |
| AC-3: Sidebar Tutorial category | Pass | collapsed: false, positionné avant Packages |
| AC-4: FR sidebar label "Tutoriel" | Pass | current.json entry ajouté |
| AC-5: Build exit 0 EN+FR | Pass | `BASE_URL=/ npm run build` passe proprement |
| AC-6: 0 em dash | Pass | Next:/Suivant: liens utilisent `:` |

## Accomplishments

- 8 pages de tutoriel bilingues créées — parité complète EN/FR incluant commentaires de code et sorties console
- Tutorial category ajoutée à `sidebars.ts` (collapsed: false, avant Packages)
- Sidebar label FR "Tutoriel" enregistré dans `current.json`
- Docusaurus build production validé : 0 erreur, 0 lien cassé, EN + FR

## Task Commits

| Tâche | Commit | Type | Description |
|-------|--------|------|-------------|
| Task 1+2+3 | `2f3d1cf` | docs | 11-02 Wave 2 — tutorial 30min EN+FR (4 pages, sidebar, i18n) |

## Files Created/Modified

| Fichier | Changement | Objet |
|---------|------------|-------|
| `docs/docs/tutorial/index.md` | Créé | Vue d'ensemble + table des étapes |
| `docs/docs/tutorial/01-create.md` | Créé | Étape 1 : scaffold, seed, doctor, headless |
| `docs/docs/tutorial/02-battle.md` | Créé | Étape 2 : BattleEngine 1v1 complet |
| `docs/docs/tutorial/03-lua-badge.md` | Créé | Étape 3 : LuaScriptEngine + badge + save |
| `docs/i18n/fr/.../tutorial/index.md` | Créé | Miroir FR de l'index |
| `docs/i18n/fr/.../tutorial/01-create.md` | Créé | Miroir FR étape 1 |
| `docs/i18n/fr/.../tutorial/02-battle.md` | Créé | Miroir FR étape 2 |
| `docs/i18n/fr/.../tutorial/03-lua-badge.md` | Créé | Miroir FR étape 3 |
| `docs/sidebars.ts` | Modifié | Ajout catégorie Tutorial |
| `docs/i18n/fr/.../current.json` | Modifié | Ajout label "Tutoriel" |

## Deviations from Plan

### Summary

| Type | Nombre | Impact |
|------|--------|--------|
| Auto-fixed | 3 | Essentiels, sans impact scope |
| Déférés | 0 | |

**Impact total:** Corrections nécessaires liées au comportement Docusaurus, zéro scope creep.

### Auto-fixed Issues

**1. Em dashes dans les liens "Next:"**
- Trouvé lors de: Task 1 (création 01-create.md, 02-battle.md)
- Problème: Liens générés avec `—` (`Next: [Step 2 — First Battle]`)
- Fix: Remplacé par `:` (`Next: [Step 2: First Battle]`) — CLAUDE.md §11
- Fichiers: 01-create.md, 02-battle.md, miroirs FR
- Vérification: grep —count "—" = 0

**2. IDs sidebar avec préfixe numérique**
- Trouvé lors de: Task 3 (build error)
- Problème: `sidebars.ts` référençait `tutorial/01-create` mais Docusaurus stripait le préfixe → ID réel = `tutorial/create`
- Fix: Mis à jour les 3 IDs dans sidebars.ts (create, battle, lua-badge)
- Fichiers: docs/sidebars.ts
- Vérification: Build exit 0

**3. Liens relatifs depuis index.md**
- Trouvé lors de: Task 3 (build error — broken links)
- Problème: `./create` depuis `/docs/tutorial` résout vers `/docs/create` (index servi sans slash final)
- Fix: `tutorial/create` (style doc ID absolu, sans `./`) dans EN + FR index.md
- Fichiers: index.md EN + FR
- Vérification: Build exit 0, 0 lien cassé

## Issues Encountered

| Problème | Résolution |
|----------|------------|
| Build error "Unknown doc" sur IDs avec préfixe | Corriger sidebar vers IDs sans préfixe (Docusaurus numberPrefixParser) |
| Build error "Broken link" depuis index.md | Passer de `./create` à `tutorial/create` (doc ID absolu) |

## Next Phase Readiness

**Ready:**
- Structure tutorial établie — les guides (11-03) peuvent suivre le même pattern
- Sidebar extensible — nouvelles catégories s'ajoutent après Tutorial
- Parité EN/FR maintenue — pattern i18n clair

**Concerns:**
- GitHub Pages non encore activé (étape manuelle repo Settings → Pages → Source: GitHub Actions)

**Blockers:**
- Aucun

---
*Phase: 11-documentation, Plan: 02*
*Complété: 2026-06-13*
