using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CheckerType {
    bot,
    left,
    top,
    right
}

public class GameController : MonoBehaviour
{

    [SerializeField] Notification notification;
    private Cell[,] matrixOfCells;
    private const int matrixSize = 11;
    private int singleColorCheckersAmount = 12;
    private RectTransform rTransform;

    private int chosenX = -1, chosenY = -1;

    private List<CheckersPlayer> players;
    private CheckersPlayer currentPlayer;
    private CheckersPlayer nextPlayer;
    private bool gameIsOver = false;
    private System.Action legalMoveDone;
    
    public bool GameIsOver {
        get {
            return gameIsOver;
        }
    }

    public CheckerType CurrentCheckersTypeTurn {
        get {
            return currentPlayer.CheckersTypeColor;
        }
    }



    #region Unity Methods

    private void Awake() {
        rTransform = GetComponent<RectTransform>();
        matrixOfCells = new Cell[matrixSize,matrixSize];
        legalMoveDone = ChangeCurrentPlayer;
    }

    private void Start() {
        InitializePlayfieldWithCells();
        PlaceCheckersOnBoard();
        currentPlayer = players[new System.Random().Next(0, players.Count - 1)];
        notification.ShowNotification(currentPlayer.CheckersTypeColor.ToString() + ", you first");
    }
    
    #endregion

    #region Public Methods
        
    public void SetChosenChecker(int i, int j) { 
        if (chosenX == -1 || chosenY == -1) { // Reselection
            chosenX = i; chosenY = j;
            matrixOfCells[chosenX, chosenY].HandleCellSelection(true);
            return;
        }

        Cell chosenBefore = matrixOfCells[chosenX, chosenY];
        chosenBefore.HandleCellSelection(false);

        // CHECKER
        if (!chosenBefore.HaveMajorCheckerOn) {
            // Conditions as paramether in Handle.. functions determines move avaiability direction
            switch (chosenBefore.TypeOfCheckerOnMe) {
                case CheckerType.bot : { 
                    HandleVerticalMovement(chosenBefore, i, j, i < chosenX);
                    break;
                }
                case CheckerType.left: { 
                    HandleHorizontalMovement(chosenBefore, i, j, j > chosenY);
                    break;
                }
                case CheckerType.top: {
                    HandleVerticalMovement(chosenBefore, i, j, i > chosenX);
                    break;
                }
                case CheckerType.right: {
                    HandleHorizontalMovement(chosenBefore, i, j, j < chosenY);
                    break;
                }
            }
            HandleAttack(chosenBefore, i, j);
        }
        // MAJOR CHECKER
        else {
            HandleMajorCheckerMoves(chosenBefore, i, j);
        }
        chosenX = -1; chosenY = -1;
    }

    #endregion


    #region Private Methods

    private void InitializePlayfieldWithCells() {
        int k = 0;
        for (int i = 0; i < matrixSize; i++) {
            for (int j = 0; j < matrixSize; j++) {
                rTransform.GetChild(k).GetComponent<Cell>().InitializeCell(this, k%2 != 0, i, j); // Paint cells
                matrixOfCells[i, j] = rTransform.GetChild(k).GetComponent<Cell>(); // Fill the matrix with cells
                k++;
            }
        }
    }

    private void PlaceCheckersOnBoard() {
        for(int i = 0; i < 3; i++ ) {
            for (int j = i + 1; j < matrixSize - i; j += 2) {
                matrixOfCells[i, j].HandleCheckerOnMe(CheckerType.top);
                matrixOfCells[j, i].HandleCheckerOnMe(CheckerType.left);
                matrixOfCells[matrixSize - 1 - i, j].HandleCheckerOnMe(CheckerType.bot);
                matrixOfCells[j, matrixSize - 1 - i].HandleCheckerOnMe(CheckerType.right);
            }
        }

        players = new List<CheckersPlayer> { 
            new CheckersPlayer(CheckerType.top, singleColorCheckersAmount),   
            new CheckersPlayer(CheckerType.right, singleColorCheckersAmount),
            new CheckersPlayer(CheckerType.bot, singleColorCheckersAmount),
            new CheckersPlayer(CheckerType.left, singleColorCheckersAmount)
        };
    }


