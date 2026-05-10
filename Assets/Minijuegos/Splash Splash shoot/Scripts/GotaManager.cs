using UnityEngine;

public class GotaManager : MonoBehaviour
{
    private ManagerSplashSplashShoot manager;

    private void Start()
    {
        manager = FindObjectOfType<ManagerSplashSplashShoot>();

        if (manager != null)
            manager.RegistrarGota(transform);
    }

    private void OnDestroy()
    {
        if (manager != null)
            manager.EliminarGota(transform);
    }
}