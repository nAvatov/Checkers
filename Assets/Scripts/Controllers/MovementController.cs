using System.Collections.Generic;

public static class MovementController
{
    private static BoardPosition _chosenPosition = new BoardPosition(-1, -1);
    public static System.Action _strokeTransition;

    #region Public Methods
        
    public static void SetChosenChecker(BoardPosition targetPosition) { 
        if (_chosenPosition.x == -1 || _chosenPosition.y == -1) { 
            _chosenPosition = targetPosition;
            Board.MatrixOfCells[_chosenPosition.x, _chosenPosition.y].HandleCellSelection(true);
            return;
        }

        Cell chosenBefore = Board.MatrixOfCells[_chosenPosition.x, _chosenPosition.y];
        chosenBefore.HandleCellSelection(false);

        if (!chosenBefore.HaveCheckerOn) {
            _chosenPosition.x = -1; _chosenPosition.y = -1;
            return;
        }

        if (!chosenBefore.HaveMajorCheckerOn) {
            if (!PredicatedRules.IsAttackVariationExist()) {
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
            } else {
                HandleAttack(chosenBefore, targetPosition);
            }
        }
        else {
            HandleMajorCheckerMoves(chosenBefore, targetPosition);
        }

        _chosenPosition.x = -1; _chosenPosition.y = -1;
    }

    #endregion


    #region Private Methods
    private static void MoveChecker(Cell previousCell, BoardPosition targetPosition, bool legalDirection) {
        if (legalDirection && PredicatedRules.CheckerMoveCondition(previousCell.HaveMajorCheckerOn, targetPosition, _chosenPosition)) {
            if (!Board.MatrixOfCells[targetPosition.x, targetPosition.y].HaveCheckerOn) {
                ExchangeCells(previousCell, targetPosition, PredicatedRules.MajorityConditionPredicate(previousCell, targetPosition), new BoardPosition(-1, -1));
                SoundController.PlaySound(SoundTypes.move);
            }
        }
    }

    private static void ExchangeCells(Cell previousCell, BoardPosition targetPosition, bool isBecomingMajor, BoardPosition victimPosition) {
        Cell transitionCellState = previousCell;

        if (victimPosition.x != -1 && victimPosition.y != -1) { 
            PlayersController.ReduceCheckerFromPlayer(Board.MatrixOfCells[victimPosition.x, victimPosition.y].TypeOfCheckerOn);
            Board.MatrixOfCells[victimPosition.x, victimPosition.y].HandleCheckerOnMe(Board.MatrixOfCells[victimPosition.x, victimPosition.y].TypeOfCheckerOn, false, false);
            PlaceCheckerOnCell(transitionCellState, targetPosition, isBecomingMajor, false);
        } else {
            PlaceCheckerOnCell(transitionCellState, targetPosition, isBecomingMajor, true);
        }

        previousCell.HandleCheckerOnMe(transitionCellState.TypeOfCheckerOn, false);
    }

    private static void PlaceCheckerOnCell(Cell previousCell, BoardPosition targetPosition, bool isMajorNow, bool noVictim) {
        if (isMajorNow) {
            Board.MatrixOfCells[targetPosition.x, targetPosition.y].HandleCheckerOnMe(previousCell.TypeOfCheckerOn, true, true); // Place checker on new position (2nd click)
            // Change player if current fresh major checker don't have any avaiable attack variants 
            if (!PredicatedRules.IsMajorMovesAvaiable(targetPosition) && noVictim || previousCell.HaveMajorCheckerOn) { 
                _strokeTransition.Invoke();
            }
        } else {
            Board.MatrixOfCells[targetPosition.x, targetPosition.y].HandleCheckerOnMe(previousCell.TypeOfCheckerOn); // Place checker on new position (2nd click)
            // Change player if placing is clear or if placing after killing enemy checker and no extra attacks is avaiable
            if (noVictim || !noVictim && !PredicatedRules.IsExtraCheckerAttacksAvaiable(targetPosition)) {
                _strokeTransition.Invoke();
            }
        }
    }

    private static void HandleAttack(Cell previousCell, BoardPosition targetPosition) {
        if (PredicatedRules.CheckerAttackCondition(previousCell.TypeOfCheckerOn, targetPosition, _chosenPosition)) {
            if (!Board.MatrixOfCells[targetPosition.x, targetPosition.y].HaveCheckerOn) {
                BoardPosition victimPosition;
                victimPosition.x = targetPosition.x > _chosenPosition.x ? targetPosition.x - 1 : targetPosition.x + 1;
                victimPosition.y = targetPosition.y > _chosenPosition.y ? targetPosition.y - 1 : targetPosition.y + 1;
                if (Board.MatrixOfCells[victimPosition.x, victimPosition.y].HaveCheckerOn && Board.MatrixOfCells[victimPosition.x, victimPosition.y].TypeOfCheckerOn != previousCell.TypeOfCheckerOn) {
                    ExchangeCells(previousCell, targetPosition, PredicatedRules.MajorityConditionPredicate(previousCell, targetPosition), victimPosition);
                    SoundController.PlaySound(SoundTypes.attack);
                }
            }
        }
    }

    private static void HandleMajorCheckerMoves(Cell previousCell, BoardPosition targetPosition) {
        if (!Board.MatrixOfCells[targetPosition.x, targetPosition.y].HaveCheckerOn && PredicatedRules.CheckerMoveCondition(previousCell.HaveMajorCheckerOn, targetPosition, _chosenPosition)) { // Check if chosen cell have checker on and new cell doesn't have yet
            CheckMajorCheckerMoveType(previousCell, targetPosition);
        }
    }

    private static void CheckMajorCheckerMoveType(Cell majorCheckerCell, BoardPosition targetPosition) {
        List<Cell> checkersOnWay = PredicatedRules.MajorMoveAdditionalCondition(_chosenPosition, targetPosition);
        
        if (checkersOnWay != null) {
            if (checkersOnWay.Count == 0 && !PredicatedRules.IsAttackVariationExist()) {
                MoveChecker(majorCheckerCell, targetPosition, true);
                return;
            } 
            if (checkersOnWay.Count > 0) {
                MajorAttack(majorCheckerCell, targetPosition, checkersOnWay[0]);
                return;
            }
        }
    }

    private static void MajorAttack(Cell majorCheckerCell, BoardPosition targetPosition, Cell victimCheckerCell) {
        if (victimCheckerCell.TypeOfCheckerOn != majorCheckerCell.TypeOfCheckerOn) {
            CheckerType transitionCheckerBuffer = majorCheckerCell.TypeOfCheckerOn;
            majorCheckerCell.HandleCheckerOnMe(transitionCheckerBuffer, false, false); 
            
            PlayersController.ReduceCheckerFromPlayer(victimCheckerCell.TypeOfCheckerOn);
            victimCheckerCell.HandleCheckerOnMe(victimCheckerCell.TypeOfCheckerOn, false, false);

            Board.MatrixOfCells[targetPosition.x, targetPosition.y].HandleCheckerOnMe(transitionCheckerBuffer, true, true);
            if (!PredicatedRules.IsMajorMovesAvaiable(targetPosition)) {
                _strokeTransition.Invoke();
            }
        }
    }

    #endregion
}
