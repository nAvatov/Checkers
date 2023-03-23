using System;
using UnityEngine;

public enum CheckerType {
    noType,
    bot,
    left,
    top,
    right
}

public class Board : MonoBehaviour
{   
    [SerializeField] private Notification _notificationManager;
    private const int _fourPlayersMatrixSize = 11;
    private const int _twoPlayersMatrixSize = 8;
    private static int _currentMatrixSize = 0;

    private static Cell[,] _matrixOfCells;
    private RectTransform _rTransform;

    public static Cell[,] MatrixOfCells {
        get {
            return _matrixOfCells;
        }
    }

    public static int CurrentMatrixSize {
        get {
            return _currentMatrixSize;
        }

        set {
            _currentMatrixSize = value;
        }
    }

    #region Unity Methods

    private void Awake() {
        _rTransform = GetComponent<RectTransform>();
    }

    private void Start() {
        CurrentMatrixSize = _twoPlayersMatrixSize;
        StartGame();
    }

    #endregion

    #region Public Methods

    public void ChangeGameMode() {
        CurrentMatrixSize = CurrentMatrixSize == _fourPlayersMatrixSize ? _twoPlayersMatrixSize : _fourPlayersMatrixSize;
        StartGame();
    }

    public void RestartGame() {
        StartGame();
        if (ButtonsController.IsHidden) {
            ButtonsController.HandleButtonsPanelWidth();
        }
    }

    #endregion

    #region Private Methods

    private void StartGame() {
        HandleCellsAmount();
        
        _matrixOfCells = new Cell[_currentMatrixSize, _currentMatrixSize];
        
        PlayersController.RefreshPlayers();
        InitializeMatrixWithCells();
        PaintCells();
        RefreshBoardCells();

        if (CurrentMatrixSize == _fourPlayersMatrixSize) {
            Spawn4Checkers();
        } else {
            Spawn2Checkers();
        }
        
        _notificationManager.ShowNotification(PlayersController.CurrentCheckersTypeTurn.ToString() + ", you first");
    }
    /// <summary>
    /// Fill matrix with cells for realtime access to any of them.
    /// </summary>
    private void InitializeMatrixWithCells() {
        int k = 0;

        for (int i = 0; i < CurrentMatrixSize; i++) {
            for (int j = 0; j < CurrentMatrixSize; j++) {
                if (_rTransform.GetChild(k).gameObject.activeSelf) {
                    _matrixOfCells[i, j] = _rTransform.GetChild(k).GetComponent<Cell>();
                    k++;
                }
            }
        }
    }

    private void Spawn2Checkers()
    {
        for(int i = 0; i < 3; i++ ) {
            for (int j = i % 2 == 0 ? 1 : 0; j < _twoPlayersMatrixSize; j += 2) {
                _matrixOfCells[i, j].AddCheckerToCell(CheckerType.top);
                _matrixOfCells[_twoPlayersMatrixSize - 1 - i, _twoPlayersMatrixSize - 1 - j].AddCheckerToCell(CheckerType.bot);
            }
        }
    }

    private void Spawn4Checkers() {
        for(int i = 0; i < 3; i++ ) {
            for (int j = i + 1; j < _fourPlayersMatrixSize - i; j += 2) {
                _matrixOfCells[i, j].AddCheckerToCell(CheckerType.top);
                _matrixOfCells[j, i].AddCheckerToCell(CheckerType.left);
                _matrixOfCells[_fourPlayersMatrixSize - 1 - i, j].AddCheckerToCell(CheckerType.bot);
                _matrixOfCells[j, _fourPlayersMatrixSize - 1 - i].AddCheckerToCell(CheckerType.right);
            }
        }
    }

    /// <summary>
    /// Removes amy checkers from all cells on board.
    /// </summary>
    private void RefreshBoardCells() {
        for (int i = 0; i < CurrentMatrixSize; i++) {
            for (int j = 0; j < CurrentMatrixSize; j++) { 
                _matrixOfCells[i, j].HandleCheckerOnMe(CheckerType.noType, false);
            }
        }
    }

    /// <summary>
    /// Disable all cells and enables needed amount.
    /// </summary>
    private void HandleCellsAmount() {
        DisableAllCells();
        for(int i = 0; i < CurrentMatrixSize * CurrentMatrixSize; i++) {
            _rTransform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void DisableAllCells() {
        for(int i = 0; i < _rTransform.childCount; i++) {
            _rTransform.GetChild(i).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Paint cells and determines their activity.
    /// </summary>
    private void PaintCells() {
        for (int i = 0; i < CurrentMatrixSize; i++) {
            for (int j = 0; j < CurrentMatrixSize; j++) {
                MatrixOfCells[i, j].InitializeCell((j - i)%2 != 0, new BoardPosition(i, j));
            }
        }
    }

    #endregion
}
