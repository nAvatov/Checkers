using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersPlayer
{
    private CheckerType myCheckerColor;
    private int checkersAmount;

    public int CheckersAmount { 
        set {
            checkersAmount = value;
        }

        get { 
            return checkersAmount;
        }
    }

    public CheckerType CheckerType { 
        get {
            return myCheckerColor;
        }
    }

    public CheckersPlayer(CheckerType _checkersTypeColor, int _checkersAmount) {
        myCheckerColor = _checkersTypeColor;
        checkersAmount = _checkersAmount;
    }

    public void ReduceCheckers(int reduceAmount) {
        if (reduceAmount < checkersAmount) {
            checkersAmount -= reduceAmount;
        }
    }

    public void ReduceChecker() {
        if (checkersAmount > 0) {
            checkersAmount--;
        }
    }
}
