using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static class describes checkers limitations and rules programmatically.
/// </summary>
public static class ConditionalRules
{
    /// <summary>
    /// Determines checker major state ability due to target position
    /// </summary>
    public static bool BecomingMajorCondition(Cell chosenCell, BoardPosition targetPosition) {
        if (!chosenCell.HaveMajorCheckerOn) {
            switch(chosenCell.TypeOfCheckerOn) {
                case CheckerType.bot:
                    return targetPosition.x == 0;
                case CheckerType.top: 
                    return targetPosition.x == Board.CurrentMatrixSize - 1;
                case CheckerType.left:
                    return targetPosition.y == Board.CurrentMatrixSize - 1;
                case CheckerType.right: 
                    return targetPosition.y == 0;
                default: {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Returns move avaiability status for both checker types.
    /// </summary>
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

    /// <summary>
    /// Returns attack avaiability for simple checker.
    /// </summary>
    public static bool CheckerAttackCondition(CheckerType attackingCheckerType, BoardPosition targetPosition, BoardPosition currentPosition) {
        return  Mathf.Abs(targetPosition.x - currentPosition.x) == 2 && 
                Mathf.Abs(targetPosition.y - currentPosition.y) == 2 && 
                targetPosition.x != currentPosition.x && 
                targetPosition.y != currentPosition.y;
    }

    /// <summary>
    /// Calculates the list of obstacles (checkers) for current major checker.
    /// </summary>
    public static List<Cell> ObstaclesForMajor(BoardPosition currentPosition, BoardPosition targetPosition) {
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

    /// <summary>
    /// Core condition for searching move-action avaiability.
    /// </summary>
    public static bool IsAttackVariationExist() {
        for(int x = 0; x < Board.CurrentMatrixSize; x++) {
            for (int y = 0; y < Board.CurrentMatrixSize; y++) {
                if (Board.MatrixOfCells[x,y].TypeOfCheckerOn == PlayersController.CurrentCheckersTypeTurn) {
                    if (!Board.MatrixOfCells[x,y].HaveMajorCheckerOn && IsExtraCheckerAttacksAvaiable(Board.MatrixOfCells[x,y].Position) || 
                        Board.MatrixOfCells[x,y].HaveMajorCheckerOn && IsExtraMajorAttacksAvaiable(Board.MatrixOfCells[x,y].Position)) {
                            return true;
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Returns extra attack avaiability for simple checker.
    /// </summary>
    public static bool IsExtraCheckerAttacksAvaiable(BoardPosition freshPosition) {
        if ((freshPosition.x - 2) >= 0 && (freshPosition.x - 2) <= Board.CurrentMatrixSize - 1) {
            if ((freshPosition.y - 2) >= 0 && (freshPosition.y - 2) <= Board.CurrentMatrixSize - 1) {
                if (!Board.MatrixOfCells[freshPosition.x - 2, freshPosition.y - 2].HaveCheckerOn) {
                    if (Board.MatrixOfCells[freshPosition.x - 1, freshPosition.y - 1].HaveCheckerOn && Board.MatrixOfCells[freshPosition.x - 1, freshPosition.y - 1].TypeOfCheckerOn != PlayersController.CurrentCheckersTypeTurn) {
                        TipsController.HighlightTip(Board.MatrixOfCells[freshPosition.x - 1, freshPosition.y - 1]); 
                        return true;
                    }
                }
            }

            if ((freshPosition.y + 2) >= 0 && (freshPosition.y + 2) <= Board.CurrentMatrixSize - 1) {
                if (!Board.MatrixOfCells[freshPosition.x - 2, freshPosition.y + 2].HaveCheckerOn) {
                    if (Board.MatrixOfCells[freshPosition.x - 1, freshPosition.y + 1].HaveCheckerOn && Board.MatrixOfCells[freshPosition.x - 1, freshPosition.y + 1].TypeOfCheckerOn != PlayersController.CurrentCheckersTypeTurn) {
                        TipsController.HighlightTip(Board.MatrixOfCells[freshPosition.x - 1, freshPosition.y + 1]);
                        return true;
                    }
                }
            }
        }

        if ((freshPosition.x + 2) >= 0 && (freshPosition.x + 2) <= Board.CurrentMatrixSize - 1) {
            if ((freshPosition.y - 2) >= 0 && (freshPosition.y - 2) <= Board.CurrentMatrixSize - 1) {
                if (!Board.MatrixOfCells[freshPosition.x + 2, freshPosition.y - 2].HaveCheckerOn) {
                    if (Board.MatrixOfCells[freshPosition.x + 1, freshPosition.y - 1].HaveCheckerOn && Board.MatrixOfCells[freshPosition.x + 1, freshPosition.y - 1].TypeOfCheckerOn != PlayersController.CurrentCheckersTypeTurn) {
                        TipsController.HighlightTip(Board.MatrixOfCells[freshPosition.x + 1, freshPosition.y - 1]);
                        return true;
                    }
                }
            }

            if ((freshPosition.y + 2) >= 0 && (freshPosition.y + 2) <= Board.CurrentMatrixSize - 1) {
                Debug.Log(freshPosition.x + " " + freshPosition.y);
                if (!Board.MatrixOfCells[freshPosition.x + 2, freshPosition.y + 2].HaveCheckerOn) {
                    if (Board.MatrixOfCells[freshPosition.x + 1, freshPosition.y + 1].HaveCheckerOn && Board.MatrixOfCells[freshPosition.x + 1, freshPosition.y + 1].TypeOfCheckerOn != PlayersController.CurrentCheckersTypeTurn) {
                        TipsController.HighlightTip(Board.MatrixOfCells[freshPosition.x + 1, freshPosition.y + 1]);
                        return true;
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Returns extra attacks avaiability for major checker.
    /// </summary>
    public static bool IsExtraMajorAttacksAvaiable(BoardPosition freshPosition) {
        // TOP DIRECTIONS
        if (freshPosition.x > 1) {
            for(int i = freshPosition.x - 1, columnMargin = 1; i > 0; i--, columnMargin++) {
                // TOP LEFT WAY
                if (freshPosition.y - columnMargin > 0) {
                    if (Board.MatrixOfCells[i, freshPosition.y - columnMargin].HaveCheckerOn) {
                        if (Board.MatrixOfCells[i, freshPosition.y - columnMargin].IsEnemyForCurrent(PlayersController.CurrentCheckersTypeTurn)) {
                            if (!Board.MatrixOfCells[i - 1, freshPosition.y - columnMargin - 1].HaveCheckerOn) {
                                TipsController.HighlightTip(Board.MatrixOfCells[i, freshPosition.y - columnMargin]);
                                return true;
                            }
                        }
                    }
                } 

                // TOP RIGHT WAY
                if (freshPosition.y + columnMargin <= Board.CurrentMatrixSize - 2) {
                    if (Board.MatrixOfCells[i, freshPosition.y + columnMargin].HaveCheckerOn) {
                        if (Board.MatrixOfCells[i, freshPosition.y + columnMargin].IsEnemyForCurrent(PlayersController.CurrentCheckersTypeTurn)) {
                            if (!Board.MatrixOfCells[i - 1, freshPosition.y + columnMargin + 1].HaveCheckerOn) {
                                TipsController.HighlightTip(Board.MatrixOfCells[i, freshPosition.y + columnMargin]);
                                return true;
                            }
                        }
                    }
                }
            } 
        }
        // BOT DIRECTIONS
        if (freshPosition.x < Board.CurrentMatrixSize - 2) {
            for(int i = freshPosition.x + 1, columnMargin = 1; i < Board.CurrentMatrixSize - 1; i++, columnMargin++) {
                // BOT LEFT WAY
                if (freshPosition.y - columnMargin > 0) {
                    if (Board.MatrixOfCells[i, freshPosition.y - columnMargin].HaveCheckerOn) {
                        if (Board.MatrixOfCells[i, freshPosition.y - columnMargin].IsEnemyForCurrent(PlayersController.CurrentCheckersTypeTurn)) {
                            if (!Board.MatrixOfCells[i + 1, freshPosition.y - columnMargin - 1].HaveCheckerOn) {
                                TipsController.HighlightTip(Board.MatrixOfCells[i, freshPosition.y - columnMargin]);
                                return true;
                            }
                        }
                    }
                }

                // BOT RIGHT WAY
                if (freshPosition.y + columnMargin <= Board.CurrentMatrixSize - 2) {
                    if (Board.MatrixOfCells[i, freshPosition.y + columnMargin].HaveCheckerOn) {
                        if (Board.MatrixOfCells[i, freshPosition.y + columnMargin].IsEnemyForCurrent(PlayersController.CurrentCheckersTypeTurn)) {
                            if (!Board.MatrixOfCells[i + 1, freshPosition.y + columnMargin + 1].HaveCheckerOn) {
                                TipsController.HighlightTip(Board.MatrixOfCells[i, freshPosition.y + columnMargin]);
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
