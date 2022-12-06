using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameOverControl : MonoBehaviour
{


    public void Restart()
    {
        // 重新加载
        SceneManager.LoadScene("Game");
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }
}

