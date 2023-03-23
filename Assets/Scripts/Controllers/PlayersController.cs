using System.Collections.Generic;
using UnityEngine;

public class PlayersController : MonoBehaviour
{
    [SerializeField] private Notification _notificationManager;
    private static List<CheckersPlayer> _players;
    private static CheckersPlayer _currentPlayer;
    private static bool _isGameOver = false;

    public static bool IsGameOver {
        get {
            return _isGameOver;
        }
    }

    public static CheckerType CurrentCheckersTypeTurn {
        get {
            return _currentPlayer.CheckerType;
        }
    }

    public static List<CheckersPlayer> Players {
        get {
            return _players;
        }
    }

    #region Unity Methods

    private void Awake() {
        _players = new List<CheckersPlayer>();
    }
    private void Start() {
        MovementController._strokeTransition = ChangeCurrentPlayer;
    }
        
    #endregion

    #region Public Methods

    public static void ReduceCheckerFromPlayer(CheckerType checkerThatGotKilled) {
        Players.Find((CheckersPlayer player) => player.CheckerType == checkerThatGotKilled)?.ReduceChecker();
    }

    public static void AddCheckerToPlayer(CheckerType checkerType) {
        CheckersPlayer player = CheckPlayerExistance(checkerType);
        if (player != null) {
            player.CheckersAmount++;
        }
    }

    public static void RefreshPlayers() {
        _players = new List<CheckersPlayer>();
        _currentPlayer = null;
        _isGameOver = false;
    }

    #endregion

    #region Private Methods

    private void ChangeCurrentPlayer() {
        if (_players.Count == 1) {
            _notificationManager.ShowNotification(_players[0].CheckerType.ToString() + " player won!");
            _isGameOver = true;
        } else {
            _currentPlayer = _players[_players.IndexOf(_currentPlayer) == _players.Count - 1 ? 0 : _players.IndexOf(_currentPlayer) + 1];
            
            if (_currentPlayer.CheckersAmount <= 0) {
                EliminatePlayer(_currentPlayer);
                return;
            }
            
            _notificationManager.ShowNotification(_currentPlayer.CheckerType.ToString() + ", your turn");
        }
    }  

    private void EliminatePlayer(CheckersPlayer player) {
        _players.Remove(player);
        _notificationManager.ShowNotification(player.CheckerType.ToString()+ " is eliminated.");
        ChangeCurrentPlayer();
    }
    private static CheckersPlayer CheckPlayerExistance(CheckerType addingCheckerType) {
        CheckersPlayer p = Players.Find((CheckersPlayer player) => player.CheckerType == addingCheckerType);

        if (p == null) {
            p = new CheckersPlayer(addingCheckerType, 0);
            Players.Add(p);
            
            if (_currentPlayer == null) {
                _currentPlayer = p;
            }

            return p;
        }

        return p;
    }
        
    #endregion
}
