using UnityEngine;
using UnityEngine.InputSystem;

public class RebindSaveLoad : MonoBehaviour
{
    [SerializeField] private InputActionAsset actions;

    private const string PlayerPrefsKey = "REBIND_OVERRIDES_JSON";

    private void Awake()
    {
        Load();
    }

    public void Save()
    {
        if (actions == null) return;

        string json = actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(PlayerPrefsKey, json);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        if (actions == null) return;
        if (!PlayerPrefs.HasKey(PlayerPrefsKey)) return;

        string json = PlayerPrefs.GetString(PlayerPrefsKey);
        actions.LoadBindingOverridesFromJson(json);
    }

    public void ResetToDefault()
    {
        if (actions == null) return;

        actions.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey(PlayerPrefsKey);
        PlayerPrefs.Save();
    }
}