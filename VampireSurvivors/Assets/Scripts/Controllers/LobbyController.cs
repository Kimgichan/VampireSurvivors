using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;
using TMPro;

public class LobbyController : MonoBehaviour
{
    [SerializeField] private float chatOpenTime;
    [SerializeField] private Button chatOpenBtn;
    [SerializeField] private ChatPanel chatPanel;
    [SerializeField] private Button chatCloseBtn;
    [SerializeField] private RectTransform chatBoard;
    private IEnumerator chatOpenCor;

    [Space]
    [SerializeField] Button playPanelOpenBtn;
    [SerializeField] private PlayPanel playPanel;
    [SerializeField] private Button playPanelCloseBtn;
    [SerializeField] private float playPanelOnOffTime;
    private IEnumerator playPanelSwitchCor;

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
        ChatInit();
        PlayPanelInit();
    }

    #region Chat Panel
    private void ChatInit()
    {
        chatOpenBtn.onClick.AddListener(OnClick_ChatOpen);
        var btn = chatPanel.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnClick_ChatClose);
        }
    }

    public void AddChat(string player, string content)
    {
        if(chatPanel != null)
        {
            chatPanel.AddChat(player, content);
        }
    }

    private IEnumerator ChatOpenCor()
    {
        if (chatCloseBtn == null) yield break;
        if (chatBoard == null) yield break;

        var image = chatCloseBtn.image;
        var timer = 0f;
        while (timer < chatOpenTime)
        {
            yield return null;

            var TSC = GameManager.GetTimeScaleController();
            if (TSC != null)
            {
                timer += TSC.UITimeScaleUpdate;
            }
            else
            {
                timer += Time.deltaTime;
            }
            var percent = timer / chatOpenTime;

            var col = image.color;
            col.a = Mathf.Lerp(0f, 0.8f, percent);
            image.color = col;

            var pivot = chatBoard.pivot;
            pivot.x = Mathf.Lerp(1f, 0f, percent);
            chatBoard.pivot = pivot;
        }

        {
            var col = image.color;
            col.a = 0.8f;
            image.color = col;

            var pivot = chatBoard.pivot;
            pivot.x = 0f;
            chatBoard.pivot = pivot;
        }

        chatOpenCor = null;
    }
    private IEnumerator ChatCloseCor()
    {
        if (chatCloseBtn == null) yield break;
        if (chatBoard == null) yield break;

        var image = chatCloseBtn.image;
        var timer = 0f;
        while (timer < chatOpenTime)
        {
            yield return null;

            var TSC = GameManager.GetTimeScaleController();
            if (TSC != null)
            {
                timer += TSC.UITimeScaleUpdate;
            }
            else
            {
                timer += Time.deltaTime;
            }
            var percent = timer / chatOpenTime;

            var col = image.color;
            col.a = Mathf.Lerp(0.8f, 0f, percent);
            image.color = col;

            var pivot = chatBoard.pivot;
            pivot.x = Mathf.Lerp(0f, 1f, percent);
            chatBoard.pivot = pivot;
        }

        chatOpenBtn.gameObject.SetActive(true);
        chatCloseBtn.gameObject.SetActive(false);
        chatOpenCor = null;
    }
    private void OnClick_ChatOpen()
    {
        if (chatOpenBtn == null || chatCloseBtn == null || chatPanel == null) return;

        chatOpenBtn.gameObject.SetActive(false);
        chatCloseBtn.gameObject.SetActive(true);

        chatPanel.SetChatScrollBottom();

        if(chatOpenCor != null)
        {
            StopCoroutine(chatOpenCor);
        }
        chatOpenCor = ChatOpenCor();
        StartCoroutine(chatOpenCor);
    }
    private void OnClick_ChatClose()
    {
        if (chatOpenCor != null)
        {
            StopCoroutine(chatOpenCor);
        }

        if (chatPanel != null)
        {
            chatPanel.ChatReset();
        }
        chatOpenCor = ChatCloseCor();
        StartCoroutine(chatOpenCor);
    }
    #endregion

    #region Play Panel
    private void PlayPanelInit()
    {
        playPanelOpenBtn.onClick.AddListener(PlayPanelOpen);
        playPanelCloseBtn.onClick.AddListener(PlayPanelClose);
    }
    private void PlayPanelOpen()
    {
        if(playPanelSwitchCor != null)
        {
            StopCoroutine(playPanelSwitchCor);
        }

        playPanelSwitchCor = PlayPanelOpenCor();
        StartCoroutine(playPanelSwitchCor);
    }

    public void EnterRoom(in NetNodes.Server.EnterRoom enterRoom)
    {
        playPanel.PlayUpdate(enterRoom);
    }

    private void PlayPanelClose()
    {
        if (playPanel.IsStayActive)
        {
            return;
        }

        if(playPanelSwitchCor != null)
        {
            StopCoroutine(playPanelSwitchCor);
        }
        playPanelSwitchCor = PlayPanelCloseCor();
        StartCoroutine(playPanelSwitchCor);
    }
    private IEnumerator PlayPanelOpenCor()
    {
        playPanel.gameObject.SetActive(true);
        var timer = 0f;
        var group = playPanelCloseBtn.GetComponent<CanvasGroup>();

        if(group == null)
        {
            playPanelSwitchCor = null;
            yield break;
        }
        while(timer < playPanelOnOffTime)
        {
            yield return null;

            var TSC = GameManager.GetTimeScaleController();
            if(TSC != null)
            {
                timer += TSC.UITimeScaleUpdate;
            }
            else
            {
                timer += Time.deltaTime;
            }

            group.alpha = Mathf.Lerp(0f, 1f, timer / playPanelOnOffTime);
        }

        group.alpha = 1f;
        playPanelSwitchCor = null;
    }
    private IEnumerator PlayPanelCloseCor()
    {
        var timer = 0f;
        var group = playPanelCloseBtn.GetComponent<CanvasGroup>();
       
        if (group == null)
        {
            playPanelSwitchCor = null;
            yield break;
        }

        group.interactable = false;
        while (timer < playPanelOnOffTime)
        {
            yield return null;

            var TSC = GameManager.GetTimeScaleController();
            if (TSC != null)
            {
                timer += TSC.UITimeScaleUpdate;
            }
            else
            {
                timer += Time.deltaTime;
            }

            group.alpha = Mathf.Lerp(1f, 0f, timer / playPanelOnOffTime);
        }

        group.interactable = true;
        playPanel.gameObject.SetActive(false);
        playPanelSwitchCor = null;
    }
    public void CancelRoom()
    {
        playPanel?.CancelRoom();
    }
    #endregion
}
