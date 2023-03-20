using System.Collections.Generic;
using UnityEngine;

public static class PredicatedRules
{
    public static bool MajorityConditionPredicate(Cell chosenCell, BoardPosition targetPosition) {
        if (!chosenCell.HaveMajorCheckerOn) {
            switch(chosenCell.TypeOfCheckerOn) {
                case CheckerType.bot:
                    return targetPosition.x == 0;
                case CheckerType.top: 
                    return targetPosition.x == Board.MatrixSize - 1;
                case CheckerType.left:
                    return targetPosition.y == Board.MatrixSize - 1;
                case CheckerType.right: 
                    return targetPosition.y == 0;
                default: {
                    Debug.LogError("Unsupported checker type");
                    return false;
                }
            }
        }

        return true;
    }

    public static bool CheckerMoveCondition(bool isMajorChecker, BoardPosition targetPosition, BoardPosition currentPosition) {
        if (!isMajorChecker) 
            return 
                (targetPosition.x != currentPosition.x && targetPosition.y != currentPosition.y) && 
                (Mathf.Abs(targetPosition.x - currentPosition.x) == 1 && Mathf.Abs(targetPosition.y - currentPosition.y) == 1);
        else 
            return 
                (targetPosition.x != currentPosition.x && targetPosition.y != currentPosition.y) && 
                (Mathf.Abs(targetPosition.x - currentPosition.x) == Mathf.Abs(targetPosition.y - currentPosition.y));
    }

    public static bool CheckerAttackCondition(CheckerType attackingCheckerType, BoardPosition targetPosition, BoardPosition currentPosition) {
        if (attackingCheckerType is CheckerType.bot or CheckerType.top) {
            return 
                (Mathf.Abs(targetPosition.x - currentPosition.x) == 2) &&
                (targetPosition.x != currentPosition.x && targetPosition.y != currentPosition.y);
        }

        if (attackingCheckerType is CheckerType.right or CheckerType.left) { 
            return 
                (Mathf.Abs(targetPosition.y - currentPosition.y) == 2) &&
                (targetPosition.x != currentPosition.x && targetPosition.y != currentPosition.y);
        }

        Debug.LogError("Checker attack condition is unsatisfied. Something wrong with attackingCheckerType.");
        return false;
    }

    public static List<Cell> MajorMoveAdditionalCondition(BoardPosition currentPosition, BoardPosition targetPosition) {
        BoardPosition checkingPosition = currentPosition;
        List<Cell> checkersOnTheWay = new List<Cell>();

        while (checkingPosition.x != targetPosition.x || checkingPosition.y != targetPosition.y) {
            checkingPosition.x = targetPosition.x > currentPosition.x ? checkingPosition.x + 1 : checkingPosition.x - 1;
            checkingPosition.y = targetPosition.y > currentPosition.y ? checkingPosition.y + 1 : checkingPosition.y - 1;
            
            
            // Count all potential -Enemy- victims between start pos and target pos.
            if (Board.MatrixOfCells[checkingPosition.x, checkingPosition.y].HaveCheckerOn) {
                checkersOnTheWay.Add(Board.MatrixOfCells[checkingPosition.x, checkingPosition.y]);
                // U can't kill more then 1 enemy between chosen and target position. Only by couple moves
                if (checkersOnTheWay.Count > 1) {
                    checkersOnTheWay = null;
                    return null;
                }
            }
        }
        
        return checkersOnTheWay;
    }

    public static bool IsAttackVariationExist() {
        for(int x = 0; x < Board.MatrixSize; x++) {
            for (int y = 0; y < Board.MatrixSize; y++) {
                if (Board.MatrixOfCells[x,y].TypeOfCheckerOn == PlayersController.CurrentCheckersTypeTurn) {
                    if (!Board.MatrixOfCells[x,y].HaveMajorCheckerOn && IsExtraCheckerAttacksAvaiable(Board.MatrixOfCells[x,y].Position) || 
                        Board.MatrixOfCells[x,y].HaveMajorCheckerOn && IsMajorMovesAvaiable(Board.MatrixOfCells[x,y].Position)) {

                            if (IsExtraCheckerAttacksAvaiable(Board.MatrixOfCells[x,y].Position)) {
                                Debug.Log("Checker moves avaiable for + " + Board.MatrixOfCells[x,y].Position.x + " " + Board.MatrixOfCells[x,y].Position.y);
                            }
                            return true;
                    }
                }
            }
        }

        return false;
    }

