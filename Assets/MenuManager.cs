using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void OnBtnPlayClick() {
        SceneManager.LoadScene("Tree");
    }
    public void OnBtnExitClick() {
        Application.Quit();
    }
}
