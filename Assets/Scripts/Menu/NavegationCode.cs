using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NavegationCode : MonoBehaviour
{
    [Header("Botón que quiero seleccionar")]
    [SerializeField] private Button elementUi;

    public void SelectNewButton()
    {
        if (elementUi == null) return;

        elementUi.Select();
        EventSystem.current.SetSelectedGameObject(elementUi.gameObject);
    }
}