using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{
    [SerializeField] private string titleScene;
    [SerializeField] private string lobbyScene;
    [SerializeField] private string playScene;
    [SerializeField] private string roomLobbyScene;
    [SerializeField] private float cooltime;

    private static LoadSceneManager instance;

    public LoadingController loadingController;

    public static LoadSceneManager Instance => instance;

    private IEnumerator loadingCor;

    private IEnumerator loadStageCor;

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

    private void LoadStage()
    {
        if (loadStageCor != null) return;

        loadStageCor = LoadStageCor();
        StartCoroutine(loadStageCor);
    }

    public void OpenStage()
    {
        if (loadingCor != null) return;
        if (loadStageCor == null) return;

        SceneManager.UnloadSceneAsync(roomLobbyScene);
        GameManager.Instance.roomLobbyController = null;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(playScene));

        StopCoroutine(loadStageCor);
        loadStageCor = null;
    }

    private IEnumerator LoadStageCor()
    {
        if (GameManager.Instance == null || GameManager.Instance.player == "")
            yield break;
        while(loadingCor != null)
        {
            yield return null;
        }

        var wait = new WaitForSeconds(cooltime);
        var op = SceneManager.LoadSceneAsync(playScene, LoadSceneMode.Additive);

        while (true)
        {
            yield return wait;
            if (NetManager.Instance != null)
            {
                if(NetManager.Instance.Client != null)
                {
                    NetManager.Instance.Client.SendData_Ready(new NetNodes.Client.Ready()
                    {
                        percent = op.progress,
                        player = GameManager.Instance.player
                    });
                }
            }
        }
    }

    public void LoadLobby()
    {
        if (loadingCor != null) return;

        //loadingCor = LoadLobbyCor();
        //StartCoroutine(loadingCor);
        loadingCor = LoadSceneCor(SceneManager.GetActiveScene().name, lobbyScene);
        StartCoroutine(loadingCor);
    }

    public void LoadTitle()
    {
        if (loadingCor != null) return;

        //loadingCor = LoadLobbyCor();
        //StartCoroutine(loadingCor);
        loadingCor = LoadSceneCor(SceneManager.GetActiveScene().name, titleScene);
        StartCoroutine(loadingCor);
    }

    public void LoadRoomLobby()
    {
        if (loadingCor != null) return;
        if (loadStageCor != null) return;

        loadingCor = LoadSceneCor(SceneManager.GetActiveScene().name, roomLobbyScene);
        StartCoroutine(loadingCor);
        LoadStage();
    }

    private IEnumerator LoadSceneCor(string beforeScene, string afterScene)
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
        var afterSceneOP = SceneManager.LoadSceneAsync(afterScene, LoadSceneMode.Additive);

        while (!afterSceneOP.isDone)
        {
            yield return null;
        }
        var afterSceneScene = SceneManager.GetSceneByName(afterScene);
        SceneManager.SetActiveScene(afterSceneScene);

        var unBeforSceneOP = SceneManager.UnloadSceneAsync(beforeScene);

        loadingController.CutAgent.OnCutIn();
        while (!loadingController.CutAgent.Complete)
        {
            yield return null;
        }
        var unLoadingOP = SceneManager.UnloadSceneAsync("LoadingScene");

        while ((!unBeforSceneOP.isDone) || (!unLoadingOP.isDone))
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
