using UnityEngine;
using UnityEngine.UI;

public class ArrowInfo : MonoBehaviour
{
    private Box boxArranged;
    private Vector3 nodeArranged;
    private Player player;

    public void ArrangeVector(Vector3 box)
    {
        nodeArranged = box;
    }

    public void SetPlayer(Player newPlayer)
    {
        player = newPlayer;
    }

    public void SetBox(Box box)
    {
        boxArranged = box;
        GetComponent<Button>().onClick.AddListener(SelectArrow);
    }

    public void SelectArrow()
    {
        StartCoroutine(player.PathSelected(nodeArranged, boxArranged));
    }
}
