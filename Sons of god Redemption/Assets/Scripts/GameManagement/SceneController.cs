using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneController : MonoBehaviour {

    public bool isLoading;
    public float progress;

    public void changeScene(string sceneName)
    {
        StartCoroutine(LoadAsynchronously(sceneName));
        isLoading = true;
    }

    IEnumerator LoadAsynchronously (string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
        {
            progress = Mathf.Clamp01(operation.progress / .9f);
            yield return null;
        }
    }

    public void exitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
