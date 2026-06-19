---
phase: 13-exp-levelup-evolution
plan: 02b
type: execute
wave: 1
depends_on: ["13-02"]
files_modified:
  - src/SDK.Battle/BattleEngine.cs
  - src/SDK.MonoGame/UI/MoveMenu.cs
  - src/SDK.MonoGame/Scenes/BattleScene.cs
  - tests/SDK.Battle.Tests/BattleEngineTests.cs
autonomous: false
---

<objective>
## Goal

Combler 4 gaps Phase 12 dans la BattleScene: PP deduction engine (P12-G3), PP display MoveMenu (P12-G2), nicknames dans HpBar (P12-G4), meteo visible (P12-G6).

## Purpose

Phase 12 a livre la structure UI sans 4 correctifs de donnees/affichage: PP jamais decrementes, labels "FOE"/"PLR" hardcodes, meteo ignoree. Ces gaps degadent la fidelite Pokemon sans bloquer la jouabilite — ils sont groupes ici car ils touchent 3 fichiers distincts sans dependances croisees.

## Output

- BattleEngine.ApplyMove decremente CurrentPP (clamp 0)
- BattleEngine.AwardExp restaure CurrentHp du delta MaxHp au level-up (gap d'authenticite Pokemon)
- SDK.Battle.Tests: 3 nouveaux tests (2 PP + 1 HP restoration)
- MoveMenu affiche "PP X/Y" par move
- MoveMenu reconstruit apres chaque ExecuteTurn() — PP toujours a jour
- HpBar labels = Nickname tronque 10 chars (ASCII)
- Weather != None affiche "SUN"/"RAIN"/"SAND"/"HAIL" en haut ecran
</objective>

<context>
## Project Context
@.paul/PROJECT.md

## Prior Work
@.paul/phases/13-exp-levelup-evolution/13-02-SUMMARY.md

## Source Files
@src/SDK.Battle/BattleEngine.cs
@src/SDK.MonoGame/UI/MoveMenu.cs
@src/SDK.MonoGame/Scenes/BattleScene.cs
@src/SDK.Core/ValueObjects/BattleMove.cs
@src/SDK.Core/ValueObjects/BattlePokemon.cs
@src/SDK.Core/ValueObjects/BattleState.cs
</context>

<acceptance_criteria>

## AC-1: PP decremente apres utilisation
```gherkin
Given un BattlePokemon avec CurrentPP = 35 sur tackle
When RunTurn est appele avec tackle comme move joueur
Then state.Player.Moves[0].CurrentPP == 34
```

## AC-2: PP clamp a 0 (pas de negatif)
```gherkin
Given un BattlePokemon avec CurrentPP = 0 sur tackle
When RunTurn est appele avec tackle
Then state.Player.Moves[0].CurrentPP == 0
```

## AC-7: HP restaure au level-up (delta MaxHp)
```gherkin
Given un BattlePokemon avec CurrentHp=45 MaxHp=60 pret a monter de niveau
When AwardExp provoque un level-up et MaxHp passe a 65 (+5)
Then state.Player.CurrentHp == 50 (45 + 5)
And state.Player.CurrentHp <= state.Player.MaxHp
```

## AC-3: PP affiche dans MoveMenu
```gherkin
Given BattleScene en phase SelectMove, move tackle CurrentPP=34 MaxPP=35
When Draw() est appele
Then "34/35" est visible dans le panneau move pour tackle
```

## AC-4: Nickname dans HpBar (pas "FOE"/"PLR")
```gherkin
Given BattleState avec Player.Nickname="BULBASAUR", Opponent.Nickname="PIDGEY"
When Draw() est appele
Then HpBar joueur affiche "BULBASAUR" et HpBar adversaire affiche "PIDGEY"
```

## AC-5: Meteo affichee si active
```gherkin
Given BattleState.Weather = WeatherType.Rain
When Draw() est appele
Then "RAIN" visible en haut de l'ecran de combat
```

## AC-6: Pas d'affichage meteo si None
```gherkin
Given BattleState.Weather = WeatherType.None
When Draw() est appele
Then aucun label meteo n'apparait
```

</acceptance_criteria>

<tasks>

<task type="auto">
  <name>Task 1: PP deduction dans BattleEngine.ApplyMove + tests</name>
  <files>src/SDK.Battle/BattleEngine.cs, tests/SDK.Battle.Tests/BattleEngineTests.cs</files>
  <action>
    Dans BattleEngine.ApplyMove, apres la premiere ligne (log "used X!"), decremente le PP du move utilise:

    1. Identifier l'index du move dans attacker.Moves par MoveId.
    2. Si trouve et CurrentPP > 0: rebuild la liste Moves avec `move with { CurrentPP = move.CurrentPP - 1 }`.
    3. Rebuild state via `state with { Player = state.Player with { Moves = updatedMoves } }` (ou Opponent si !isPlayer).
    4. Si CurrentPP == 0: ne pas decrementer (clamp).

    Pattern identique a AwardExp: toList(), modifier, with-expression.

    Attention: le parametre `move` passe a ApplyMove est une copie value. Matcher par MoveId.
    Ne pas ajouter Struggle mechanic (Phase 14+).
    Tester uniquement le cas joueur (opponent PP: meme pattern, teste indirectement).

    **HP restoration au level-up (dans AwardExp):**
    En debut de methode, apres `int maxHp = player.MaxHp;`, ajouter:
    ```csharp
    int currentHp = player.CurrentHp;
    ```
    Dans la boucle while, avant `maxHp = (int)(maxHp * scale)`, capturer l'ancien maxHp:
    ```csharp
    int oldMaxHp = maxHp;
    maxHp = (int)(maxHp * scale);
    currentHp = Math.Min(currentHp + (maxHp - oldMaxHp), maxHp);
    ```
    Dans `updatedPlayer`, ajouter `CurrentHp = currentHp,` avec MaxHp.

    Logique: delta = newMaxHp - oldMaxHp. Si MaxHp passe de 60 a 65 (+5), currentHp gagne 5.
    `Math.Min(..., maxHp)` garantit que currentHp ne depasse jamais maxHp (multi-level-up safe).

    Dans BattleEngineTests.cs ajouter 3 tests:
    - `RunTurn_DeductsOnePP_WhenMoveUsed`: Player PP 35 -> 34 apres un tour
    - `RunTurn_DoesNotDeductPP_WhenAlreadyZero`: Player PP 0 -> 0 apres un tour
    - `AwardExp_RestoresCurrentHpByMaxHpDelta_OnLevelUp`: Player 45/60 HP, level-up MaxHp 60->65, CurrentHp == 50
  </action>
  <verify>dotnet test tests/SDK.Battle.Tests/SDK.Battle.Tests.csproj — tous les tests verts, les 3 nouveaux tests passent</verify>
  <done>AC-1 + AC-2 + AC-7 satisfaits: PP decremente correctement, HP restaure au level-up</done>
</task>

<task type="auto">
  <name>Task 2: PP display dans MoveMenu</name>
  <files>src/SDK.MonoGame/UI/MoveMenu.cs</files>
  <action>
    Dans MoveMenu.Draw(), apres le DrawString du label move (nom avec curseur), ajouter le PP:

    Format: `$"{move.CurrentPP}/{move.MaxPP}"`
    Position: a droite du slot move, ex: `new Vector2(origin.X + 70f, origin.Y + i * 14f + 1f)`
    Scale: 0.55f identique au nom
    Color: blanc si CurrentPP > MaxPP/4, jaune si <= MaxPP/4, rouge si 0

    Slot width = 110px. PP text ancree a droite (position X = origin.X + 68 environ).
    Tout ASCII (chiffres, slash) — pas de probleme SpriteFont.
  </action>
  <verify>dotnet build PokemonSDK.slnx — 0 erreurs. Visual: F1 battle, MoveMenu montre "35/35" sur tackle.</verify>
  <done>AC-3 satisfait: PP visible dans le menu move</done>
</task>

<task type="auto">
  <name>Task 3: Nickname HpBar + affichage meteo</name>
  <files>src/SDK.MonoGame/Scenes/BattleScene.cs</files>
  <action>
    **P12-G4 Nickname HpBar:**
    Remplacer les deux appels HpBar.Draw dans Draw():
    - `"FOE"` → `_state.Opponent.Nickname[..Math.Min(_state.Opponent.Nickname.Length, 10)]`
    - `"PLR"` → `_state.Player.Nickname[..Math.Min(_state.Player.Nickname.Length, 10)]`

    Nicknames sont deja en ASCII majuscule dans tous les scenarios debug — pas de filtrage supplementaire requis.
    Tronquer a 10 chars max pour eviter overflow dans le panel 140px.

    **MoveMenu refresh apres ExecuteTurn() (gap critique PP display):**
    Dans ExecuteTurn(), apres `_phase = BattlePhase.ShowLog;`, ajouter:
    ```csharp
    _moveMenu = new MoveMenu(_state.Player.Moves, _graphicsDevice!);
    ```
    Raison: `_moveMenu._moves` reference la liste de `LoadBattle()`. Apres PP deduction,
    `_state.Player.Moves` est une nouvelle liste. Sans rebuild, le PP display affiche
    des valeurs perimees. `ApplyMoveLearnDecision()` fait deja ce rebuild — pattern identique.

    **P12-G6 Meteo:**
    Dans Draw(), avant le panneau UI (avant DrawRect y=178), si `_state.Weather != WeatherType.None` et `_font != null`:
    ```csharp
    if (_state?.Weather != WeatherType.None && _font != null)
    {
        string weatherLabel = _state!.Weather switch
        {
            WeatherType.Sun  => "SUN",
            WeatherType.Rain => "RAIN",
            WeatherType.Sand => "SAND",
            WeatherType.Hail => "HAIL",
            _                => ""
        };
        if (weatherLabel.Length > 0)
            sb.DrawString(_font, weatherLabel, new Vector2(220f, 2f), Color.Yellow,
                0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
    }
    ```
    Position centree approximativement (480/2 - quelques pixels) a y=2 (haut ecran).
  </action>
  <verify>dotnet build PokemonSDK.slnx — 0 erreurs. Visual: F1 montre "BULBASAUR"/"PIDGEY" dans HpBars.</verify>
  <done>AC-4 + AC-5 + AC-6 satisfaits: nicknames corrects, meteo conditionnelle</done>
</task>

<task type="checkpoint:human-verify" gate="blocking">
  <what-built>PP deduction engine + HP restoration level-up + PP display MoveMenu (toujours a jour) + Nicknames HpBar + Weather display</what-built>
  <how-to-verify>
    1. `dotnet run --project src/SDK.MonoGame --configuration Debug`
    2. F1 (EXP bar sans level-up): confirmer "BULBASAUR"/"PIDGEY" dans HP bars (plus "PLR"/"FOE"). MoveMenu affiche "35/35". Jouer un tour: MoveMenu montre "34/35" apres le tour.
    3. F2 (level-up): apres le niveau, verifier que HpBar joueur montre HP augmente (ex: si 45/60 avant, ~50/65 apres) et pas inchange.
    4. F3 (MoveLearnOverlay): MoveMenu affiche "35/35" (ou valeur actuelle) par move.
    5. Pas de crash, pas de caractere manquant (?) dans SpriteBatch.
  </how-to-verify>
  <resume-signal>Type "approved" pour continuer, ou decris les problemes a corriger</resume-signal>
</task>

</tasks>

<boundaries>

## DO NOT CHANGE
- `src/SDK.Core/ValueObjects/BattleMove.cs` (record deja correct — CurrentPP/MaxPP existent)
- `src/SDK.Core/ValueObjects/BattlePokemon.cs` (pas de nouveaux champs)
- `src/SDK.MonoGame/UI/HpBar.cs` (smooth animation = 13-02c, pas ce plan)
- `src/SDK.MonoGame/UI/LevelUpOverlay.cs`, `MoveLearnOverlay.cs`, `ExpBar.cs`

## SCOPE LIMITS
- Pas de mecanique Struggle (PP=0 → auto-struggle) — Phase 14+
- Pas de FIGHT/RUN menu (P12-G5) — defere 13-02c
- Pas de HpBar animation smooth (P12-G1) — defere 13-02c
- Pas de MoveMenu refactor majeur — juste ajouter PP text dans Draw()
- Meteo affectant les dommages = Phase 19 (Mecaniques modernes)
- Multi level-up overlays separes (1 par niveau) — defere 13-02c (complexite state machine)
- Dispose pattern BattleScene (HpBar/MoveMenu/StatusIcon) — defere, mineur
- PP dans MoveLearnOverlay (liste moves a oublier) — defere, cosmétique

</boundaries>

<verification>
Avant de declarer plan complet:
- [ ] `dotnet build PokemonSDK.slnx` — 0 erreurs
- [ ] `dotnet test PokemonSDK.slnx` — tous tests verts (inclus 3 nouveaux tests: 2 PP + 1 HP restoration)
- [ ] F1 visuel: "BULBASAUR"/"PIDGEY" dans HP bars (plus "PLR"/"FOE")
- [ ] F1 visuel: MoveMenu affiche "34/35" apres un tour joue (PP decremente ET visible)
- [ ] F2 visuel: HP joueur augmente apres level-up (pas inchange)
- [ ] F3 visuel: MoveMenu affiche "PP X/Y" par move
- [ ] Aucun caractere non-ASCII dans DrawString
- [ ] checkpoint:human-verify approuve
</verification>

<success_criteria>
- Tous tasks executes
- 2 nouveaux tests PP verts
- Verification checks passes
- Human-verify approuve
- Aucune regression sur scenarios F1-F5
</success_criteria>

<output>
Apres completion: `.paul/phases/13-exp-levelup-evolution/13-02b-SUMMARY.md`
</output>
