namespace SDK.Battle;

using SDK.Battle.Difficulty;
using SDK.Battle.Formulas;
using SDK.Battle.Plugins;
using SDK.Core.Enums;
using SDK.Core.Interfaces;
using SDK.Core.ValueObjects;

public sealed class BattleEngine : IBattleEngine
{
    private const int MaxTurns = 200;

    private readonly IDamageFormula _formula;
    private readonly IDifficultyMode _playerStrategy;
    private readonly IDifficultyMode _opponentStrategy;
    private readonly ITypeChart _typeChart;
    private readonly PluginRegistry _plugins;
    private readonly IExpFormula? _expFormula;

    public BattleEngine(
        IDamageFormula formula,
        IDifficultyMode playerStrategy,
        IDifficultyMode opponentStrategy,
        ITypeChart typeChart,
        PluginRegistry? plugins = null,
        IExpFormula? expFormula = null)
    {
        _formula = formula;
        _playerStrategy = playerStrategy;
        _opponentStrategy = opponentStrategy;
        _typeChart = typeChart;
        _plugins = plugins ?? new PluginRegistry();
        _expFormula = expFormula;
    }

    public BattleResult RunBattle(BattleRequest request)
    {
        var state = new BattleState(
            request.Player,
            request.Opponent,
            0,
            WeatherType.None,
            request.Config,
            Array.Empty<string>());

        _plugins.NotifyBattleStart(state);

        while (state.Turn < MaxTurns)
        {
            if (state.Player.CurrentHp <= 0)
            {
                _plugins.NotifyFainted(state, state.Player);
                var result = new BattleResult(false, state.Turn);
                _plugins.NotifyBattleEnd(state, result);
                return result;
            }
            if (state.Opponent.CurrentHp <= 0)
            {
                _plugins.NotifyFainted(state, state.Opponent);
                var result = new BattleResult(true, state.Turn);
                _plugins.NotifyBattleEnd(state, result);
                return result;
            }

            var playerMove = _playerStrategy.SelectMove(state.Player, state.Opponent, state.Config);
            var opponentMove = SelectOpponentMove(state);

            state = RunTurn(state, playerMove, opponentMove);
        }

        var maxResult = new BattleResult(false, MaxTurns, "MaxTurns");
        _plugins.NotifyBattleEnd(state, maxResult);
        return maxResult;
    }

    public BattleState RunTurn(BattleState state, BattleMove playerMove, BattleMove opponentMove)
    {
        state = state with { Log = Array.Empty<string>() };
        _plugins.NotifyTurnStart(state);

        state = _plugins.ApplyBeforeMove(state, new BattleAction(playerMove.MoveId, true));
        state = _plugins.ApplyBeforeMove(state, new BattleAction(opponentMove.MoveId, false));

        bool playerFirst = state.Player.Speed > state.Opponent.Speed
            || (state.Player.Speed == state.Opponent.Speed && Random.Shared.Next(2) == 0);

        if (playerFirst)
        {
            state = ApplyMove(state, isPlayer: true, playerMove);
            if (state.Opponent.CurrentHp > 0)
                state = ApplyMove(state, isPlayer: false, opponentMove);
        }
        else
        {
            state = ApplyMove(state, isPlayer: false, opponentMove);
            if (state.Player.CurrentHp > 0)
                state = ApplyMove(state, isPlayer: true, playerMove);
        }

        if (_expFormula != null && state.Opponent.CurrentHp <= 0)
            state = AwardExp(state, _playerStrategy.VictoryExpMultiplier);
        else if (_expFormula != null && state.Player.CurrentHp <= 0 && _playerStrategy.DefeatExpMultiplier > 0f)
            state = AwardExp(state, _playerStrategy.DefeatExpMultiplier);

        state = state with { Turn = state.Turn + 1 };
        _plugins.NotifyTurnEnd(state);
        return state;
    }

    public BattleMove SelectOpponentMove(BattleState state) =>
        _opponentStrategy.SelectMove(state.Opponent, state.Player, state.Config);

