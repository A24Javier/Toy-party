using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class MouseParticles : MonoBehaviour
{
    [SerializeField] private float distancia = 10;
    [SerializeField] private ParticleSystem clickEffect;
    [SerializeField] private Camera particleCamera;
    public bool isEfectoActivado;

    public static MouseParticles Instance;

    void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        RefreshCamera();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshCamera();
    }

    public void RefreshCamera()
    {
        UniversalAdditionalCameraData data = Camera.main.GetUniversalAdditionalCameraData();

        if(data.cameraStack.Count > 0)
        {
            particleCamera = data?.cameraStack[0];
            isEfectoActivado = true;
        }
        else
        {
            particleCamera = null;
            isEfectoActivado = false;
        }
    }

    void Update()
    {
        if (!isEfectoActivado)
            return;

        if (particleCamera == null)
        {
            RefreshCamera();
            if (particleCamera == null) return;
        }

        Ray r = particleCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 pos = r.GetPoint(distancia);

        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(clickEffect, pos, Quaternion.identity);
        }
    }
}