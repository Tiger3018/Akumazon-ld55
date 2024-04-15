using System.Collections;
using System.Linq.Expressions;
using UnityEngine;

public class UIAnimation
{
    public static IEnumerator FadeAlpha(CanvasGroup canvasGroup, float from, float to, float fadeTime)
    {
        float time = 0;
        while (time < fadeTime)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, time / fadeTime);
            time += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = to;
    }
    public static IEnumerator WaitSecondsThenClear(float seconds, TMPro.TextMeshProUGUI textMeshProUGUI)
    {
        yield return new WaitForSeconds(seconds);
        textMeshProUGUI.text = "";
    }
}
