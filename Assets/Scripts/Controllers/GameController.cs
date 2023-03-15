using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
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
            return _currentPlayer.CheckersTypeColor;
        }
    }

    public static List<CheckersPlayer> Players {
        get {
            return _players;
        }
    }

    private void Awake() {
        _players = new List<CheckersPlayer> {
            new CheckersPlayer(CheckerType.bot, 12),
            new CheckersPlayer(CheckerType.left, 12),
            new CheckersPlayer(CheckerType.top, 12),
            new CheckersPlayer(CheckerType.right, 12)
        };
    }
    // Start is called before the first frame update
    void Start()
    {
        MovementController._legalMoveDone = ChangeCurrentPlayer;
        _currentPlayer = _players[new System.Random().Next(0, _players.Count - 1)];
        _notificationManager.ShowNotification(_currentPlayer.CheckersTypeColor.ToString() + ", you first");
    }

    #region Private Methods

    private void ChangeCurrentPlayer() {
        if (_players.Count == 1) {
            _notificationManager.ShowNotification(_players[0].CheckersTypeColor.ToString() + " player won!");
            _isGameOver = true;
        } else {
            _currentPlayer = _players[_players.IndexOf(_currentPlayer) == _players.Count - 1 ? 0 : _players.IndexOf(_currentPlayer) + 1];
            
            if (_currentPlayer.CheckersAmount <= 0) {
                EliminatePlayer(_currentPlayer);
                return;
            }
            
            _notificationManager.ShowNotification(_currentPlayer.CheckersTypeColor.ToString() + ", your turn");
        }
    }  
        
    #endregion

    private void EliminatePlayer(CheckersPlayer player) {
        _players.Remove(player);
        _notificationManager.ShowNotification(player.CheckersTypeColor.ToString()+ " is eliminated.");
        ChangeCurrentPlayer();
    }

    public static void ReduceCheckerFromPlayer(CheckerType checkerThatKilled) {
        foreach(CheckersPlayer player in Players) {
            if (player.CheckersTypeColor == checkerThatKilled) {
                player.ReduceChecker();
                return;
            }
        }
    }
}
