using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MouseParticles : MonoBehaviour
{
    [SerializeField] private float distancia = 10;
    [SerializeField] private ParticleSystem clickEffect;
    [SerializeField] private Camera particleCamera;
    public bool isEfectoActivado;

    public static MouseParticles Instance;

    void Awake()
    {
        if (Instance != this && Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (isEfectoActivado)
        {
            Ray r = particleCamera.ScreenPointToRay(Input.mousePosition);
            Vector3 pos = r.GetPoint(distancia);

            if (Input.GetMouseButtonDown(0))
            {
                Instantiate(clickEffect, pos, Quaternion.identity);
            }
        }
    }
}
