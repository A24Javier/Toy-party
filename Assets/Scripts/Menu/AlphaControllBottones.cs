using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class AlphaControllBottones : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private Image imagen;
    [SerializeField] private float duration = 0.1f;

    private Coroutine fadeRoutine;

    public void OnPointerEnter(PointerEventData eventData)
    {
        FadeTo(1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        FadeTo(0f);
    }

    public void OnSelect(BaseEventData eventData)
    {
        FadeTo(1f);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        FadeTo(0f);
    }

    private void FadeTo(float targetAlpha)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeAlpha(targetAlpha));
    }

    private IEnumerator FadeAlpha(float targetAlpha)
    {
        Color color = imagen.color;
        float startAlpha = color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);

            color.a = alpha;
            imagen.color = color;

            yield return null;
        }

        color.a = targetAlpha;
        imagen.color = color;
    }
}

