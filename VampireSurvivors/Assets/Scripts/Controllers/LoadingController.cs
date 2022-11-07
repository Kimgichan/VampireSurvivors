using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingController : MonoBehaviour
{

    [SerializeField] private CutAgent cutAgent;

    public CutAgent CutAgent => cutAgent;

    private IEnumerator Start()
    {
        while (LoadSceneManager.Instance == null)
            yield return null;

        if(LoadSceneManager.Instance.loadingController != null)
        {
            Destroy(gameObject);
            yield break;
        }

        LoadSceneManager.Instance.loadingController = this;
    }
}
