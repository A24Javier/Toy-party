using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class Logo : MonoBehaviour
{
    [System.Serializable]
    public class BotonAnimado
    {
        public RectTransform boton;
        public float posicionFinalY;
    }

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

    [Header("Botones")]
    [SerializeField] private List<BotonAnimado> botones = new List<BotonAnimado>();

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
            Move();
        });
    }

    public void Move()
    {
        if (logo != null)
            logo.DOKill();

        for (int i = 0; i < botones.Count; i++)
        {
            if (botones[i].boton != null)
                botones[i].boton.DOKill();
        }

        Sequence seq = DOTween.Sequence();

        if (logo != null)
        {
            seq.Append(logo.DOAnchorPosY(posicionFinalY, duracionMov).SetEase(Ease.InQuad));
            seq.Append(logo.DOAnchorPosY(posicionFinalY + reboteH, duracionReb).SetEase(Ease.OutQuad));
            seq.Append(logo.DOAnchorPosY(posicionFinalY, duracionSegundoMov).SetEase(Ease.InQuad));
        }

        for (int i = 0; i < botones.Count; i++)
        {
            BotonAnimado botonActual = botones[i];

            if (botonActual.boton == null)
                continue;

            seq.Join(
                botonActual.boton.DOAnchorPosY(botonActual.posicionFinalY, duracionMov).SetEase(Ease.InQuad)
            );
        }

        seq.OnComplete(() =>
        {
            OnMoveFinished?.Invoke();
        });
    }
}