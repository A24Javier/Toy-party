using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

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

    [Header("Movimiento Boton")]
    [SerializeField] private RectTransform conf;
    [SerializeField] private RectTransform play;
    [SerializeField] private RectTransform exit;
    [SerializeField] private RectTransform credito;

    [SerializeField] private float posicionFinalConfig = -223f;
    [SerializeField] private float posicionFinalpe = -223f;
    [SerializeField] private float posicionFinalCreditoY = -54f;

    [Header("Evento")]
    public UnityEvent OnMoveFinished;




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

        seq.Join(conf.DOAnchorPosY(posicionFinalConfig, duracionMov).SetEase(Ease.InQuad));

        seq.Join(play.DOAnchorPosY(posicionFinalpe, duracionMov).SetEase(Ease.InQuad));

        seq.Join(exit.DOAnchorPosY(posicionFinalpe, duracionMov).SetEase(Ease.InQuad));

        seq.Join(credito.DOAnchorPosY(posicionFinalpe, duracionMov).SetEase(Ease.InQuad));

        seq.OnComplete(() =>
        {
            OnMoveFinished?.Invoke();
        });

    }
}
