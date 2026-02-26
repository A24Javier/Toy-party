using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;


public class Transicion : MonoBehaviour
{
    [Header("Movimiento Transicion")]
    [SerializeField] private RectTransform trans1;
    [SerializeField] private RectTransform trans2;
    [SerializeField] private float posicionZero = 0f;
    [SerializeField] private float posicionFinal = 1500f;
    [SerializeField] private float duracion = 2.5f;

    private void Awake()
    {
        FueraTelon();
    }

    public void DentroTelon(string sceneName)
    {
        trans1.DOKill();
        trans2.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Append(trans1.DOAnchorPosX(posicionZero, duracion).SetEase(Ease.InQuad));

        seq.Join(trans2.DOAnchorPosX(posicionZero, duracion).SetEase(Ease.InQuad));

        seq.AppendInterval(1f);

        seq.AppendCallback(() =>
        {
            if (sceneName.StartsWith("MG_"))
            {
                SceneManager.LoadScene("LoadingScene");
                return;
            }
            SceneManager.LoadScene(sceneName);
        });
    }

    public void FueraTelon()
    {
        trans1.DOKill();
        trans2.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Append(trans1.DOAnchorPosX(posicionFinal, duracion).SetEase(Ease.InQuad));

        seq.Join(trans2.DOAnchorPosX(-posicionFinal, duracion).SetEase(Ease.InQuad));
    }
}

