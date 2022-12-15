using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    [Header("Appearence on board")]
    [SerializeField] UnityEngine.UI.Image image;
    [SerializeField] GameObject selectionPaddingObj;
    [SerializeField] Color activeColor;
    [SerializeField] Color inactiveColor;
    [Header("Checkers")]
    [SerializeField] GameObject greenCheker, redChecker, pinkChecker, blueChekcer;


    private GameController gameController;
    private int coordX, coordY;
    private bool haveCheckerOn;
    private bool active;
    private CheckerType typeOfCheckerOnMe;

    public bool HaveCheckerOn {
        get {
            return haveCheckerOn;
        }

        set {
            haveCheckerOn = value;
        }
    }

    public bool IsActiveCell {
        get {
            return active;
        }
    }

    public CheckerType TypeOfCheckerOnMe {
        get {
            return typeOfCheckerOnMe;
        }
    }

    #region UnityMethods

    public void OnPointerClick(PointerEventData pointerEventData) {
        if (active) {
            gameController.SetChosenChecker(coordX, coordY);
            //HandleCellSelection(true);
        }
    }
        
    #endregion

    #region Public Methods

    public void InitializeCell(GameController gc, bool isActiveCell, int i, int j) {
       image.color = isActiveCell ? activeColor : inactiveColor;
       active = isActiveCell;
       coordX = i; coordY = j;
       gameController = gc;
    }

    public void HandleCheckerOnMe(CheckerType checker, bool isPlacing = true) {
        switch(checker) {
            case CheckerType.red: {
                redChecker.SetActive(isPlacing);
                break;
            }
            case CheckerType.green: {
                greenCheker.SetActive(isPlacing);
                break;
            }
            case CheckerType.blue: {
                blueChekcer.SetActive(isPlacing);
                break;
            }
            case CheckerType.pink: {
                pinkChecker.SetActive(isPlacing);
                break;
            }
        }

        haveCheckerOn = isPlacing;
        if (isPlacing) {
            typeOfCheckerOnMe = checker;
        }
    }

    public void RemoveAnyCheckerFromMe() {
        foreach(GameObject checker in new List<GameObject> { redChecker, blueChekcer, greenCheker, pinkChecker}) {
            checker.SetActive(false);
        }
    }

    public void HandleCellSelection(bool isSelected) {
        selectionPaddingObj.SetActive(isSelected);
    }

    

    #endregion
}
