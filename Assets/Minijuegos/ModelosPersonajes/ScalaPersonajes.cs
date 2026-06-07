using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalaPersonajes : MonoBehaviour
{
    public enum Minijuegos { StripSlack, SplashSplashShoot, SillaSevilla, RelayRace, ChuteChuteGol, SpeedyTrack, HudRecompensas}
    public Minijuegos JuegoActual;

    // Esta estructura nos permite empaquetar los datos de configuración visual
    [System.Serializable]
    public struct AjustesVisuales
    {
        public string nombrePersonaje;
        public int idPersonaje;
        public Vector3 escala;
        public Vector3 posicionOffset;
        public Vector3 rotacion;
    }

    [System.Serializable]
    public struct ConfigMinijuego
    {
        public Minijuegos minijuego;
        public List<AjustesVisuales> ajustesDePersonajes;
    }

    [Header("Configuración de todos los Minijuegos")]
    [SerializeField] private List<ConfigMinijuego> listaConfiguraciones;

    private Dictionary<int, AjustesVisuales> ajustesActualesActivos = new Dictionary<int, AjustesVisuales>();

    public void InicializarMinijuego(int numGame)
    {
        JuegoActual = (Minijuegos)(numGame - 1);
        CargarAjustesDelJuego(JuegoActual);
    }

    void CargarAjustesDelJuego(Minijuegos juego)
    {
        ajustesActualesActivos.Clear();

        ConfigMinijuego configJuego = listaConfiguraciones.Find(c => c.minijuego == juego);

        if (configJuego.ajustesDePersonajes != null)
        {
            foreach (var ajuste in configJuego.ajustesDePersonajes)
            {
                ajustesActualesActivos[ajuste.idPersonaje] = ajuste;
            }
        }
    }

    public void ObtenerTransformaciones(int idPersonaje, out Vector3 escala, out Vector3 posicion, out Vector3 rotacion, int numeroJugador)
    {
        if (ajustesActualesActivos.TryGetValue(idPersonaje, out AjustesVisuales ajustes))
        {
            escala = ajustes.escala;
            posicion = ajustes.posicionOffset;
            rotacion = ajustes.rotacion;
        }
        else
        {
            escala = Vector3.one;
            posicion = Vector3.zero;
            rotacion = Vector3.zero;
        }

        if (JuegoActual == Minijuegos.StripSlack)
        {
            float rotacionY = 0;

            if (numeroJugador == 1)
            {
                rotacionY = 90f;
            }
            else if (numeroJugador == 2)
            {
                rotacionY = -90f;
            }

            rotacion.y += rotacionY;
        }
    }
}