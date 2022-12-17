using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notification : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI textTMP;
    [SerializeField] CanvasGroup cg;

    public void ShowNotification(string text) {
        StopAllCoroutines();
        textTMP.SetText(text);
        StartCoroutine(RevealNotification());
    }

    public IEnumerator RevealNotification() {
        while(cg.alpha < 1) {
            cg.alpha += 0.01f;
            yield return new WaitForSecondsRealtime(0.01f);
        }

        cg.interactable = true;
        cg.blocksRaycasts = true;
        StartCoroutine(HideNotification());
    }

    private IEnumerator HideNotification() {
        cg.interactable = false;
        cg.blocksRaycasts = false;

        while (cg.alpha > 0) {
            cg.alpha -= 0.01f;
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
}
