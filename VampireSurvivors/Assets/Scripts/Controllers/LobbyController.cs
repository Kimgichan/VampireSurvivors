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
    [SerializeField] private Button chatPanel;
    [SerializeField] private TMP_InputField chatInputField;
    [SerializeField] private RectTransform chatBoard;
    [SerializeField] private Transform chatContent;
    [SerializeField] private ChatSlot chatPrefab;
    [SerializeField] private int chatCapacity;
    private Queue<ChatSlot> chats;
    private IEnumerator chatOpenCor;

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
    }

    public void GameStart()
    {
        if(LoadSceneManager.Instance != null)
        {
            LoadSceneManager.Instance.LoadStage();
        }
    }

    private void ChatInit()
    {
        chats = new Queue<ChatSlot>(chatCapacity);

        chatOpenBtn.onClick.AddListener(OnClick_ChatOpen);
        chatPanel.onClick.AddListener(OnClick_ChatClose);
        chatInputField.onSubmit.AddListener(OnSubmit_Chat);
    }

    public void AddChat(string player, string content)
    {
        if (chatContent == null || chatPrefab == null) return;

        while(chats.Count >= chatCapacity)
        {
            var chatSlot = chats.Dequeue();
            Destroy(chatSlot);
        }

        var newChat = Instantiate(chatPrefab, chatContent);
        newChat.Write(player, content);
        chats.Enqueue(newChat);
    }

    private IEnumerator ChatOpenCor()
    {
        if (chatPanel == null) yield break;
        if (chatBoard == null) yield break;

        var image = chatPanel.image;
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
        if (chatPanel == null) yield break;
        if (chatBoard == null) yield break;

        var image = chatPanel.image;
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
        chatPanel.gameObject.SetActive(false);
        chatOpenCor = null;
    }
    private void OnClick_ChatOpen()
    {
        if (chatOpenBtn == null || chatPanel == null) return;

        chatOpenBtn.gameObject.SetActive(false);
        chatPanel.gameObject.SetActive(true);

        SetChatScrollBottom();

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

        chatOpenCor = ChatCloseCor();
        StartCoroutine(chatOpenCor);
    }
    private void OnSubmit_Chat(string msg)
    {
        chatInputField.text = "";
        SetChatScrollBottom();

        if (NetManager.Instance == null || NetManager.Instance.Client == null)
            return;

        if (GameManager.Instance == null || GameManager.Instance.player == "")
            return;
        var chat = new NetNodes.Client.Chat(GameManager.Instance.player, msg);
        NetManager.Instance.Client.SendData_Chat(chat);
    }
    private void SetChatScrollBottom()
    {
        if (chatBoard != null)
        {
            var scroll = chatBoard.GetComponent<ScrollRect>();
            if (scroll != null)
            {
                scroll.normalizedPosition = Vector2.zero;
            }
        }
    }
}
