using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalaPersonajes : MonoBehaviour
{
    public enum Minijuegos { StripSlack, SplashSplashShoot, SillaSevilla, RelayRace, ChuteChuteGol }

    // Esta estructura nos permite empaquetar los datos de configuración visual
    [System.Serializable]
    public struct AjustesVisuales
    {
        public string nombrePersonaje; // Solo para identificarlo en el inspector
        public int idPersonaje;        // El ID que coincide con tu listaPersonajes
        public Vector3 escala;
        public Vector3 posicionOffset;
        public Vector3 rotacion;
    }

    // Esta estructura asocia un minijuego con las configuraciones de TODOS los personajes
    [System.Serializable]
    public struct ConfigMinijuego
    {
        public Minijuegos minijuego;
        public List<AjustesVisuales> ajustesDePersonajes;
    }

    [Header("Configuración de todos los Minijuegos")]
    [SerializeField] private List<ConfigMinijuego> listaConfiguraciones;

    // Variables internas para guardar la configuración del minijuego actual
    private Dictionary<int, AjustesVisuales> ajustesActualesActivos = new Dictionary<int, AjustesVisuales>();

    // Llamar en el awake de cada minijuego pasándole el enum directamente o el número
    public void InicializarMinijuego(int numGame)
    {
        Minijuegos juegoSeleccionado = (Minijuegos)(numGame - 1);
        CargarAjustesDelJuego(juegoSeleccionado);
    }

    void CargarAjustesDelJuego(Minijuegos juego)
    {
        ajustesActualesActivos.Clear();

        // Buscamos la configuración que coincida con el minijuego actual
        ConfigMinijuego configJuego = listaConfiguraciones.Find(c => c.minijuego == juego);

        // Pasamos los datos al diccionario para que el ModelController los lea al instante
        if (configJuego.ajustesDePersonajes != null)
        {
            foreach (var ajuste in configJuego.ajustesDePersonajes)
            {
                ajustesActualesActivos[ajuste.idPersonaje] = ajuste;
            }
        }
    }

    // Esta es la función que usará tu ModelController para saber qué Vector3 aplicar
    public void ObtenerTransformaciones(int idPersonaje, out Vector3 escala, out Vector3 posicion, out Vector3 rotacion)
    {
        // Si el personaje actual tiene ajustes específicos para este minijuego, los aplicamos
        if (ajustesActualesActivos.TryGetValue(idPersonaje, out AjustesVisuales ajustes))
        {
            escala = ajustes.escala;
            posicion = ajustes.posicionOffset;
            rotacion = ajustes.rotacion;
        }
        else
        {
            // Valores predeterminados si no se configuró nada en el inspector
            escala = Vector3.one;
            posicion = Vector3.zero;
            rotacion = Vector3.zero;
        }
    }
}