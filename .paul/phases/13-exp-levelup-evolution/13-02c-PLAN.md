---
phase: 13-exp-levelup-evolution
plan: 02c
type: execute
wave: 3
depends_on: ["13-02b"]
files_modified:
  - src/SDK.MonoGame/Scenes/BattleScene.cs
autonomous: false
---

<objective>
## Goal
HpBar smooth lerp animation + FIGHT/RUN top-level action menu — deux gaps Phase 12 restants (P12-G1 et P12-G5).

## Purpose
Compléter l'UX de base du BattleScene pour que les barres HP ne sautent plus instantanément et que le joueur choisisse FIGHT ou RUN avant d'accéder au menu de moves.

## Output
BattleScene.cs modifié :
- `SelectAction` phase (FIGHT/RUN) avant `SelectMove`
- `_playerDisplayHp / _opponentDisplayHp` floats avec lerp dans `Update()`
- HpBar.Draw() reçoit les floats castés (animation visuelle)
- RUN → "Got away safely!" → BattleEnd
</objective>

<context>
## Project Context
@.paul/PROJECT.md

## Prior Work
@.paul/phases/13-exp-levelup-evolution/13-02b-SUMMARY.md

## Source Files
@src/SDK.MonoGame/Scenes/BattleScene.cs
@src/SDK.MonoGame/Input/InputMap.cs
</context>

<acceptance_criteria>

## AC-1: HP bars animent par lerp
```gherkin
Given les HP changent après RunTurn
When BattleScene Draw() est appelé frame par frame
Then la largeur de la HpBar interpole de l'ancienne valeur vers la nouvelle sur ~0.4s (facteur lerp 8f/sec)
```

## AC-2: Sélection FIGHT avant la liste de moves
```gherkin
Given c'est le tour du joueur
When BattleScene est en phase SelectAction
Then le panneau bas affiche "> FIGHT" / "  RUN" ; Up/Down navigue, Space confirme ; FIGHT passe à SelectMove
```

## AC-3: RUN quitte le combat
```gherkin
Given le joueur sélectionne RUN et confirme
When la phase SelectAction traite l'input
Then le log affiche "Got away safely!" et le combat transite vers BattleEnd
```

## AC-4: Retour à SelectAction (pas SelectMove) après chaque tour
```gherkin
Given un tour se termine et aucun Pokémon n'est KO
When ShowLog est confirmé et toutes les overlays sont fermées
Then la phase retourne à SelectAction (le joueur doit re-choisir FIGHT ou RUN)
```

</acceptance_criteria>

<tasks>

<task type="auto">
  <name>Task 1: HpBar smooth animation via float display tracking</name>
  <files>src/SDK.MonoGame/Scenes/BattleScene.cs</files>
  <action>
    Ajouter deux champs float dans BattleScene :
    ```
    private float _playerDisplayHp;
    private float _opponentDisplayHp;
    ```

    Dans LoadBattle() — initialiser AVANT le return :
    ```
    _playerDisplayHp  = initialState.Player.CurrentHp;
    _opponentDisplayHp = initialState.Opponent.CurrentHp;
    ```

    Dans Update() — ajouter lerp juste après le calcul ExpBar (avant le switch) :
    ```
    float lerpSpeed = 8f * (float)gameTime.ElapsedGameTime.TotalSeconds;
    _playerDisplayHp   = Math.Clamp(MathHelper.Lerp(_playerDisplayHp,   _state.Player.CurrentHp,   lerpSpeed), 0f, _state.Player.MaxHp);
    _opponentDisplayHp = Math.Clamp(MathHelper.Lerp(_opponentDisplayHp, _state.Opponent.CurrentHp, lerpSpeed), 0f, _state.Opponent.MaxHp);
    ```

    Dans Draw() — remplacer les deux appels HpBar.Draw pour passer les floats castés :
    - Opponent : `(int)Math.Ceiling(_opponentDisplayHp)` à la place de `_state.Opponent.CurrentHp`
    - Player   : `(int)Math.Ceiling(_playerDisplayHp)`   à la place de `_state.Player.CurrentHp`

    Utiliser Math.Ceiling pour éviter que la barre disparaisse (0px) alors que le Pokémon est encore vivant.
    Éviter : modifier HpBar.cs (rester stateless).
  </action>
  <verify>dotnet build PokemonSDK.slnx — 0 erreurs ; en game F1 la barre HP descend en glissant, pas en sautant</verify>
  <done>AC-1 satisfait : HpBar anime visuellement entre ancienne et nouvelle valeur</done>
