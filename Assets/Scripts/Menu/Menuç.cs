using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class Logo : MonoBehaviour
{
    [Header("Movimiento Cortinas")]
    [SerializeField] private float posicionFinalCor = 1000f;
    [SerializeField] private float duracionMovCor = 2.5f;
    [SerializeField] private RectTransform cor1;
    [SerializeField] private RectTransform cor2;
    [SerializeField] private RectTransform corFondo;

    [Header("Evento")]
    public UnityEvent OnMoveFinished;

    public void MoveCortinas()
    {
        if (cor1 != null) cor1.DOKill();
        if (cor2 != null) cor2.DOKill();
        if (corFondo != null) corFondo.DOKill();

        Sequence seq = DOTween.Sequence();

        if (cor1 != null)
            seq.Append(cor1.DOAnchorPosX(-posicionFinalCor, duracionMovCor).SetEase(Ease.InQuad));

        if (cor2 != null)
            seq.Join(cor2.DOAnchorPosX(posicionFinalCor, duracionMovCor).SetEase(Ease.InQuad));

        if (corFondo != null)
            seq.Join(corFondo.DOAnchorPosY(posicionFinalCor, duracionMovCor).SetEase(Ease.InQuad));

        seq.OnComplete(() =>
        {
            OnMoveFinished?.Invoke();
        });
    }
}