    private BattleState AwardExp(BattleState state, float multiplier = 1.0f)
    {
        var formula = _expFormula!;
        var player = state.Player;
        var opponent = state.Opponent;

        var levelCap = state.Config.GetLevelCap();
        if (levelCap.HasValue && player.Level >= levelCap.Value)
        {
            var capLog = state.Log.ToList();
            capLog.Add($"EXP bloquée ! (Cap niveau {levelCap.Value} — prochain badge requis)");
            return state with { Log = capLog };
        }

        int gained = (int)(formula.CalcExpGain(opponent.BaseExpYield, opponent.Level, false) * multiplier);
        int newExp = player.CurrentExp + gained;

        var log = state.Log.ToList();
        log.Add($"{player.Nickname} gained {gained} EXP!");

        int newLevel = player.Level;
        int atk = player.Attack, def = player.Defense;
        int spa = player.SpecialAttack, spd = player.SpecialDefense, spe = player.Speed;
        int maxHp = player.MaxHp;

        while (newLevel < 100 && newExp >= formula.ExpThreshold(newLevel + 1, player.GrowthRate))
        {
            int oldLevel = newLevel;
            newLevel++;

            double scale = (newLevel + 5.0) / (oldLevel + 5.0);
            atk   = (int)(atk   * scale);
            def   = (int)(def   * scale);
            spa   = (int)(spa   * scale);
            spd   = (int)(spd   * scale);
            spe   = (int)(spe   * scale);
            maxHp = (int)(maxHp * scale);

            log.Add($"{player.Nickname} grew to level {newLevel}!");

            if (player.FullLearnset != null)
                foreach (var (learnLevel, move) in player.FullLearnset)
                    if (learnLevel == newLevel)
                        log.Add($"{player.Nickname} learned {move.Identifier}!");

            var updatedPlayer = player with
            {
                Level = newLevel, CurrentExp = newExp,
                Attack = atk, Defense = def,
                SpecialAttack = spa, SpecialDefense = spd, Speed = spe,
                MaxHp = maxHp,
            };
            _plugins.NotifyLevelUp(updatedPlayer, oldLevel, newLevel);
            player = updatedPlayer;
        }

        player = player with { CurrentExp = newExp };
        return state with { Player = player, Log = log };
    }

    private BattleState ApplyMove(BattleState state, bool isPlayer, BattleMove move)
    {
        var attacker = isPlayer ? state.Player : state.Opponent;
        var defender = isPlayer ? state.Opponent : state.Player;

        state = AddLog(state, $"{attacker.Nickname} used {move.Identifier.ToUpperInvariant()}!");

        if (Random.Shared.Next(0, 100) >= move.Accuracy)
            return AddLog(state, "The attack missed!");

        if (move.Category == MoveCategory.Status || move.Power is null)
            return AddLog(state, $"(stat effects: Phase 13)");

        var factor1 = _typeChart.GetFactor(move.TypeId, defender.Type1Id, _formula.Generation);
        var factor2 = defender.Type2Id.HasValue
            ? _typeChart.GetFactor(move.TypeId, defender.Type2Id.Value, _formula.Generation)
            : 1.0m;
        var stab = (move.TypeId == attacker.Type1Id ||
                    (attacker.Type2Id.HasValue && move.TypeId == attacker.Type2Id.Value))
            ? 1.5m : 1.0m;
        var typeMultiplier = factor1 * factor2 * stab;

        if (typeMultiplier == 0m)
            return AddLog(state, "It had no effect...");

        var damageResult = _formula.Calculate(attacker, defender, move, typeMultiplier, state.Config);
        state = _plugins.ApplyBeforeDamage(state, damageResult);

        if (typeMultiplier > 1.0m)
            state = AddLog(state, "It is super effective!");
        else if (typeMultiplier < 1.0m)
            state = AddLog(state, "It is not very effective...");

        var currentDefender = isPlayer ? state.Opponent : state.Player;
        var newDefender = currentDefender with
        {
            CurrentHp = Math.Max(0, currentDefender.CurrentHp - damageResult.Damage)
        };

        state = AddLog(state, $"{defender.Nickname} took {damageResult.Damage} damage!");
        if (newDefender.CurrentHp <= 0)
            state = AddLog(state, $"{defender.Nickname} fainted!");

        return isPlayer
            ? state with { Opponent = newDefender }
            : state with { Player = newDefender };
    }

    private static BattleState AddLog(BattleState state, string msg) =>
        state with { Log = [.. state.Log, msg] };
}
