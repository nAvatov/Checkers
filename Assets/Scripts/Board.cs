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
        //PlaceCheckers();
        TestPlacing();
         _notificationManager.ShowNotification(PlayersController.CurrentCheckersTypeTurn.ToString() + ", you first");
    }

    private void InitializeCells() {
        int k = 0;
        for (int i = 0; i < _matrixSize; i++) {
            for (int j = 0; j < _matrixSize; j++) {
                _rTransform.GetChild(k).GetComponent<Cell>().InitializeCell(k%2 != 0, new BoardPosition(i, j)); // Paint cells
                _matrixOfCells[i, j] = _rTransform.GetChild(k).GetComponent<Cell>(); // Fill the matrix with cells
                k++;
            }
        }
    }

    private void PlaceCheckers() {
        for(int i = 0; i < 3; i++ ) {
            for (int j = i + 1; j < _matrixSize - i; j += 2) {
                _matrixOfCells[i, j].AddCheckerToCell(CheckerType.top);
                _matrixOfCells[j, i].AddCheckerToCell(CheckerType.left);
                _matrixOfCells[_matrixSize - 1 - i, j].AddCheckerToCell(CheckerType.bot);
                _matrixOfCells[j, _matrixSize - 1 - i].AddCheckerToCell(CheckerType.right);
            }
        }
    }

    private void TestPlacing() {
         _matrixOfCells[1, 4].AddCheckerToCell(CheckerType.top);
         _matrixOfCells[2, 5].AddCheckerToCell(CheckerType.bot);
         _matrixOfCells[3, 6].AddCheckerToCell(CheckerType.top);
    }
}
