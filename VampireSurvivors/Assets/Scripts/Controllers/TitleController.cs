using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using TMPro;

public class TitleController : MonoBehaviour
{
    [SerializeField] private float loadingTime;
    [SerializeField] private Button loginBtn;
    [SerializeField] private TMP_InputField player;

    [SerializeField] private TextMeshProUGUI error;
    [SerializeField] private float errorDuration;
    private IEnumerator errorCor;
    private IEnumerator loginCor;

    public string Error
    {
        set
        {
            error.text = $"*{value}*";
            if(errorCor != null)
            {
                StopCoroutine(errorCor);
            }
            errorCor = ErrorCor();
            StartCoroutine(errorCor);
        }
    }

    private IEnumerator Start()
    {
        while (GameManager.Instance == null)
            yield return null;

        if(GameManager.Instance.titleController != null)
        {
            Destroy(gameObject);
            yield break;
        }

        GameManager.Instance.titleController = this;

        loginBtn.onClick.AddListener(Login);
    }


    private void Login()
    {
        #region Unity InputField에서 설정을 Name으로 하니 특수문자 입력이 불가능했음
        //if (Regex.IsMatch(player.text, @"[^a-zA-Z0-9가-힣]"))
        //{
        //    Debug.Log("확인");
        //}
        #endregion

        if(player.text.Length > 0)
        {
            if(loginCor == null)
            {
                loginCor = LoginCor();
                StartCoroutine(loginCor);
            }
        }
        else
        {
            Error = "닉넴임을 설정하지 않았습니다";
        }
    }

    private IEnumerator LoginCor()
    {
        if (NetManager.Instance != null || NetManager.Instance.Client != null)
        {
            Error = "서버에 연결중";

            var client = NetManager.Instance.Client;
            client.ConnectToServer();

            var timer = 0f;
            while (timer < loadingTime)
            {
                yield return null;

                timer += Time.deltaTime;
                if (client.IsConnect)
                {
                    NetManager.Instance.Client.SendData_Login(new NetNodes.Client.Login() { player = player.text });
                    Error = "서버 연결 성공";

                    yield break;
                }
            }

            client.Close();
        }

        Error = "서버 연결 실패";
        LoginReset();
    }

    public void LoginReset()
    {
        if(loginCor != null)
        {
            StopCoroutine(loginCor);
            loginCor = null;
        }
    }

    private IEnumerator ErrorCor()
    {
        error.gameObject.SetActive(true);
        var current = 0f;
        var col = error.color;
        col.a = 1f;
        error.color = col;

        while(current < errorDuration)
        {
            yield return null;

            current += Time.deltaTime;
            col.a = Mathf.Lerp(1, 0, current / errorDuration);

            error.color = col;
        }
        yield return null;
        error.gameObject.SetActive(false);
        errorCor = null;
    }
}
