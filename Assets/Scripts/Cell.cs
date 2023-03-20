using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public struct Position {
    public int x;
    public int y;

    public Position (int _x, int _y) {
        x = _x;
        y = _y;
    }
}

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

    private Position _position;
    private bool _hasCheckerOn;
    private bool _hasMajorCheckerOn;
    private bool _isActive;
    private CheckerType _typeOfCheckerOn;

    public bool HaveCheckerOn {
        get {
            return _hasCheckerOn;
        }

        set {
            _hasCheckerOn = value;
        }
    }

    public bool HaveMajorCheckerOn { 
        get {
            return _hasMajorCheckerOn;
        }

        set {
            _hasMajorCheckerOn = value;
        }
    }

    public bool IsActiveCell {
        get {
            return _isActive;
        }
    }

    public CheckerType TypeOfCheckerOn {
        get {
            return _typeOfCheckerOn;
        }
    }

    #region UnityMethods

    public void OnPointerClick(PointerEventData pointerEventData) {
        if (_isActive && !PlayersController.IsGameOver) {
            if ((_hasCheckerOn && _typeOfCheckerOn == PlayersController.CurrentCheckersTypeTurn) || !_hasCheckerOn) {
                MovementController.SetChosenChecker(_position);
            }
        }
    }
        
    #endregion

    #region Public Methods

    public void InitializeCell(bool isActiveCell, Position newPosition) {
       image.color = isActiveCell ? activeColor : inactiveColor;
       _isActive = isActiveCell;
       _position = newPosition;
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

        _hasCheckerOn = isPlacing;
        _hasMajorCheckerOn = isMajorChecker;
        majorityState.SetActive(isMajorChecker);

        if (isPlacing) {
            _typeOfCheckerOn = checker;
        }
    }

    public void AddCheckerToCell(CheckerType checkerType) {
        HandleCheckerOnMe(checkerType);
        PlayersController.AddCheckerToPlayer(checkerType);
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
        return _hasCheckerOn && (currentPlayerCheckerType != _typeOfCheckerOn);
    }
    

    #endregion
}
