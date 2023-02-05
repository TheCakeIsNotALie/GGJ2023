using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public void OnContinueClick() {
        GameManager.instance.Continue();
    }

    public void OnMenuClick() {
        SceneManager.LoadScene("Main Screen");
    }
}