</task>

<task type="auto">
  <name>Task 2: SelectAction phase — FIGHT/RUN top-level menu</name>
  <files>src/SDK.MonoGame/Scenes/BattleScene.cs</files>
  <action>
    **Étape A — BattlePhase enum** : ajouter `SelectAction` entre `Init` et `SelectMove` :
    ```csharp
    private enum BattlePhase { Init, SelectAction, SelectMove, ShowLog, ShowLevelUp, ShowMoveLearn, BattleEnd }
    ```

    **Étape B — Champs** : ajouter à la liste des champs existants :
    ```csharp
    private int  _actionIndex  = 0;
    private bool _playerRanAway = false;
    ```

    **Étape C — LoadBattle()** : changer `_phase = BattlePhase.SelectMove` → `_phase = BattlePhase.SelectAction`.
    Ajouter reset : `_actionIndex = 0; _playerRanAway = false;`

    **Étape D — NextPhaseAfterBattle()** : modifier pour retourner `SelectAction` à la place de `SelectMove` :
    ```csharp
    private BattlePhase NextPhaseAfterBattle()
    {
        if (_playerRanAway) return BattlePhase.BattleEnd;
        if (_state!.Player.CurrentHp <= 0 || _state.Opponent.CurrentHp <= 0)
            return BattlePhase.BattleEnd;
        return BattlePhase.SelectAction;
    }
    ```

    **Étape E — Update() case SelectAction** : insérer AVANT le case SelectMove existant :
    ```csharp
    case BattlePhase.SelectAction:
    {
        var ksAct = Keyboard.GetState();
        if (ksAct.IsKeyDown(InputMap.NavUp)   && !_prevKs.IsKeyDown(InputMap.NavUp))
            _actionIndex = (_actionIndex - 1 + 2) % 2;
        if (ksAct.IsKeyDown(InputMap.NavDown) && !_prevKs.IsKeyDown(InputMap.NavDown))
            _actionIndex = (_actionIndex + 1) % 2;
        if (ksAct.IsKeyDown(InputMap.Confirm) && !_prevKs.IsKeyDown(InputMap.Confirm))
        {
            if (_actionIndex == 0) // FIGHT
            {
                _moveMenu = new MoveMenu(_state!.Player.Moves, _graphicsDevice!, Keyboard.GetState());
                _phase = BattlePhase.SelectMove;
            }
            else // RUN
            {
                _lastLog = new List<string> { "Got away safely!" };
                _playerRanAway = true;
                _phase = BattlePhase.ShowLog;
                _prevKs = Keyboard.GetState();
            }
        }
        _prevKs = ksAct;
        break;
    }
    ```

    **Étape F — Draw() SelectAction** : ajouter dans le bloc UI (après DrawRect panneau bas) :
    ```csharp
    if (_phase == BattlePhase.SelectAction && _font != null)
    {
        string fightTxt = _actionIndex == 0 ? "> FIGHT" : "  FIGHT";
        string runTxt   = _actionIndex == 1 ? "> RUN"   : "  RUN";
        sb.DrawString(_font, fightTxt, new Vector2(8f,  187f), Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
        sb.DrawString(_font, runTxt,   new Vector2(8f,  207f), Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
    }
    ```

    Éviter : modifier MoveMenu, BattleEngine, HpBar, BattleEndOverlay.
    Note : BattleEndOverlay reçoit `_state.Player.CurrentHp > 0` — quand le joueur fuit HP > 0 donc affiche "Victory" (acceptable MVP ; corrigé Phase 14).
  </action>
  <verify>dotnet build clean ; F1 → combat → panneau bas affiche FIGHT/RUN ; RUN → "Got away safely!" → BattleEnd ; FIGHT → liste de moves</verify>
  <done>AC-2, AC-3, AC-4 satisfaits</done>
