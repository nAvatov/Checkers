using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Notification : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI textTMP;
    [SerializeField] CanvasGroup cg;
    [SerializeField] ButtonsController buttonsController;

    public void ShowNotification(string text) {
        StopAllCoroutines();
        textTMP.SetText(text);
        cg.DOFade(1, 1f).OnComplete(() => {
            cg.interactable = true;
            cg.blocksRaycasts = true;
            cg.DOFade(0, 1f).OnComplete(() => {
                cg.interactable = false;
                cg.blocksRaycasts = false;

                if (PlayersController.IsGameOver) {
                    ButtonsController.HandleButtonsPanelWidth();
                }
            });
        });
    }
}