    /// <summary>
    /// newCelly != chosenCelly && newCellx != chosenCellx FOR MAJOR CHECKER 
    /// </summary>
    /// <param name="previousCell"></param>
    /// <param name="targetX"></param>
    /// <param name="targetY"></param>
    private void HandleVerticalMovement(Cell previousCell, int targetX, int targetY, bool vDirConstraint) {
        if (vDirConstraint && CheckerMoveConditionSatisfied(previousCell.HaveMajorCheckerOn, targetX, targetY)) {
            if (previousCell.HaveCheckerOn && !matrixOfCells[targetX, targetY].HaveCheckerOn) { // Check if chosen cell have checker on and new cell doesn't have yet
                ExchangeCells(previousCell, targetX, targetY, MajorityConditionPredicate(previousCell.TypeOfCheckerOnMe, targetX, targetY));
            }
        }
    }

    private void HandleHorizontalMovement(Cell previousCell, int targetX, int targetY, bool hDirConstraint) {
        if (hDirConstraint && CheckerMoveConditionSatisfied(previousCell.HaveMajorCheckerOn, targetX, targetY)) {
            if (previousCell.HaveCheckerOn && !matrixOfCells[targetX, targetY].HaveCheckerOn) { // Check if chosen cell have checker on and new cell doesn't have yet
                ExchangeCells(previousCell, targetX, targetY, MajorityConditionPredicate(previousCell.TypeOfCheckerOnMe, targetX, targetY));
            }
        }
    }

    private void ExchangeCells(Cell previousCell, int targetX, int targetY, bool majorityCondition, int victimX = -1, int victimY = -1) {
        CheckerType transitionCheckerBuffer = previousCell.TypeOfCheckerOnMe; // Remember checker type from previous position
        previousCell.HandleCheckerOnMe(transitionCheckerBuffer, false); // Remove checker from previous position (1st click)

        if (victimX != -1 && victimY != -1) { 
            matrixOfCells[victimX, victimY].HandleCheckerOnMe(matrixOfCells[victimX, victimY].TypeOfCheckerOnMe, false, false); // Just killed one checker!
            ReduceCheckerFromPlayer(matrixOfCells[victimX, victimY].TypeOfCheckerOnMe);
            PlaceCheckerOnCell(transitionCheckerBuffer, targetX, targetY, majorityCondition, false);

            if (!IsThereSomeMoreMovesAvaiable(targetX, targetY)) { // If there no more any legal moves - change player. Otherwise - give player a chance.
                legalMoveDone.Invoke();
            }
        } else {
            PlaceCheckerOnCell(transitionCheckerBuffer, targetX, targetY, majorityCondition, true);
        }
    }

    private void PlaceCheckerOnCell(CheckerType transitionChecker, int targetX, int targetY, bool majorityCondition, bool noVictim) {
        if (majorityCondition) {
            matrixOfCells[targetX,targetY].HaveMajorCheckerOn = true; 
            matrixOfCells[targetX,targetY].HandleCheckerOnMe(transitionChecker, true, true); // Place checker on new position (2nd click)
            if (!IsThereMoreMovesAvaiableForMajorChecker(targetX,targetY) && noVictim) legalMoveDone.Invoke();
        } else {
            matrixOfCells[targetX,targetY].HandleCheckerOnMe(transitionChecker); // Place checker on new position (2nd click)
            if (noVictim) legalMoveDone.Invoke();
        }
    }

