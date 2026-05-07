using UnityEngine;

public class CoinScript : MonoBehaviour
{
    private ParticleSystem _particleSystem;

    void Start()
    {
        _particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("NPC"))
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            Destroy(gameObject, 1f);
            _particleSystem.transform.parent = null;
            _particleSystem.Play();
        }
    }
}
