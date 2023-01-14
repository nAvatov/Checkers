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
    [SerializeField] GameObject leftChecker;
    [SerializeField] GameObject botChecker;
    [SerializeField] GameObject rightChecker;
    [SerializeField] GameObject topChecker;
    [SerializeField] GameObject majorityState;



    private GameController gameController;
    private int coordX, coordY;
    private bool haveCheckerOn;
    private bool haveMajorCheckerOn;
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

    public bool HaveMajorCheckerOn { 
        get {
            return haveMajorCheckerOn;
        }

        set {
            haveMajorCheckerOn = value;
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
        if (active && !gameController.GameIsOver) {
            if ((haveCheckerOn && typeOfCheckerOnMe == gameController.CurrentCheckersTypeTurn) || !haveCheckerOn)
            {
                gameController.SetChosenChecker(coordX, coordY);
            }
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

    public void HandleCheckerOnMe(CheckerType checker, bool isPlacing = true, bool isMajorChecker = false) {
        switch(checker) {
            case CheckerType.bot: {
                botChecker.SetActive(isPlacing);
                break;
            }
            case CheckerType.left: {
                leftChecker.SetActive(isPlacing);
                break;
            }
            case CheckerType.top: {
                topChecker.SetActive(isPlacing);
                break;
            }
            case CheckerType.right: {
                rightChecker.SetActive(isPlacing);
                break;
            }
        }

        haveCheckerOn = isPlacing;
        haveMajorCheckerOn = isMajorChecker;
        majorityState.SetActive(isMajorChecker);

        if (isPlacing) {
            typeOfCheckerOnMe = checker;
        }
    }

    public void RemoveAnyCheckerFromMe() {
        foreach(GameObject checker in new List<GameObject> { botChecker, topChecker, leftChecker, rightChecker}) {
            checker.SetActive(false);
        }
    }

    public void HandleCellSelection(bool isSelected) {
        selectionPaddingObj.SetActive(isSelected);
    }

    public bool IsEnemyForCurrent(CheckerType currentPlayerCheckerType) {
        return haveCheckerOn && (currentPlayerCheckerType != typeOfCheckerOnMe);
    }
    

    #endregion
}
