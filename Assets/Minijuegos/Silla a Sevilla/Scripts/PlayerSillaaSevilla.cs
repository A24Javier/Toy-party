public class PlayerSillaaSevilla
{
    int id;
    int posicionFinal;

    public PlayerSillaaSevilla(int id, int pos)
    {
        this.id = id;
        this.posicionFinal = pos;
    }

    public int VerId()
    {
        return id;
    }

    public int VerPos()
    {
        return posicionFinal;
    }

    public void Setid(int id)
    {
        this.id = id;
    }

    public void SetPos(int pos)
    {
        this.posicionFinal = pos;
    }
}