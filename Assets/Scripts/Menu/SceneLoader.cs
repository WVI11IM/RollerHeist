using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public GameObject loaderUI;
    public Image progressSlider;

    public void LoadScene(string sceneName)
    {
        MusicManager.Instance.StopAllLoopingMusic();
        StartCoroutine(LoadScene_Coroutine(sceneName));
    }

    private IEnumerator LoadScene_Coroutine(string sceneName)
    {
        progressSlider.fillAmount = 0;
        loaderUI.SetActive(true);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;

        float displayedProgress = 0f;

        while (!asyncOperation.isDone)
        {
            float targetProgress = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            displayedProgress = Mathf.MoveTowards(displayedProgress, targetProgress, Time.deltaTime * 0.5f);
            progressSlider.fillAmount = displayedProgress;

            if (asyncOperation.progress >= 0.9f)
            {
                displayedProgress = Mathf.MoveTowards(displayedProgress, 1f, Time.deltaTime * 0.5f);
                progressSlider.fillAmount = displayedProgress;

                if (displayedProgress >= 1f)
                {
                    asyncOperation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }
}