using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Button : MonoBehaviour
{
    public void SceneChange(int i)
    {
        SceneManager.LoadScene(i);
    }
}
