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

        // SIMPLE CHECKER ATTACK
        HandleAttack(chosenBefore, i, j);

        // CHECKER MOVEMENT
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
        if ( vDirConstraint && Mathf.Abs(targetY - chosenY) == 1 ) {
            // Move
            if (previousCell.HaveCheckerOn && !matrixOfCells[targetX, targetY].HaveCheckerOn) { // Check if chosen cell have checker on and new cell doesn't have yet
                CheckerType transitionCheckerBuffer = previousCell.TypeOfCheckerOnMe; // Remember checker type from previous position
                previousCell.HandleCheckerOnMe(transitionCheckerBuffer, false); // Remove checker from previous position (1st click)
                matrixOfCells[targetX,targetY].HandleCheckerOnMe(transitionCheckerBuffer); // Place checker on new position (2nd click)
            }
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
                    CheckerType transitionCheckerBuffer = previousCell.TypeOfCheckerOnMe; // Remember checker type from previous position
                    previousCell.HandleCheckerOnMe(transitionCheckerBuffer, false); // Remove checker from previous position (1st click)

                    matrixOfCells[x, y].HandleCheckerOnMe(matrixOfCells[x, y].TypeOfCheckerOnMe, false); // Just killed one checker!

                    matrixOfCells[targetX,targetY].HandleCheckerOnMe(transitionCheckerBuffer); // Place checker on new position (2nd click)
                }
            }
        }
    }

    private void HandleHorizontalMovement(Cell previousCell, int targetX, int targetY, bool hDirConstraint) {
        if ( hDirConstraint && Mathf.Abs(targetY - chosenY) == 1 ) {
            // Move
            if (previousCell.HaveCheckerOn && !matrixOfCells[targetX, targetY].HaveCheckerOn) { // Check if chosen cell have checker on and new cell doesn't have yet
                CheckerType transitionCheckerBuffer = previousCell.TypeOfCheckerOnMe; // Remember checker type from previous position
                previousCell.HandleCheckerOnMe(transitionCheckerBuffer, false); // Remove checker from previous position (1st click)
                matrixOfCells[targetX,targetY].HandleCheckerOnMe(transitionCheckerBuffer); // Place checker on new position (2nd click)
            }
        }
    }






    #endregion
}
