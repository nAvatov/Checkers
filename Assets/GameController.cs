using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CheckerType {
    red,
    green,
    blue,
    pink
}

public class GameController : MonoBehaviour
{
    private Cell[,] matrixOfCells;
    private int matrixSize = 11;
    private RectTransform rTransform;

    private int chosenX = -1, chosenY = -1;


    #region Unity Methods

    private void Awake() {
        rTransform = GetComponent<RectTransform>();
        matrixOfCells = new Cell[matrixSize,matrixSize];
    }

    private void Start() {
        InitializePlayfieldWithCells();
        PlaceCheckersOnBoard();
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
            HandleAttack(chosenBefore, i, j);
            switch (chosenBefore.TypeOfCheckerOnMe) {
                case CheckerType.red : { 
                    HandleVerticalMovement(chosenBefore, i, j, i < chosenX);
                    break;
                }
                case CheckerType.green: { 
                    HandleHorizontalMovement(chosenBefore, i, j, j > chosenY);
                    break;
                }
                case CheckerType.blue: {
                    HandleVerticalMovement(chosenBefore, i, j, i > chosenX);
                    break;
                }
                case CheckerType.pink: {
                    HandleHorizontalMovement(chosenBefore, i, j, j < chosenY);
                    break;
                }
            }
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
                matrixOfCells[i, j].HandleCheckerOnMe(CheckerType.blue);
                matrixOfCells[j, i].HandleCheckerOnMe(CheckerType.green);
                matrixOfCells[matrixSize - 1 - i, j].HandleCheckerOnMe(CheckerType.red);
                matrixOfCells[j, matrixSize - 1 - i].HandleCheckerOnMe(CheckerType.pink);
            }
        }
    }


    /// <summary>
    /// newCelly != chosenCelly && newCellx != chosenCellx FOR MAJOR CHECKER 
    /// </summary>
    /// <param name="previousCell"></param>
    /// <param name="targetX"></param>
    /// <param name="targetY"></param>
    private void HandleVerticalMovement(Cell previousCell, int targetX, int targetY, bool vDirConstraint) {
        if (vDirConstraint && Mathf.Abs(targetY - chosenY) == 1) {
            if (previousCell.HaveCheckerOn && !matrixOfCells[targetX, targetY].HaveCheckerOn) { // Check if chosen cell have checker on and new cell doesn't have yet
                ExchangeCells(previousCell, targetX, targetY, targetX == 0 || targetX == matrixSize - 1);
            }
        }
    }

    private void HandleHorizontalMovement(Cell previousCell, int targetX, int targetY, bool hDirConstraint) {
        if (hDirConstraint && Mathf.Abs(targetY - chosenY) == 1) {
            if (previousCell.HaveCheckerOn && !matrixOfCells[targetX, targetY].HaveCheckerOn) { // Check if chosen cell have checker on and new cell doesn't have yet
                ExchangeCells(previousCell, targetX, targetY, targetY == 0 || targetY == matrixSize - 1);
            }
        }
    }

    private void ExchangeCells(Cell previousCell, int targetX, int targetY, bool majorityCondition, int victimX = -1, int victimY = -1) {
        CheckerType transitionCheckerBuffer = previousCell.TypeOfCheckerOnMe; // Remember checker type from previous position
        previousCell.HandleCheckerOnMe(transitionCheckerBuffer, false); // Remove checker from previous position (1st click)

        if (victimX != -1 && victimY != -1) { 
            matrixOfCells[victimX, victimY].HandleCheckerOnMe(matrixOfCells[victimX, victimY].TypeOfCheckerOnMe, false, false); // Just killed one checker!
            Debug.Log("ORDINARY CHECKER KILLED SOMEONE");
        }

        if (majorityCondition) {
            matrixOfCells[targetX,targetY].HaveMajorCheckerOn = true; 
            matrixOfCells[targetX,targetY].HandleCheckerOnMe(transitionCheckerBuffer, true, true); // Place checker on new position (2nd click)
            Debug.Log("ORDINARY CHECKER BECAME MAJOR");
        }
        else {
            matrixOfCells[targetX,targetY].HandleCheckerOnMe(transitionCheckerBuffer); // Place checker on new position (2nd click)
            Debug.Log("ORDINARY CHECKER MOVED");
        }
    }

    private void HandleAttack(Cell previousCell, int targetX, int targetY) {
        if (Mathf.Abs(targetY - chosenY) == 2) {
            if (previousCell.HaveCheckerOn && !matrixOfCells[targetX, targetY].HaveCheckerOn) { // Check if chosen cell have checker on and new cell doesn't have yet
                // Getting info about victim
                int x,y;
                x = targetX > chosenX ? targetX - 1 : targetX + 1;
                y = targetY > chosenY ? targetY - 1 : targetY + 1;
                // Check if there any checker between chosen pos and new pos and don't hit your troops
                if (matrixOfCells[x, y].HaveCheckerOn && matrixOfCells[x, y].TypeOfCheckerOnMe != previousCell.TypeOfCheckerOnMe) {
                    ExchangeCells(previousCell, targetX, targetY, CheckOrientationPredicate(previousCell.TypeOfCheckerOnMe, targetX, targetY), x, y); // TODO: Make majority handle after killing enemy checker
                }
            }
        }
    }

    private bool CheckOrientationPredicate(CheckerType checkerType, int targetX, int targetY) {
        if (checkerType == CheckerType.red || checkerType == CheckerType.blue) { 
            return targetX == 0 || targetX == matrixSize - 1;
        }
        return targetY == 0 || targetY == matrixSize - 1;
    }

    private void HandleMajorCheckerMoves(Cell previousCell, int targetX, int targetY) {
        if (previousCell.HaveMajorCheckerOn) {
            if (previousCell.HaveCheckerOn && !matrixOfCells[targetX, targetY].HaveCheckerOn) { // Check if chosen cell have checker on and new cell doesn't have yet
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

        }

        if (countOfVictims == 1 && lastFoundCell.TypeOfCheckerOnMe != majorCheckerCell.TypeOfCheckerOnMe) {
            CheckerType transitionCheckerBuffer = majorCheckerCell.TypeOfCheckerOnMe; // Remember checker type from previous position
            majorCheckerCell.HandleCheckerOnMe(transitionCheckerBuffer, false, false); // Make cell empty and remove majority status from it

            lastFoundCell.HandleCheckerOnMe(lastFoundCell.TypeOfCheckerOnMe, false, false); // Killing the victim. Be sure that u killed any possible type of enemy checker (major also killable)

            matrixOfCells[x, y].HandleCheckerOnMe(transitionCheckerBuffer, true, true);
            Debug.Log("MAJOR KILLED SOMEONE");
        }
    }
    #endregion
}