    private void HandleAttack(Cell previousCell, int targetX, int targetY) {
        if (CheckerAttackConditionSatisfied(previousCell.TypeOfCheckerOnMe, targetX, targetY)) {
            if (previousCell.HaveCheckerOn && !matrixOfCells[targetX, targetY].HaveCheckerOn) { // Check if chosen cell have checker on and new cell doesn't have yet
                // Getting info about victim
                int x,y;
                x = targetX > chosenX ? targetX - 1 : targetX + 1;
                y = targetY > chosenY ? targetY - 1 : targetY + 1;
                // Check if there any checker between chosen pos and new pos and don't hit your troops
                if (matrixOfCells[x, y].HaveCheckerOn && matrixOfCells[x, y].TypeOfCheckerOnMe != previousCell.TypeOfCheckerOnMe) {
                    ExchangeCells(previousCell, targetX, targetY, MajorityConditionPredicate(previousCell.TypeOfCheckerOnMe, targetX, targetY), x, y);
                }
            }
        }
    }

    private bool MajorityConditionPredicate(CheckerType checkerType, int targetX, int targetY) {
        switch(checkerType) {
            case CheckerType.bot:
                return targetX == 0;
            case CheckerType.top: 
                return targetX == matrixSize - 1;
            case CheckerType.left:
                return targetY == matrixSize - 1;
            case CheckerType.right: 
                return targetY == 0;
            default: {
                Debug.LogError("Unsupported checker type");
                return false;
            }
        }
    }

    private void HandleMajorCheckerMoves(Cell previousCell, int targetX, int targetY) {
        if (previousCell.HaveMajorCheckerOn) {
            if (previousCell.HaveCheckerOn && !matrixOfCells[targetX, targetY].HaveCheckerOn && CheckerMoveConditionSatisfied(previousCell.HaveMajorCheckerOn, targetX, targetY)) { // Check if chosen cell have checker on and new cell doesn't have yet
                CheckMajorCheckerMoveType(previousCell, targetX, targetY);
            }
        }
    }

    private void CheckMajorCheckerMoveType(Cell majorCheckerCell, int x, int y) {
        int checkX = chosenX, checkY = chosenY;
        int countOfVictims = 0;
        Cell lastFoundCell = null;
        while (checkX != x || checkY != y) {
            checkX = x > chosenX ? checkX + 1 : checkX - 1;
            checkY = y > chosenY ? checkY + 1 : checkY - 1;
            
            
            // Count all potential -Enemy- victims between start pos and target pos.
            if (matrixOfCells[checkX, checkY].HaveCheckerOn) {
                lastFoundCell = matrixOfCells[checkX, checkY];
                countOfVictims++;
                if (countOfVictims > 1) { // U can't kill more then 1 enemy between chosen and target position. Only by couple moves
                    return;
                }
            }
        }

        if (countOfVictims == 0) {
            // Simple Movement of major checker;
            CheckerType transitionCheckerBuffer = majorCheckerCell.TypeOfCheckerOnMe; // Remember checker type from previous position
            majorCheckerCell.HandleCheckerOnMe(transitionCheckerBuffer, false, false); // Make cell empty and remove majority status from it
            matrixOfCells[x, y].HandleCheckerOnMe(transitionCheckerBuffer, true, true);
            legalMoveDone.Invoke();
        }

        if (countOfVictims == 1 && lastFoundCell.TypeOfCheckerOnMe != majorCheckerCell.TypeOfCheckerOnMe) {
            CheckerType transitionCheckerBuffer = majorCheckerCell.TypeOfCheckerOnMe; // Remember checker type from previous position
            majorCheckerCell.HandleCheckerOnMe(transitionCheckerBuffer, false, false); // Make cell empty and remove majority status from it

            lastFoundCell.HandleCheckerOnMe(lastFoundCell.TypeOfCheckerOnMe, false, false); // Killing the victim. Be sure that u killed any possible type of enemy checker (major also killable)
            ReduceCheckerFromPlayer(lastFoundCell.TypeOfCheckerOnMe);

            matrixOfCells[x, y].HandleCheckerOnMe(transitionCheckerBuffer, true, true);
            if (!IsThereMoreMovesAvaiableForMajorChecker(x, y)) {
                legalMoveDone.Invoke();
            }
        }
    }

