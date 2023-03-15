using System.Collections.Generic;

public static class MovementController
{
    private static Position _chosenPosition = new Position(-1, -1);
    public static System.Action _legalMoveDone;

    #region Public Methods
        
    public static void SetChosenChecker(Position targetPosition) { 
        // Reselection
        if (_chosenPosition.x == -1 || _chosenPosition.y == -1) { 
            _chosenPosition = targetPosition;
            Board.MatrixOfCells[_chosenPosition.x, _chosenPosition.y].HandleCellSelection(true);
            return;
        }

        Cell chosenBefore = Board.MatrixOfCells[_chosenPosition.x, _chosenPosition.y];
        chosenBefore.HandleCellSelection(false);

        // CHECKER
        if (!chosenBefore.HaveMajorCheckerOn) {
            switch (chosenBefore.TypeOfCheckerOn) {
                case CheckerType.bot : { 
                    MoveChecker(chosenBefore, targetPosition, targetPosition.x < _chosenPosition.x);
                    break;
                }
                case CheckerType.left: { 
                    MoveChecker(chosenBefore, targetPosition, targetPosition.y > _chosenPosition.y);
                    break;
                }
                case CheckerType.top: {
                    MoveChecker(chosenBefore, targetPosition, targetPosition.x > _chosenPosition.x);
                    break;
                }
                case CheckerType.right: {
                    MoveChecker(chosenBefore, targetPosition, targetPosition.y < _chosenPosition.y);
                    break;
                }
            }
            HandleAttack(chosenBefore, targetPosition);
        }
        // MAJOR CHECKER
        else {
            HandleMajorCheckerMoves(chosenBefore, targetPosition);
        }

        _chosenPosition.x = -1; _chosenPosition.y = -1;
    }

    #endregion


    #region Private Methods
    private static void MoveChecker(Cell previousCell, Position targetPosition, bool legalDirection) {
        if (legalDirection && PredicatedRules.CheckerMoveCondition(previousCell.HaveMajorCheckerOn, targetPosition, _chosenPosition)) {
            if (!Board.MatrixOfCells[targetPosition.x, targetPosition.y].HaveCheckerOn) {
                ExchangeCells(previousCell, targetPosition, PredicatedRules.MajorityConditionPredicate(previousCell.TypeOfCheckerOn, targetPosition), new Position(-1, -1));
            }
        }
    }

    private static void ExchangeCells(Cell previousCell, Position targetPosition, bool isBecomingMajor, Position victimPosition) {
        CheckerType transitionCheckerBuffer = previousCell.TypeOfCheckerOn;
        previousCell.HandleCheckerOnMe(transitionCheckerBuffer, false);

        if (victimPosition.x != -1 && victimPosition.y != -1) { 
            // Checker is eliminated
            Board.MatrixOfCells[victimPosition.x, victimPosition.y].HandleCheckerOnMe(Board.MatrixOfCells[victimPosition.x, victimPosition.y].TypeOfCheckerOn, false, false);
            GameController.ReduceCheckerFromPlayer(Board.MatrixOfCells[victimPosition.x, victimPosition.y].TypeOfCheckerOn);
            PlaceCheckerOnCell(transitionCheckerBuffer, targetPosition, isBecomingMajor, false);

            if (!PredicatedRules.CheckerMovesAvaiable(targetPosition)) {
                _legalMoveDone.Invoke();
            }
        } else {
            PlaceCheckerOnCell(transitionCheckerBuffer, targetPosition, isBecomingMajor, true);
        }
    }

    private static void PlaceCheckerOnCell(CheckerType transitionChecker, Position targetPosition, bool isMajorNow, bool noVictim) {
        if (isMajorNow) {
            Board.MatrixOfCells[targetPosition.x, targetPosition.y].HaveMajorCheckerOn = true; 
            Board.MatrixOfCells[targetPosition.x, targetPosition.y].HandleCheckerOnMe(transitionChecker, true, true); // Place checker on new position (2nd click)
            if (!PredicatedRules.MajorMovesAvaiable(targetPosition) && noVictim) { 
                _legalMoveDone.Invoke();
            }
        } else {
            Board.MatrixOfCells[targetPosition.x, targetPosition.y].HandleCheckerOnMe(transitionChecker); // Place checker on new position (2nd click)
            if (noVictim) _legalMoveDone.Invoke();
        }
    }

    private static void HandleAttack(Cell previousCell, Position targetPosition) {
        if (PredicatedRules.CheckerAttackCondition(previousCell.TypeOfCheckerOn, targetPosition, _chosenPosition)) {
            if (previousCell.HaveCheckerOn && !Board.MatrixOfCells[targetPosition.x, targetPosition.y].HaveCheckerOn) { // Check if chosen cell have checker on and new cell doesn't have yet
                // Getting info about victim
                Position victimPosition;
                victimPosition.x = targetPosition.x > _chosenPosition.x ? targetPosition.x - 1 : targetPosition.x + 1;
                victimPosition.y = targetPosition.y > _chosenPosition.y ? targetPosition.y - 1 : targetPosition.y + 1;
                // Check if there any checker between chosen pos and new pos and don't hit your troops
                if (Board.MatrixOfCells[victimPosition.x, victimPosition.y].HaveCheckerOn && Board.MatrixOfCells[victimPosition.x, victimPosition.y].TypeOfCheckerOn != previousCell.TypeOfCheckerOn) {
                    ExchangeCells(previousCell, targetPosition, PredicatedRules.MajorityConditionPredicate(previousCell.TypeOfCheckerOn, targetPosition), victimPosition);
                }
            }
        }
    }

    private static void HandleMajorCheckerMoves(Cell previousCell, Position targetPosition) {
        if (!Board.MatrixOfCells[targetPosition.x, targetPosition.y].HaveCheckerOn && PredicatedRules.CheckerMoveCondition(previousCell.HaveMajorCheckerOn, targetPosition, _chosenPosition)) { // Check if chosen cell have checker on and new cell doesn't have yet
            CheckMajorCheckerMoveType(previousCell, targetPosition);
        }
    }

    private static void CheckMajorCheckerMoveType(Cell majorCheckerCell, Position targetPosition) {
        List<Cell> checkersOnWay = PredicatedRules.MajorMoveAdditionalCondition(_chosenPosition, targetPosition);
        
        if (checkersOnWay != null) {
            if (checkersOnWay.Count == 0) {
                MoveChecker(majorCheckerCell, targetPosition, true);
            } else {
                MajorAttack(majorCheckerCell, targetPosition, checkersOnWay[0]);
            }
        }
    }

    private static void MajorAttack(Cell majorCheckerCell, Position targetPosition, Cell victimCheckerCell) {
        if (victimCheckerCell.TypeOfCheckerOn != majorCheckerCell.TypeOfCheckerOn) {
            CheckerType transitionCheckerBuffer = majorCheckerCell.TypeOfCheckerOn;
            majorCheckerCell.HandleCheckerOnMe(transitionCheckerBuffer, false, false); 
            
            // Killing the victim. Be sure that u killed any possible type of enemy checker (major also killable)
            victimCheckerCell.HandleCheckerOnMe(victimCheckerCell.TypeOfCheckerOn, false, false);
            GameController.ReduceCheckerFromPlayer(victimCheckerCell.TypeOfCheckerOn);

            Board.MatrixOfCells[targetPosition.x, targetPosition.y].HandleCheckerOnMe(transitionCheckerBuffer, true, true);
            if (!PredicatedRules.MajorMovesAvaiable(targetPosition)) {
                _legalMoveDone.Invoke();
            }
        }
    }

    #endregion
}
