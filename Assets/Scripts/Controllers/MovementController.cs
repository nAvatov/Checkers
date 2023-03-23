using System.Collections.Generic;
/// <summary>
/// Static class responsible for abstract checker moves.
/// </summary>
public static class MovementController
{
    private static BoardPosition _chosenPosition = new BoardPosition(-1, -1);
    public static System.Action _strokeTransition;

    #region Public Methods
    /// <summary>
    /// Core method that handles any cell click action and transmits responsibilities further.
    /// </summary>
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
            if (!ConditionalRules.IsAttackVariationExist()) {
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
            MajorMovesEntrance(chosenBefore, targetPosition);
        }

        _chosenPosition.x = -1; _chosenPosition.y = -1;
    }

    #endregion


    #region Private Methods
    /// <summary>
    /// Main move method for both checkers types. Executes checker exchange between cells after satisfying couple conditions.
    /// </summary>
    private static void MoveChecker(Cell previousCell, BoardPosition targetPosition, bool legalDirection) {
        if (legalDirection && ConditionalRules.CheckerMoveCondition(previousCell.HaveMajorCheckerOn, targetPosition, _chosenPosition)) {
            if (!Board.MatrixOfCells[targetPosition.x, targetPosition.y].HaveCheckerOn) {
                ExchangeCells(previousCell, targetPosition, ConditionalRules.BecomingMajorCondition(previousCell, targetPosition), new BoardPosition(-1, -1));
                SoundController.PlaySound(SoundTypes.move);
            }
        }
    }

    /// <summary>
    /// Method for cells exchange operation - checker transition.
    /// </summary>
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

    /// <summary>
    /// Core method for placing checker on cell.
    /// </summary>
    private static void PlaceCheckerOnCell(Cell previousCell, BoardPosition targetPosition, bool isMajorNow, bool noVictim) {
        if (isMajorNow) {
            Board.MatrixOfCells[targetPosition.x, targetPosition.y].HandleCheckerOnMe(previousCell.TypeOfCheckerOn, true, true); // Place checker on new position (2nd click)
            // Change player if current fresh major checker don't have any avaiable attack variants 
            if (!ConditionalRules.IsExtraMajorAttacksAvaiable(targetPosition) && noVictim || previousCell.HaveMajorCheckerOn) { 
                _strokeTransition.Invoke();
            }
        } else {
            Board.MatrixOfCells[targetPosition.x, targetPosition.y].HandleCheckerOnMe(previousCell.TypeOfCheckerOn); // Place checker on new position (2nd click)
            // Change player if placing is clear or if placing after killing enemy checker and no extra attacks is avaiable
            if (noVictim || !noVictim && !ConditionalRules.IsExtraCheckerAttacksAvaiable(targetPosition)) {
                _strokeTransition.Invoke();
            }
        }
    }

    /// <summary>
    /// Attack handler for simple checker.
    /// </summary>
    private static void HandleAttack(Cell previousCell, BoardPosition targetPosition) {
        if (ConditionalRules.CheckerAttackCondition(previousCell.TypeOfCheckerOn, targetPosition, _chosenPosition)) {
            if (!Board.MatrixOfCells[targetPosition.x, targetPosition.y].HaveCheckerOn) {
                BoardPosition victimPosition;
                victimPosition.x = targetPosition.x > _chosenPosition.x ? targetPosition.x - 1 : targetPosition.x + 1;
                victimPosition.y = targetPosition.y > _chosenPosition.y ? targetPosition.y - 1 : targetPosition.y + 1;
                if (Board.MatrixOfCells[victimPosition.x, victimPosition.y].HaveCheckerOn && Board.MatrixOfCells[victimPosition.x, victimPosition.y].TypeOfCheckerOn != previousCell.TypeOfCheckerOn) {
                    ExchangeCells(previousCell, targetPosition, ConditionalRules.BecomingMajorCondition(previousCell, targetPosition), victimPosition);
                    SoundController.PlaySound(SoundTypes.attack);
                }
            }
        }
    }

    /// <summary>
    /// Major checker moves entrance which contains couple conditions to satisfy.
    /// </summary>
    private static void MajorMovesEntrance(Cell previousCell, BoardPosition targetPosition) {
        if (!Board.MatrixOfCells[targetPosition.x, targetPosition.y].HaveCheckerOn && ConditionalRules.CheckerMoveCondition(previousCell.HaveMajorCheckerOn, targetPosition, _chosenPosition)) { // Check if chosen cell have checker on and new cell doesn't have yet
            DetermineMajorMoveType(previousCell, targetPosition);
        }
    }

    /// <summary>
    /// This method determines whether major-action move or attack.
    /// </summary>
    private static void DetermineMajorMoveType(Cell majorCheckerCell, BoardPosition targetPosition) {
        List<Cell> checkersOnWay = ConditionalRules.ObstaclesForMajor(_chosenPosition, targetPosition);
        
        if (checkersOnWay != null) {
            if (checkersOnWay.Count == 0 && !ConditionalRules.IsAttackVariationExist()) {
                MoveChecker(majorCheckerCell, targetPosition, true);
                return;
            } 
            if (checkersOnWay.Count > 0) {
                MajorAttack(majorCheckerCell, targetPosition, checkersOnWay[0]);
                return;
            }
        }
    }

    /// <summary>
    /// Handler for major attack action.
    /// </summary>
    private static void MajorAttack(Cell majorCheckerCell, BoardPosition targetPosition, Cell victimCheckerCell) {
        if (victimCheckerCell.TypeOfCheckerOn != majorCheckerCell.TypeOfCheckerOn) {
            CheckerType transitionCheckerBuffer = majorCheckerCell.TypeOfCheckerOn;
            majorCheckerCell.HandleCheckerOnMe(transitionCheckerBuffer, false, false); 
            
            PlayersController.ReduceCheckerFromPlayer(victimCheckerCell.TypeOfCheckerOn);
            victimCheckerCell.HandleCheckerOnMe(victimCheckerCell.TypeOfCheckerOn, false, false);

            Board.MatrixOfCells[targetPosition.x, targetPosition.y].HandleCheckerOnMe(transitionCheckerBuffer, true, true);
            if (!ConditionalRules.IsExtraMajorAttacksAvaiable(targetPosition)) {
                _strokeTransition.Invoke();
            }
        }
    }

    #endregion
}