    /// <summary>
    /// TODO: Разделить на две функции
    /// </summary>
    private void ChangeCurrentPlayer() {
        Debug.Log("PLayers Count: " + players.Count);
        if (players.Count == 1) {
            Debug.Log("game is over");
            notification.ShowNotification(players[0].CheckersTypeColor.ToString() + " player won!");
            gameIsOver = true;
        } else {
            currentPlayer = players[players.IndexOf(currentPlayer) == players.Count - 1 ? 0 : players.IndexOf(currentPlayer) + 1];
            if (currentPlayer.CheckersAmount <= 0) {
                players.Remove(currentPlayer);
                notification.ShowNotification(currentPlayer.CheckersTypeColor.ToString()+ " is eliminated.");
                ChangeCurrentPlayer();
                return;
            }
            notification.ShowNotification(currentPlayer.CheckersTypeColor.ToString() + ", your turn");
        }
    }



    private void ReduceCheckerFromPlayer(CheckerType checkerThatKilled) {
        foreach(CheckersPlayer player in players) {
            if (player.CheckersTypeColor == checkerThatKilled) {
                player.ReduceChecker();
                return;
            }
        }
    }

    private bool IsThereSomeMoreMovesAvaiable(int newX, int newY) {
        if ((newX - 2) is >= 0 and <= 10) {
            if ((newY - 2) is >= 0 and <= 10) {
                if (!matrixOfCells[newX - 2, newY - 2].HaveCheckerOn) {
                    if (matrixOfCells[newX - 1, newY - 1].HaveCheckerOn && matrixOfCells[newX - 1, newY - 1].TypeOfCheckerOnMe != CurrentCheckersTypeTurn) { 
                        return true;
                    }
                }
            }

            if ((newY + 2) is >= 0 and <= 10) {
                if (!matrixOfCells[newX - 2, newY + 2].HaveCheckerOn) {
                    if (matrixOfCells[newX - 1, newY + 1].HaveCheckerOn && matrixOfCells[newX - 1, newY + 1].TypeOfCheckerOnMe != CurrentCheckersTypeTurn) { 
                        return true;
                    }
                }
            }
        }

        if ((newX + 2) is >= 0 and <= 10) {
            if ((newY - 2) is >= 0 and <= 10) {
                if (!matrixOfCells[newX + 2, newY - 2].HaveCheckerOn) {
                    if (matrixOfCells[newX + 1, newY - 1].HaveCheckerOn && matrixOfCells[newX + 1, newY - 1].TypeOfCheckerOnMe != CurrentCheckersTypeTurn) {
                        return true;
                    }
                }
            }

            if ((newY + 2) is >= 0 and <= 10) {
                if (!matrixOfCells[newX + 2, newY + 2].HaveCheckerOn) {
                    if (matrixOfCells[newX + 1, newY + 1].HaveCheckerOn && matrixOfCells[newX + 1, newY + 1].TypeOfCheckerOnMe != CurrentCheckersTypeTurn) {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool IsThereMoreMovesAvaiableForMajorChecker(int newX, int newY) {
        if (newX > 1) { // TOP DIRECTIONS
            for(int i = newX - 1, columnMargin = 1; i > 0; i--, columnMargin++) {
                
                // TOP LEFT WAY
                if (newY - columnMargin > 0) {
                    if (matrixOfCells[i, newY - columnMargin].HaveCheckerOn) { // Finding any checker on current major checker way
                        Debug.Log("SOME ON MY TOP LEFT WAY");
                        if (matrixOfCells[i, newY - columnMargin].IsEnemyForCurrent(CurrentCheckersTypeTurn)) { // Check is it enemy checker?
                            Debug.Log("IT'S ENEMY!TL");
                            if (!matrixOfCells[i - 1, newY - columnMargin - 1].HaveCheckerOn) { // Is there empty cell behind enemy ?
                                Debug.Log("I CAN KILL HIM!TL");
                                return true;
                            }
                        }
                    }
                } 

                // TOP RIGHT WAY
                if (newY + columnMargin <= matrixSize - 2) {
                    if (matrixOfCells[i, newY + columnMargin].HaveCheckerOn) { // Finding any checker on current major checker way
                        Debug.Log("SOME ON MY TOP RIGHT WAY");
                        if (matrixOfCells[i, newY + columnMargin].IsEnemyForCurrent(CurrentCheckersTypeTurn)) { // Check is it enemy checker?
                            Debug.Log("IT'S ENEMY!TR");
                            if (!matrixOfCells[i - 1, newY + columnMargin + 1].HaveCheckerOn) { // Is there empty cell behind enemy ?
                                Debug.Log("I CAN KILL HIM!TR");
                                return true;
                            }
                        }
                    }
                }
            } 
        }

        if (newX < matrixSize - 2) { // BOT DIRECTIONS
            for(int i = newX + 1, columnMargin = 1; i < matrixSize - 1; i++, columnMargin++) {
                // BOT LEFT WAY
                if (newY - columnMargin > 0) {
                    if (matrixOfCells[i, newY - columnMargin].HaveCheckerOn) { // Finding any checker on current major checker way
                        Debug.Log("SOME ON MY BOT LEFT WAY");
                        if (matrixOfCells[i, newY - columnMargin].IsEnemyForCurrent(CurrentCheckersTypeTurn)) { // Check is it enemy checker?
                            Debug.Log("IT'S ENEMY!BL");
                            if (!matrixOfCells[i + 1, newY - columnMargin - 1].HaveCheckerOn) { // Is there empty cell behind enemy ?
                                Debug.Log("I CAN KILL HIM!BL");
                                return true;
                            }
                        }
                    }
                }

                // BOT RIGHT WAY
                if (newY + columnMargin <= matrixSize - 2) {
                    if (matrixOfCells[i, newY + columnMargin].HaveCheckerOn) { // Finding any checker on current major checker way
                        Debug.Log("SOME ON MY BOT RIGHT WAY");
                        if (matrixOfCells[i, newY + columnMargin].IsEnemyForCurrent(CurrentCheckersTypeTurn)) { // Check is it enemy checker?
                            Debug.Log("IT'S ENEMY!BR");
                            if (!matrixOfCells[i + 1, newY + columnMargin + 1].HaveCheckerOn) { // Is there empty cell behind enemy ?
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

    /// <summary>
    /// Diagonal movement condition
    /// </summary>
    /// <param name="targetX"></param>
    /// <param name="targetY"></param>
    /// <returns></returns>
    private bool CheckerMoveConditionSatisfied(bool isMajorChecker, int targetX, int targetY) {
        if (!isMajorChecker) 
            return 
                (targetX != chosenX && targetY != chosenY) && 
                (Mathf.Abs(targetX - chosenX) == 1 && Mathf.Abs(targetY - chosenY) == 1);
        else 
            return 
                (targetX != chosenX && targetY != chosenY) && 
                (Mathf.Abs(targetX - chosenX) == Mathf.Abs(targetY - chosenY));
    }

    private bool CheckerAttackConditionSatisfied(CheckerType attackingCheckerType, int targetX, int targetY) {
        if (attackingCheckerType is CheckerType.bot or CheckerType.top) {
            return 
                (Mathf.Abs(targetX - chosenX) == 2) &&
                (targetX != chosenX && targetY != chosenY);
        }

        if (attackingCheckerType is CheckerType.right or CheckerType.left) { 
            return 
                (Mathf.Abs(targetY - chosenY) == 2) &&
                (targetX != chosenX && targetY != chosenY);
        }

        Debug.LogError("Checker attack condition is unsatisfied. Something wrong with attackingCheckerType.");
        return false;
    }

    #endregion
}
