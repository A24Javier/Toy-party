using UnityEngine;
using FMODUnity;

public class Sondido : MonoBehaviour
{
    [System.Serializable]
    public class SoundEntry
    {
        public string id;
        public EventReference sound;
    }

    [Header("Lista de sonidos")]
    public SoundEntry[] sounds;

    public void Play(string id)
    {
        foreach (SoundEntry entry in sounds)
        {
            if (entry.id == id)
            {
                PlaySound(entry.sound);
                return;
            }
        }

        Debug.LogWarning("No se encontró el sonido con id: " + id);
    }

    public void PlaySound(EventReference sound)
    {
        if (sound.IsNull)
        {
            Debug.LogWarning("El sonido FMOD está vacío.");
            return;
        }

        RuntimeManager.PlayOneShot(sound);
    }
}