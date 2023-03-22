using DG.Tweening;
using UnityEngine;

public class ButtonsController : MonoBehaviour
{
    [SerializeField] private RectTransform _buttonsHolder;
    private float _revealedWidth = 550f;

    public void HandleButtonsPanelWidth() {
        _buttonsHolder.DOSizeDelta(new Vector2(Mathf.Abs(_buttonsHolder.sizeDelta.x - _revealedWidth), _buttonsHolder.sizeDelta.y), 0.3f);
    }
}
