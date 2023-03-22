using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public struct BoardPosition {
    public int x;
    public int y;

    public BoardPosition (int _x, int _y) {
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

    private BoardPosition _position;
    private bool _hasCheckerOn;
    private bool _hasMajorCheckerOn;
    private bool _isActive;
    private CheckerType _typeOfCheckerOn = CheckerType.noType;

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

    public BoardPosition Position {
        get {
            return _position;
        }
    }

    #region UnityMethods

    public void OnPointerClick(PointerEventData pointerEventData) {
        Debug.Log(TypeOfCheckerOn);
        if (_isActive && !PlayersController.IsGameOver) {
            if ((_hasCheckerOn && _typeOfCheckerOn == PlayersController.CurrentCheckersTypeTurn) || !_hasCheckerOn) {
                Debug.Log("all ok");
                MovementController.SetChosenChecker(_position);
            }
        }
    }
        
    #endregion

    #region Public Methods

    public void InitializeCell(bool isActiveCell, BoardPosition newPosition) {
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
            case CheckerType.noType: {
                RemoveAnyCheckerFromMe();
                break;
            }
        }

        _hasCheckerOn = isPlacing;
        _hasMajorCheckerOn = isMajorChecker;
        majorityState.SetActive(isMajorChecker);

        _typeOfCheckerOn = isPlacing ? checker : CheckerType.noType;
    }

    public void AddCheckerToCell(CheckerType checkerType) {
        HandleCheckerOnMe(checkerType);
        PlayersController.AddCheckerToPlayer(checkerType);
    }

    public void RemoveAnyCheckerFromMe() {
        botChecker.SetActive(false);
        topChecker.SetActive(false);
        leftChecker.SetActive(false);
        rightChecker.SetActive(false);
    }

    public void HandleCellSelection(bool isSelected) {
        selectionPaddingObj.SetActive(isSelected);
    }

    public bool IsEnemyForCurrent(CheckerType currentPlayerCheckerType) {
        return _hasCheckerOn && (currentPlayerCheckerType != _typeOfCheckerOn);
    }
    

    #endregion
}
