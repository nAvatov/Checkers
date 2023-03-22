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
    public static string[] checkersType;
    [SerializeField] private Notification _notificationManager;
    private const int fourPlayersMatrixSize = 11;
    private const int twoPlayersMatrixSize = 8;
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

    #region UnityMethods

    private void Awake() {
        _rTransform = GetComponent<RectTransform>();
    }

    private void Start() {
        StartGame(true);
    }

    #endregion

    #region Public Methods

    public void StartGame(bool isFourPlayers) {
        _currentMatrixSize = isFourPlayers ? fourPlayersMatrixSize : twoPlayersMatrixSize;
        HandleCellsAmount();
        
        _matrixOfCells = new Cell[_currentMatrixSize, _currentMatrixSize];
        
        PlayersController.RefreshPlayers();
        InitializeCells();
        RefreshBoardCells();

        if (isFourPlayers) {
            Spawn4Checkers();
        } else {
            Spawn2Checkers();
        }
        
        _notificationManager.ShowNotification(PlayersController.CurrentCheckersTypeTurn.ToString() + ", you first");
    }
        
    #endregion

    #region Private Methods

    private void InitializeCells() {
        int k = 0;
        for (int i = 0; i < CurrentMatrixSize; i++) {
            for (int j = 0; j < CurrentMatrixSize; j++) {
                if (_rTransform.GetChild(k).gameObject.activeSelf) {
                    _rTransform.GetChild(k).GetComponent<Cell>().InitializeCell(k%2 != 0, new BoardPosition(i, j)); // Paint cells
                    _matrixOfCells[i, j] = _rTransform.GetChild(k).GetComponent<Cell>(); // Fill the matrix with cells
                    k++;
                }
            }
        }
    }

    private void Spawn2Checkers()
    {
        for(int i = 0; i < 3; i++ ) {
            for (int j = i % 2 == 0 ? 1 : 0; j < twoPlayersMatrixSize; j += 2) {
                _matrixOfCells[i, j].AddCheckerToCell(CheckerType.top);
                _matrixOfCells[twoPlayersMatrixSize - 1 - i, j].AddCheckerToCell(CheckerType.bot);
            }
        }
    }

    private void Spawn4Checkers() {
        for(int i = 0; i < 3; i++ ) {
            for (int j = i + 1; j < fourPlayersMatrixSize - i; j += 2) {
                _matrixOfCells[i, j].AddCheckerToCell(CheckerType.top);
                _matrixOfCells[j, i].AddCheckerToCell(CheckerType.left);
                _matrixOfCells[fourPlayersMatrixSize - 1 - i, j].AddCheckerToCell(CheckerType.bot);
                _matrixOfCells[j, fourPlayersMatrixSize - 1 - i].AddCheckerToCell(CheckerType.right);
            }
        }
    }

    private void RefreshBoardCells() {
        for (int i = 0; i < CurrentMatrixSize; i++) {
            for (int j = 0; j < CurrentMatrixSize; j++) { 
                _matrixOfCells[i, j].HandleCheckerOnMe(CheckerType.noType, false);
            }
        }
    }

    private void HandleCellsAmount() {
        for(int i = 0; i < CurrentMatrixSize * CurrentMatrixSize; i++) {
            _rTransform.GetChild(i).gameObject.SetActive(true);
        }
    }

    #endregion
}
