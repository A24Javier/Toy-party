using UnityEngine;

public class PelotaController : MonoBehaviour
{
    [SerializeField] ManagerHUDChutechutegol Manger;

    private void OnEnable()
    {
        if (Manger != null)
            Manger.MiPelotaList(this);
        else
            Debug.LogWarning("PelotaController no tiene ManagerHUDChutechutegol asignado.");
    }
}