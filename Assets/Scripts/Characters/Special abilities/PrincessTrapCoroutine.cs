using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrincessTrapCoroutine : MonoBehaviour
{
    [SerializeField] private GameObject _prefabTrap;

    private BoardCameraController _cameraController;
    public static PrincessTrapCoroutine Instance;

    void Awake()
    {
        if (Instance != this && Instance != null) { Destroy(Instance); return; }
        Instance = this;
    }

    void Start()
    {
        _cameraController = FindAnyObjectByType<BoardCameraController>();
    }

    public void StartTrapCoroutine(HashSet<Box> aleatoryBoxes)
    {
        StartCoroutine(TrapCoroutine(aleatoryBoxes));
    }

    private IEnumerator TrapCoroutine(HashSet<Box> aleatoryBoxes)
    {
        foreach(Box box in aleatoryBoxes)
        {
            // Plantamos la trampa en la casilla
            GameObject trap = Instantiate(_prefabTrap, box.transform.position + (Vector3.up * 0.1f), Quaternion.identity);
            _cameraController.SetTarget(trap.transform);

            // Hacemos saber a la casilla que tiene una trampa
            box.SetTrap(trap);

            yield return new WaitForSeconds(1f);
        }

        Character character = GameController.instance.GetCharacterOfTurn();
        _cameraController.SetTarget(character.transform);
        character.usingAbility = false;
    }
}
