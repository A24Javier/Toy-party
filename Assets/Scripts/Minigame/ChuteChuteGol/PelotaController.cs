using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PelotaController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] ManagerHUDChutechutegol Manger;

    private void OnEnable()
    {
        Manger.MiPelotaList(this);
    }
}
