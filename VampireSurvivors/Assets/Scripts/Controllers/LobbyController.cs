using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour
{
    private IEnumerator Start()
    {
        while (GameManager.Instance == null)
            yield return null;

        if (GameManager.Instance.lobbyController != null)
        {
            Destroy(gameObject);
            yield break;
        }

        GameManager.Instance.lobbyController = this;
    }

    public void GameStart()
    {
        if(LoadSceneManager.Instance != null)
        {
            LoadSceneManager.Instance.LoadStage();
        }
    }
}
