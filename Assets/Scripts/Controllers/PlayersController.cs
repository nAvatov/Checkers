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

    private void Awake() {
        _players = new List<CheckersPlayer>();
    }
    // Start is called before the first frame update
    void Start() {
        MovementController._strokeTransition = ChangeCurrentPlayer;
    }

    #region Private Methods

    private void ChangeCurrentPlayer() {
        if (_players.Count == 1) {
            _notificationManager.ShowNotification(_players[0].CheckerType.ToString() + " player won!");
            _isGameOver = true;
        } else {
            _currentPlayer = _players[_players.IndexOf(_currentPlayer) == _players.Count - 1 ? 0 : _players.IndexOf(_currentPlayer) + 1];
            Debug.Log("Current player + " + _currentPlayer.CheckerType + " with "  + _currentPlayer.CheckersAmount + " checkers");
            
            if (_currentPlayer.CheckersAmount <= 0) {
                EliminatePlayer(_currentPlayer);
                return;
            }
            
            _notificationManager.ShowNotification(_currentPlayer.CheckerType.ToString() + ", your turn");
        }
    }  
        
    #endregion

    private void EliminatePlayer(CheckersPlayer player) {
        _players.Remove(player);
        _notificationManager.ShowNotification(player.CheckerType.ToString()+ " is eliminated.");
        ChangeCurrentPlayer();
    }

    public static void ReduceCheckerFromPlayer(CheckerType checkerThatGotKilled) {
        Debug.Log("Reduce call");
        Players.Find((CheckersPlayer player) => player.CheckerType == checkerThatGotKilled)?.ReduceChecker();
    }

    public static void AddCheckerToPlayer(CheckerType checkerType) {
        CheckersPlayer player = CheckPlayerExistance(checkerType);
        if (player != null) {
            player.CheckersAmount++;
        }
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
}
