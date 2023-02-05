using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void OnBtnPlayClick() {
        print("Loading main Game");
        SceneManager.LoadScene("Tree");
    }
    public void OnBtnSettingsClick()
    {
        print("Loading Settings");
        SceneManager.LoadScene(2);
    }
    public void OnBtnExitClick() {
        print("Quitting app");
        Application.Quit();
    }
}