</task>

<task type="checkpoint:human-verify" gate="blocking">
  <what-built>
    - HpBar smooth lerp animation (float _playerDisplayHp / _opponentDisplayHp, lerp 8f/sec)
    - SelectAction phase avec menu FIGHT / RUN (Up/Down pour naviguer, Space pour confirmer)
    - RUN → "Got away safely!" → BattleEnd
    - FIGHT → SelectMove (comportement existant)
  </what-built>
  <how-to-verify>
    1. `dotnet run --project src/SDK.MonoGame`
    2. F1 pour lancer le combat de test
    3. Vérifier : le panneau bas affiche "> FIGHT" et "  RUN" (pas la liste de moves directement)
    4. Appuyer Down : "  FIGHT" et "> RUN" — curseur bouge ✓
    5. Remonter Up : "> FIGHT" revient ✓
    6. Confirmer FIGHT → liste de moves apparait ✓
    7. Choisir Tackle → attaque → log s'affiche → confirmer → retour à FIGHT/RUN (pas directement aux moves) ✓
    8. Après le tour : observer la barre HP adverse descendre en glissant (~0.3-0.5s) plutôt que sauter ✓
    9. Lancer un nouveau combat F1 → choisir RUN → "Got away safely!" dans le log → Space → BattleEnd ✓
  </how-to-verify>
  <resume-signal>Type "approved" pour continuer, ou décris les problèmes à corriger</resume-signal>
</task>

</tasks>

<boundaries>

## DO NOT CHANGE
- src/SDK.Battle/BattleEngine.cs (moteur intact)
- src/SDK.MonoGame/UI/MoveMenu.cs (PP + ghost-input fix de 13-02b)
- src/SDK.MonoGame/UI/HpBar.cs (garder stateless)
- src/SDK.MonoGame/UI/LevelUpOverlay.cs
- src/SDK.MonoGame/UI/MoveLearnOverlay.cs
- src/SDK.MonoGame/UI/BattleEndOverlay.cs

## SCOPE LIMITS
- Pas de formule de fuite (RUN réussit toujours — MVP)
- Pas de menu BAG / POKéMON (Phase 14-15)
- Pas de sequential multi-level-up overlays (requiert états intermédiaires BattleEngine — déféré)
- BattleEndOverlay inchangé (affiche "Victory" quand joueur fuit — acceptable pour l'instant)
- Pas de nouvelle classe UI (FIGHT/RUN inline dans BattleScene)

</boundaries>

<verification>
Avant de déclarer le plan complet :
- [ ] `dotnet build PokemonSDK.slnx` — 0 erreurs, 0 warnings
- [ ] `dotnet test tests/ --collect:"XPlat Code Coverage"` — 293+ tests passent (0 régressions)
- [ ] HP bars animent visuellement (vérification humaine F3)
- [ ] FIGHT/RUN menu apparait en début de tour (vérification humaine)
- [ ] RUN → "Got away safely!" → BattleEnd (vérification humaine)
- [ ] Retour à SelectAction après chaque tour (pas SelectMove) (vérification humaine)
- [ ] checkpoint:human-verify passé
</verification>

<success_criteria>
- Tasks 1 et 2 terminées
- Tous les tests passent
- HP bars animent par lerp (observable en game)
- FIGHT/RUN menu opérationnel
- RUN quitte le combat proprement
</success_criteria>

<output>
Après completion, créer `.paul/phases/13-exp-levelup-evolution/13-02c-SUMMARY.md`
</output>
