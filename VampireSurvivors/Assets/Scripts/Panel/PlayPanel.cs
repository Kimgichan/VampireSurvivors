using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayPanel : MonoBehaviour
{
    [SerializeField] private float playTabOpenTime;
    [SerializeField] private GameObject playTab;
    [SerializeField] private Button multi_x2Btn;
    [SerializeField] private Button singleBtn;
    private IEnumerator playTabOpenCor;

    [Space]
    [SerializeField] private GameObject stayPanel;
    [SerializeField] private TextMeshProUGUI countTxt;
    [SerializeField] private Image timeImg;
    [SerializeField] private float cycleTime;
    [SerializeField] private Button cancelRoomBtn;
    private IEnumerator stayPanelUpdateCor;

    public bool IsStayActive 
    {
        get
        {
            return stayPanel.activeSelf;
        }
    }

    private void Start()
    {
        PlayTabInit();
        StayPanelInit();
    }

    private void OnEnable()
    {
        Open();
    }

    private void OnDisable()
    {
        playTab.SetActive(true);
        stayPanel.SetActive(false);
    }

    private void Open()
    {
        playTab.SetActive(true);
        stayPanel.SetActive(false);

        if(playTabOpenCor != null)
        {
            StopCoroutine(playTabOpenCor);
        }

        playTabOpenCor = PlayTabOpenCor();
        StartCoroutine(playTabOpenCor);
    }

    private void PlayTabInit()
    {
        multi_x2Btn.onClick.AddListener(() => GamePlay(2));
        singleBtn.onClick.AddListener(() => GamePlay(1));
    }
    private void GamePlay(int playerCount)
    {
        if(playerCount == 1)
        {

        }
        else if(playerCount > 0)
        {
            if (NetManager.Instance == null || NetManager.Instance.Client == null)
            {
                return;
            }

            if(GameManager.Instance == null || GameManager.Instance.player == "")
            {
                return;
            }

            var client = NetManager.Instance.Client;
            var enterRoom = new NetNodes.Client.EnterRoom();
            enterRoom.player = GameManager.Instance.player;
            enterRoom.size = playerCount;
            enterRoom.stage = "πÃ¡§";
            client.SendData_EnterRoom(enterRoom);

            playTab.SetActive(false);
            stayPanel.SetActive(true);
            StayPanel_Active();
        }
    }
    private IEnumerator PlayTabOpenCor()
    {
        var timer = 0f;
        playTab.transform.localScale = Vector3.zero;
        while(timer < playTabOpenTime)
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

            playTab.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, timer / playTabOpenTime);
        }
        playTab.transform.localScale = Vector3.one;
        playTabOpenCor = null;
    }

    private void StayPanelInit()
    {
        cancelRoomBtn.onClick.AddListener(OnClick_CancelRoomBtn);
    }
    private void OnClick_CancelRoomBtn()
    {
        if(NetManager.Instance == null || NetManager.Instance.Client == null)
        {
            CancelRoom();
            return;
        }

        if(GameManager.Instance.player == "")
        {
            CancelRoom();
            return;
        }

        var client = NetManager.Instance.Client;
        var cancelRoom = new NetNodes.Client.CancelRoom();
        cancelRoom.player = GameManager.Instance.player;
        client.SendData_CancelRoom(cancelRoom);
    }

    public void CancelRoom()
    {
        if (stayPanelUpdateCor != null)
        {
            StopCoroutine(stayPanelUpdateCor);
            stayPanelUpdateCor = null;
        }

        Open();
    }

    private void StayPanel_Active()
    {
        if(stayPanelUpdateCor != null)
        {
            StopCoroutine(stayPanelUpdateCor);
        }

        stayPanelUpdateCor = StayPanelUpdateCor();
        StartCoroutine(stayPanelUpdateCor);
    }
    public void PlayUpdate(in NetNodes.Server.EnterRoom enterRoom)
    {
        if (!IsStayActive) return;

        countTxt.text = $"{enterRoom.PlayerCount}/{enterRoom.RoomSize}";
    }
    private IEnumerator StayPanelUpdateCor()
    {
        var timer = 0f;
        bool toggle = false;
        timeImg.fillClockwise = !toggle;
        while (true)
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

            if (timeImg.fillClockwise)
            {
                timeImg.fillAmount = Mathf.Lerp(0f, 1f, timer / cycleTime);
            }
            else
            {
                timeImg.fillAmount = Mathf.Lerp(1f, 0f, timer / cycleTime);
            }


            if (timer >= cycleTime)
            {
                timer = 0f;
                toggle = !toggle;
                timeImg.fillClockwise = !toggle;
            }
        }
    }
}
