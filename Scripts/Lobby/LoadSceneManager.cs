using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneManager : MonoBehaviour
{
    public enum SceneType
    {
        Lobby,
        Game
    }

    public Image backgroundImage;
    public Slider progressBar;

    private static int sceneIdx;

    private static AsyncOperation op;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        StartCoroutine(LoadingScene());
    }

    public static void LoadScene(SceneType sceneType)
    {
        switch (sceneType)
        {
            case SceneType.Lobby:
                sceneIdx = 0;
                break;
            case SceneType.Game:
                sceneIdx = 2;
                break;
            default:
                UIManager.Instance.ShowErrUi("알수없는 씬");
                break;
        }
        UIManager.Instance.FadeOut(LoadLoadingScene);
    }

    private static void LoadLoadingScene()
    {
        PhotonNetwork.LoadLevel(1);
    }        

    IEnumerator LoadingScene()
    {
        yield return null;
        PhotonNetwork.LoadLevel(sceneIdx);

        float timer = 0.0f;

        while (PhotonNetwork.LevelLoadingProgress == 1)
        {
            yield return null;

            progressBar.value = Mathf.Lerp(progressBar.value, PhotonNetwork.LevelLoadingProgress, timer);
            UIManager.Instance.FadeOut(OnLoadingFinish);
        }
    }

    public static void OnLoadingFinish()
    {
        if(op != null)
            op.allowSceneActivation = true;
    }
}