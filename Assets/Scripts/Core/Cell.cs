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
    [SerializeField] private UnityEngine.UI.Image _image;
    [SerializeField] private GameObject _selectionPaddingObj;
    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _inactiveColor;
    [Header("Checkers")]
    [SerializeField] private GameObject _leftChecker;
    [SerializeField] private GameObject _botChecker;
    [SerializeField] private GameObject _rightChecker;
    [SerializeField] private GameObject _topChecker;
    [SerializeField] private GameObject _majorityState;

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

    #region Unity Methods

    public void OnPointerClick(PointerEventData pointerEventData) {
        if (_isActive && !PlayersController.IsGameOver) {
            if ((_hasCheckerOn && _typeOfCheckerOn == PlayersController.CurrentCheckersTypeTurn) || !_hasCheckerOn) {
                MovementController.SetChosenChecker(_position);
                if (!ButtonsController.IsHidden) {
                    ButtonsController.HandleButtonsPanelWidth();
                }
            }
        }
    }
        
    #endregion

    #region Public Methods

    public void InitializeCell(bool isActiveCell, BoardPosition newPosition) {
       _image.color = isActiveCell ? _activeColor : _inactiveColor;
       _isActive = isActiveCell;
       _position = newPosition;
    }

    /// <summary>
    /// Core method used to handle checker appearence on cell.
    /// </summary>
    public void HandleCheckerOnMe(CheckerType checker, bool isPlacing = true, bool isMajorChecker = false) {
        switch(checker) {
            case CheckerType.bot: {
                _botChecker.SetActive(isPlacing);
                break;
            }
            case CheckerType.left: {
                _leftChecker.SetActive(isPlacing);
                break;
            }
            case CheckerType.top: {
                _topChecker.SetActive(isPlacing);
                break;
            }
            case CheckerType.right: {
                _rightChecker.SetActive(isPlacing);
                break;
            }
            case CheckerType.noType: {
                RemoveAnyCheckerFromMe();
                break;
            }
        }

        _hasCheckerOn = isPlacing;
        _hasMajorCheckerOn = isMajorChecker;
        _majorityState.SetActive(isMajorChecker);

        _typeOfCheckerOn = isPlacing ? checker : CheckerType.noType;
    }

    /// <summary>
    /// Method for adding new checker when board initializes.
    /// </summary>
    /// <param name="checkerType"></param>
    public void AddCheckerToCell(CheckerType checkerType) {
        HandleCheckerOnMe(checkerType);
        PlayersController.AddCheckerToPlayer(checkerType);
    }

    public void HandleCellSelection(bool isSelected) {
        _selectionPaddingObj.SetActive(isSelected);
    }

    public bool IsEnemyForCurrent(CheckerType currentPlayerCheckerType) {
        return _hasCheckerOn && (currentPlayerCheckerType != _typeOfCheckerOn);
    }

    #endregion

    private void RemoveAnyCheckerFromMe() {
        _botChecker.SetActive(false);
        _topChecker.SetActive(false);
        _leftChecker.SetActive(false);
        _rightChecker.SetActive(false);
    }

}
