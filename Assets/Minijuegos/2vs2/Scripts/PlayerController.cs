using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerInfo playerInfo;
    [SerializeField] Manager2vs2 Man;
    private void OnEnable()
    {
        Man.AñadirJuagdores(this);
    }

    private void Awake()
    {
        playerInfo = GetComponent<PlayerInfo>();
    }

    public PlayerInfo GetPlayerInfo() { return playerInfo; }
}
