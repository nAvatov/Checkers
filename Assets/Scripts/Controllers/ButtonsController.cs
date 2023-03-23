using DG.Tweening;
using UnityEngine;

public class ButtonsController : MonoBehaviour
{
    private static RectTransform _buttonsHolder;
    private static float _revealedWidth = 550f;
    private static bool _isHidden = false;

    public static bool IsHidden {
        get {
            return _isHidden;
        }
    }

    private void Awake() {
        _buttonsHolder = gameObject.GetComponent<RectTransform>();
    }

    public static void HandleButtonsPanelWidth() {
        _buttonsHolder.DOSizeDelta(new Vector2(Mathf.Abs(_buttonsHolder.sizeDelta.x - _revealedWidth), _buttonsHolder.sizeDelta.y), 0.3f);
        _isHidden = !_isHidden;
    }
}
