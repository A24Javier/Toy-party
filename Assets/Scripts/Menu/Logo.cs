using DG.Tweening;
using UnityEngine;

public class Logo : MonoBehaviour
{
    [Header("Movimiento principal")]
    [SerializeField] private float posicionFinalY = 200f;
    [SerializeField] private float duracionMov = 1.5f;

    [Header("Rebote")]
    [SerializeField] private float reboteH = 30f;
    [SerializeField] private float duracionReb = 0.15f;
    [SerializeField] private float duracionSegundoMov = 0.2f;

    private RectTransform rt;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    void Start()
    {
        Move();
    }

    public void Move()
    {
        rt.DOKill(); 

        Sequence seq = DOTween.Sequence();

        seq.Append(rt.DOAnchorPosY(posicionFinalY, duracionMov).SetEase(Ease.InQuad));

        seq.Append(rt.DOAnchorPosY(posicionFinalY + reboteH, duracionReb).SetEase(Ease.OutQuad));

        seq.Append(rt.DOAnchorPosY(posicionFinalY, duracionSegundoMov).SetEase(Ease.InQuad));
    }
}
