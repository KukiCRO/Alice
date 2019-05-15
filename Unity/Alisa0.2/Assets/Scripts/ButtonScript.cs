using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    public void ClickStart()
    {
        SceneManager.LoadScene(1);
    }
    public void ClickExit()
    {
        Application.Quit();
    }
}
