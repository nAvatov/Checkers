using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CheckerType {
    bot,
    left,
    top,
    right
}

public class Board : MonoBehaviour
{   
    private const int _matrixSize = 11;
    private static Cell[,] _matrixOfCells;
    private RectTransform _rTransform;

    public static int MatrixSize {
        get {
            return _matrixSize;
        }
    }

    public static Cell[,] MatrixOfCells {
        get {
            return _matrixOfCells;
        }
    }

    private void Awake() {
        _rTransform = GetComponent<RectTransform>();
        _matrixOfCells = new Cell[_matrixSize, _matrixSize];
    }

    private void Start() {
        InitializeCells();
        PlaceCheckers();
    }

    private void InitializeCells() {
        int k = 0;
        for (int i = 0; i < _matrixSize; i++) {
            for (int j = 0; j < _matrixSize; j++) {
                _rTransform.GetChild(k).GetComponent<Cell>().InitializeCell(k%2 != 0, new Position(i, j)); // Paint cells
                _matrixOfCells[i, j] = _rTransform.GetChild(k).GetComponent<Cell>(); // Fill the matrix with cells
                k++;
            }
        }
    }

    private void PlaceCheckers() {
        for(int i = 0; i < 3; i++ ) {
            for (int j = i + 1; j < _matrixSize - i; j += 2) {
                _matrixOfCells[i, j].HandleCheckerOnMe(CheckerType.top);
                _matrixOfCells[j, i].HandleCheckerOnMe(CheckerType.left);
                _matrixOfCells[_matrixSize - 1 - i, j].HandleCheckerOnMe(CheckerType.bot);
                _matrixOfCells[j, _matrixSize - 1 - i].HandleCheckerOnMe(CheckerType.right);
            }
        }

        // players = new List<CheckersPlayer> { 
        //     new CheckersPlayer(CheckerType.top, singleColorCheckersAmount),   
        //     new CheckersPlayer(CheckerType.right, singleColorCheckersAmount),
        //     new CheckersPlayer(CheckerType.bot, singleColorCheckersAmount),
        //     new CheckersPlayer(CheckerType.left, singleColorCheckersAmount)
        // };
    }
}
