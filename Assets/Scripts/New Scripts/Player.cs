using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CasillaEvent : UnityEvent<Player, Box> { }

public class Player : MonoBehaviour
{
    [SerializeField] private Board board;
    private Vector3 upToBox = new Vector3 (0f, 0.6f, 0f);
    private Box actualBox1;
    [SerializeField] private float speed = 3f;
    public bool isSelectingPath = false;

    // Información del jugador
    private Sprite playerImage = null;
    private int coins = 0;
    private int stars = 0;

    public CasillaEvent onInNewBox;

    void Start()
    {
        actualBox1 = board.GetCasilla(0);
        transform.position = actualBox1.GetThisBoxTransf().position + upToBox;
    }

    public void Move(int steps)
    {
        StartCoroutine(MoveCharacterBoard(steps));
    }

    private IEnumerator MoveCharacterBoard(int steps)
    {
        Box newBox = null;

        for (int i = 0; i < steps; i++)
        {
            newBox = actualBox1.GetNewBox(0);

            Vector3 destination = newBox.GetThisBoxTransf().position + upToBox;

            while (Vector3.Distance(transform.position, destination) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
                yield return null;
            }

            transform.position = destination;
            actualBox1 = newBox;

            if (actualBox1.PossiblesBoxesCount() >= 2)
            {
                Debug.Log("Esta en encrucijada");
                isSelectingPath = true;

                UIManager.instance.SetActualPlayer(this);
                UIManager.instance.CreateSelectionPath(newBox);

                // Es una encrucijada
                // Desactivar movimiento

                while (isSelectingPath)
                {
                    yield return null;
                }
                UIManager.instance.DeactivatePathDecision();
                newBox = actualBox1;
                // Activar movimiento
                
            }

            yield return new WaitForSeconds(0.2f);
        }

        newBox.ActiveEffect(this);
    }

    public IEnumerator PathSelected(Vector3 destination1, Box box)
    {
        destination1 += upToBox;

        while (Vector3.Distance(transform.position, destination1) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination1, speed * Time.deltaTime);
            yield return null;
        }
        actualBox1 = box;
        isSelectingPath = false;
    }

    // -- Getters y Setters --

    public Sprite GetPlayerImage()
    {
        return playerImage;
    }

    public int GetPlayerCoins()
    {
        return coins;
    }

    public void SetPlayerCoins(int newCoins)
    {
        coins = newCoins;
    }

    public void GetStar()
    {
        stars++;
    }

    public int GetPlayerStars()
    {
        return stars;
    }
}