    public static bool IsExtraCheckerAttacksAvaiable(BoardPosition freshPosition) {
        if ((freshPosition.x - 2) is >= 0 and <= 10) {
            if ((freshPosition.y - 2) is >= 0 and <= 10) {
                if (!Board.MatrixOfCells[freshPosition.x - 2, freshPosition.y - 2].HaveCheckerOn) {
                    if (Board.MatrixOfCells[freshPosition.x - 1, freshPosition.y - 1].HaveCheckerOn && Board.MatrixOfCells[freshPosition.x - 1, freshPosition.y - 1].TypeOfCheckerOn != PlayersController.CurrentCheckersTypeTurn) { 
                        return true;
                    }
                }
            }

            if ((freshPosition.y + 2) is >= 0 and <= 10) {
                if (!Board.MatrixOfCells[freshPosition.x - 2, freshPosition.y + 2].HaveCheckerOn) {
                    if (Board.MatrixOfCells[freshPosition.x - 1, freshPosition.y + 1].HaveCheckerOn && Board.MatrixOfCells[freshPosition.x - 1, freshPosition.y + 1].TypeOfCheckerOn != PlayersController.CurrentCheckersTypeTurn) { 
                        return true;
                    }
                }
            }
        }

        if ((freshPosition.x + 2) is >= 0 and <= 10) {
            if ((freshPosition.y - 2) is >= 0 and <= 10) {
                if (!Board.MatrixOfCells[freshPosition.x + 2, freshPosition.y - 2].HaveCheckerOn) {
                    if (Board.MatrixOfCells[freshPosition.x + 1, freshPosition.y - 1].HaveCheckerOn && Board.MatrixOfCells[freshPosition.x + 1, freshPosition.y - 1].TypeOfCheckerOn != PlayersController.CurrentCheckersTypeTurn) {
                        return true;
                    }
                }
            }

            if ((freshPosition.y + 2) is >= 0 and <= 10) {
                if (!Board.MatrixOfCells[freshPosition.x + 2, freshPosition.y + 2].HaveCheckerOn) {
                    if (Board.MatrixOfCells[freshPosition.x + 1, freshPosition.y + 1].HaveCheckerOn && Board.MatrixOfCells[freshPosition.x + 1, freshPosition.y + 1].TypeOfCheckerOn != PlayersController.CurrentCheckersTypeTurn) {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public static bool IsMajorMovesAvaiable(BoardPosition freshPosition) {
        if (freshPosition.x > 1) { // TOP DIRECTIONS
            for(int i = freshPosition.x - 1, columnMargin = 1; i > 0; i--, columnMargin++) {
                
                // TOP LEFT WAY
                if (freshPosition.y - columnMargin > 0) {
                    if (Board.MatrixOfCells[i, freshPosition.y - columnMargin].HaveCheckerOn) { // Finding any checker on current major checker way
                        Debug.Log("SOME ON MY TOP LEFT WAY");
                        if (Board.MatrixOfCells[i, freshPosition.y - columnMargin].IsEnemyForCurrent(PlayersController.CurrentCheckersTypeTurn)) { // Check is it enemy checker?
                            Debug.Log("IT'S ENEMY!TL");
                            if (!Board.MatrixOfCells[i - 1, freshPosition.y - columnMargin - 1].HaveCheckerOn) { // Is there empty cell behind enemy ?
                                Debug.Log("I CAN KILL HIM!TL");
                                return true;
                            }
                        }
                    }
                } 

                // TOP RIGHT WAY
                if (freshPosition.y + columnMargin <= Board.MatrixSize - 2) {
                    if (Board.MatrixOfCells[i, freshPosition.y + columnMargin].HaveCheckerOn) { // Finding any checker on current major checker way
                        Debug.Log("SOME ON MY TOP RIGHT WAY");
                        if (Board.MatrixOfCells[i, freshPosition.y + columnMargin].IsEnemyForCurrent(PlayersController.CurrentCheckersTypeTurn)) { // Check is it enemy checker?
                            Debug.Log("IT'S ENEMY!TR");
                            if (!Board.MatrixOfCells[i - 1, freshPosition.y + columnMargin + 1].HaveCheckerOn) { // Is there empty cell behind enemy ?
                                Debug.Log("I CAN KILL HIM!TR");
                                return true;
                            }
                        }
                    }
                }
            } 
        }

        if (freshPosition.x < Board.MatrixSize - 2) { // BOT DIRECTIONS
            for(int i = freshPosition.x + 1, columnMargin = 1; i < Board.MatrixSize - 1; i++, columnMargin++) {
                // BOT LEFT WAY
                if (freshPosition.y - columnMargin > 0) {
                    if (Board.MatrixOfCells[i, freshPosition.y - columnMargin].HaveCheckerOn) { // Finding any checker on current major checker way
                        Debug.Log("SOME ON MY BOT LEFT WAY");
                        if (Board.MatrixOfCells[i, freshPosition.y - columnMargin].IsEnemyForCurrent(PlayersController.CurrentCheckersTypeTurn)) { // Check is it enemy checker?
                            Debug.Log("IT'S ENEMY!BL");
                            if (!Board.MatrixOfCells[i + 1, freshPosition.y - columnMargin - 1].HaveCheckerOn) { // Is there empty cell behind enemy ?
                                Debug.Log("I CAN KILL HIM!BL");
                                return true;
                            }
                        }
                    }
                }

                // BOT RIGHT WAY
                if (freshPosition.y + columnMargin <= Board.MatrixSize - 2) {
                    if (Board.MatrixOfCells[i, freshPosition.y + columnMargin].HaveCheckerOn) { // Finding any checker on current major checker way
                        Debug.Log("SOME ON MY BOT RIGHT WAY");
                        if (Board.MatrixOfCells[i, freshPosition.y + columnMargin].IsEnemyForCurrent(PlayersController.CurrentCheckersTypeTurn)) { // Check is it enemy checker?
                            Debug.Log("IT'S ENEMY!BR");
                            if (!Board.MatrixOfCells[i + 1, freshPosition.y + columnMargin + 1].HaveCheckerOn) { // Is there empty cell behind enemy ?
                                Debug.Log("I CAN KILL HIM!BR");
                                return true;
                            }
                        }
                    }
                }
            } 
        }

        return false;
    }
}
