using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    // �tv�lt a megadott Scene-re
    public void moveToScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }
    // Kil�p az alkalmaz�sb�l
    public void Exit()
    {
        print("exit");
        Application.Quit();
    }
}
