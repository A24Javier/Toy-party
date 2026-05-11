public static class DatosMinijuego
{
    public static int cantidadJugadores = 4;

    public static int[] ids = new int[4];
    public static int[] posiciones = new int[4];
    public static int[] puntos = new int[4];
    public static int[] monedas = new int[4];
    public static int[] estrellas = new int[4];

    public static string escenaTablero = "TABLERO";
    public static string escenaRecompensas = "NivelRecompensasMinijuegos";

    public static void ResetDatos()
    {
        cantidadJugadores = 4;

        for (int i = 0; i < 4; i++)
        {
            ids[i] = i + 1;
            posiciones[i] = i + 1;
            puntos[i] = 0;
            monedas[i] = 0;
            estrellas[i] = 0;
        }
    }
}