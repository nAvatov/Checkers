public class CheckersPlayer
{
    private CheckerType _checkersType;
    private int _checkersAmount;

    public int CheckersAmount { 
        set {
            _checkersAmount = value;
        }

        get { 
            return _checkersAmount;
        }
    }

    public CheckerType CheckerType { 
        get {
            return _checkersType;
        }
    }

    public CheckersPlayer(CheckerType checkersType, int checkersAmount) {
        _checkersType = checkersType;
        _checkersAmount = checkersAmount;
    }

    public void ReduceChecker() {
        if (_checkersAmount > 0) {
            _checkersAmount--;
        }
    }
}
