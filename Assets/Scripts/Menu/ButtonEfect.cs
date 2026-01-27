using UnityEngine;
using DG.Tweening;

public class ButtonEfect : MonoBehaviour
{
    public RectTransform ripple;
    public float duration = 0.5f;

    public void PlayRipple()
    {
        ripple.localScale = Vector3.zero;
        ripple.gameObject.SetActive(true);

        ripple.DOScale(2f, duration);
        ripple.GetComponent<UnityEngine.UI.Image>()
            .DOFade(0f, duration)
            .OnComplete(() =>
            {
                ripple.gameObject.SetActive(false);
            });
    }
}