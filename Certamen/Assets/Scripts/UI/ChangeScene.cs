using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    // átvált a megadott Scene-re
    public void moveToScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }
    // Kilép az alkalmazásból
    public void Exit()
    {
        print("exit");
        Application.Quit();
    }
}
