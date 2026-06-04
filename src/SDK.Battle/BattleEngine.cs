namespace SDK.Battle;

using SDK.Battle.Difficulty;
using SDK.Battle.Formulas;
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

    public BattleEngine(
        IDamageFormula formula,
        IDifficultyMode playerStrategy,
        IDifficultyMode opponentStrategy,
        ITypeChart typeChart)
    {
        _formula = formula;
        _playerStrategy = playerStrategy;
        _opponentStrategy = opponentStrategy;
        _typeChart = typeChart;
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

        while (state.Turn < MaxTurns)
        {
            if (state.Player.CurrentHp <= 0)
                return new BattleResult(false, state.Turn);
            if (state.Opponent.CurrentHp <= 0)
                return new BattleResult(true, state.Turn);

            var playerMove = _playerStrategy.SelectMove(state.Player, state.Opponent, state.Config);
            var opponentMove = _opponentStrategy.SelectMove(state.Opponent, state.Player, state.Config);

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

            state = state with { Turn = state.Turn + 1 };
        }

        return new BattleResult(false, MaxTurns, "MaxTurns");
    }

    private BattleState ApplyMove(BattleState state, bool isPlayer, BattleMove move)
    {
        var attacker = isPlayer ? state.Player : state.Opponent;
        var defender = isPlayer ? state.Opponent : state.Player;

        if (Random.Shared.Next(0, 100) >= move.Accuracy)
            return state;

        var factor1 = _typeChart.GetFactor(move.TypeId, defender.Type1Id, _formula.Generation);
        var factor2 = defender.Type2Id.HasValue
            ? _typeChart.GetFactor(move.TypeId, defender.Type2Id.Value, _formula.Generation)
            : 1.0m;
        var stab = (move.TypeId == attacker.Type1Id ||
                    (attacker.Type2Id.HasValue && move.TypeId == attacker.Type2Id.Value))
            ? 1.5m : 1.0m;
        var typeMultiplier = factor1 * factor2 * stab;

        if (typeMultiplier == 0m)
            return state;

        var result = _formula.Calculate(attacker, defender, move, typeMultiplier, state.Config);
        var newDefender = defender with { CurrentHp = Math.Max(0, defender.CurrentHp - result.Damage) };

        return isPlayer
            ? state with { Opponent = newDefender }
            : state with { Player = newDefender };
    }
}
