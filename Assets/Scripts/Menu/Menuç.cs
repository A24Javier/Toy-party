using DG.Tweening;
using UnityEngine;

public class Logo : MonoBehaviour
{
    [Header("Movimiento Logo")]
    [SerializeField] private float posicionFinalY = 200f;
    [SerializeField] private float duracionMov = 1.5f;
    [SerializeField] private float reboteH = 30f;
    [SerializeField] private float duracionReb = 0.15f;
    [SerializeField] private float duracionSegundoMov = 0.2f;
    [SerializeField] private RectTransform logo;

    [Header("Movimiento Cortinas")]
    [SerializeField] private float posicionFinalCor = 1000f;
    [SerializeField] private float duracionMovCor = 2.5f;
    [SerializeField] private RectTransform cor1;
    [SerializeField] private RectTransform cor2;
    [SerializeField] private RectTransform corFondo;


    public void MoveCortinas()
    {
        cor1.DOKill();
        cor2.DOKill();
        corFondo.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Append(cor1.DOAnchorPosX(-posicionFinalCor, duracionMovCor).SetEase(Ease.InQuad));

        seq.Join(cor2.DOAnchorPosX(posicionFinalCor, duracionMovCor).SetEase(Ease.InQuad));

        seq.Join(corFondo.DOAnchorPosY(posicionFinalCor, duracionMovCor).SetEase(Ease.InQuad));

        seq.OnComplete(() =>
        {
            Move();
        });


    }
    public void Move()
    {
        logo.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Append(logo.DOAnchorPosY(posicionFinalY, duracionMov).SetEase(Ease.InQuad));

        seq.Append(logo.DOAnchorPosY(posicionFinalY + reboteH, duracionReb).SetEase(Ease.OutQuad));

        seq.Append(logo.DOAnchorPosY(posicionFinalY, duracionSegundoMov).SetEase(Ease.InQuad));
    }
}
