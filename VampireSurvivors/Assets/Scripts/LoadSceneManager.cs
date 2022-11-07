using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{
    private static LoadSceneManager instance;

    public LoadingController loadingController;

    public static LoadSceneManager Instance => instance;

    private IEnumerator loadingCor;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadStage()
    {
        //로딩창으로 가려서 중복 씬 로드 호출을 막을 예정
        //SceneManager.Loa
        if (loadingCor != null) return;

        loadingCor = LoadStageCor();
        StartCoroutine(loadingCor);
    }

    public void LoadLobby()
    {
        if (loadingCor != null) return;

        loadingCor = LoadLobbyCor();
        StartCoroutine(loadingCor);
    }

    private IEnumerator LoadStageCor()
    {
        yield return null;
        var loadingOP = SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive);

        while(loadingController == null)
        {
            yield return null;
        }

        yield return null;
        loadingController.CutAgent.OnCutOut();
        while (!loadingController.CutAgent.Complete)
        {
            yield return null;
        }

        ControllerReset();
        var gameOP = SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);

        while (!gameOP.isDone)
        {
            yield return null;
        }
        var gameScene = SceneManager.GetSceneByName("GameScene");
        SceneManager.SetActiveScene(gameScene);

        var unLobbyOP = SceneManager.UnloadSceneAsync("LobbyScene");

        loadingController.CutAgent.OnCutIn();
        while (!loadingController.CutAgent.Complete)
        {
            yield return null;
        }
        var unLoadingOP = SceneManager.UnloadSceneAsync("LoadingScene");

        while((!unLobbyOP.isDone) || (!unLoadingOP.isDone))
        {
            yield return null;
        }
        loadingCor = null;
    }

    private IEnumerator LoadLobbyCor()
    {
        yield return null;
        var loadingOP = SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive);

        while (loadingController == null)
        {
            yield return null;
        }

        yield return null;
        loadingController.CutAgent.OnCutOut();
        while (!loadingController.CutAgent.Complete)
        {
            yield return null;
        }

        ControllerReset();
        var gameOP = SceneManager.LoadSceneAsync("LobbyScene", LoadSceneMode.Additive);

        while (!gameOP.isDone)
        {
            yield return null;
        }
        var lobbyScene = SceneManager.GetSceneByName("LobbyScene");
        SceneManager.SetActiveScene(lobbyScene);

        var unGameOP = SceneManager.UnloadSceneAsync("GameScene");

        loadingController.CutAgent.OnCutIn();
        while (!loadingController.CutAgent.Complete)
        {
            yield return null;
        }
        var unLoadingOP = SceneManager.UnloadSceneAsync("LoadingScene");

        while((!unGameOP.isDone) || (!unLoadingOP.isDone))
        {
            yield return null;
        }

        loadingCor = null;
    }

    public static LoadingController GetLoadingController()
    {
        if(Instance == null || Instance.loadingController == null)
        {
            return null;
        }
        return Instance.loadingController;
    }

    public void ControllerReset()
    {
        GameManager.Instance?.GMReset();
        AudioManager.Instance?.AMReset();
    }
}
