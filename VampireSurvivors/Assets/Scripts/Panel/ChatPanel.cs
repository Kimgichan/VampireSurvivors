using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField chatInputField;
    [SerializeField] private RectTransform chatBoard;
    [SerializeField] private Transform chatContent;
    [SerializeField] private ChatSlot chatPrefab;
    [SerializeField] private int chatCapacity;
    private Queue<ChatSlot> chats;

    public int ChatCapacity => chatCapacity;

    private void Start()
    {
        chats = new Queue<ChatSlot>(chatCapacity);
        chatInputField.richText = false;
        chatInputField.textComponent.richText = false;
        chatInputField.onSubmit.AddListener(OnSubmit_Chat);
    }

    public void AddChat(string player, string content)
    {
        if (chatContent == null || chatPrefab == null) return;

        while (chats.Count >= chatCapacity)
        {
            var chatSlot = chats.Dequeue();
            Destroy(chatSlot);
        }

        var newChat = Instantiate(chatPrefab, chatContent);
        newChat.Write(player, content);
        chats.Enqueue(newChat);
    }

    public void SetChatScrollBottom()
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

    public void ChatReset()
    {
        chatInputField.text = "";
    }
}
