using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ControllerFruta : MonoBehaviour
{
    [SerializeField] InputActionAsset MappingContext;
    InputAction soltarFruta;

    [SerializeField] GameObject[] Frutas;
    
    [SerializeField] GameObject[] PosArbol;

    [SerializeField] TextMeshProUGUI timetext;

    bool IA = true;

    float timeSpwnIA = 0;

    float time = 60.0f;
    public float timeDuration;

    public bool frutaTime = true;

    private void Start()
    {
        MappingContext.Enable();
        soltarFruta = MappingContext.FindActionMap("Soltar").FindAction("Fruta");
        timeDuration = time;
    }

    private void Update()
    {
        if (frutaTime)
        {
            InstaciarFruta();
            timeController();
            timetext.text = timeDuration.ToString("F2");
        }
    }

    void Winner()
    {
        
    }

    void timeController()
    {
        if (timeDuration > 0)
        {
            timeDuration -= Time.deltaTime;
        }
        else
        {
            Winner();
            frutaTime = false;
            timetext.text = "00,00";
        }
    }
    void InstaciarFruta()
    {
        if (soltarFruta.triggered)
            Instantiate(Frutas[0], PosArbol[0].transform.position,Quaternion.identity);
        if (IA)
        {
            timeSpwnIA += Time.deltaTime;
            if (timeSpwnIA > 2f)
            {
                Instantiate(Frutas[1], PosArbol[1].transform.position, Quaternion.identity);
                timeSpwnIA = 0;
            }
        }
           
    }
}
