using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Nodes;

public class SkillBook : MonoBehaviour
{
    [SerializeField] private string warningSFX;
    [SerializeField] private TextMeshProUGUI currentLevelTxt;
    [SerializeField] private Button slotMachineResetBtn;
    [SerializeField] private TextMeshProUGUI resetPriceTxt;
    private int resetPrice;
    [SerializeField] private List<SkillSlotBtn> purchaseSlotBtns;
    [SerializeField] private List<int> priceList;

    /// <summary>
    /// 값이 5면 5개 중에 1개가 나올 확률이라는 뜻.
    /// 값이 12면 12개 중에 1개
    /// </summary>
    [SerializeField] private int completeSkillFreq;

    [Space]
    [SerializeField] private GameObject contentPanel;
    [SerializeField] private SkillSlot selectedSlot;
    [SerializeField] private TextMeshProUGUI selectedSlotLevel;
    [SerializeField] private TextMeshProUGUI selectedSlotTitle;
    [SerializeField] private TextMeshProUGUI selectedSlotContent;
    [SerializeField] private Button slotPurchaseBtn;
    [SerializeField] private TextMeshProUGUI purchasePriceTxt;
    private int purchaseSelectedIndx = 0;
    private SkillSlotBtn focusSlot;

    #region

    [Space]
    [SerializeField] private ScrollRect ownSkillsScroll;
    [SerializeField] private List<SkillSlotScrollBtn> ownSkillList;

    [Space]
    [SerializeField] private Color offFocusCol;
    [SerializeField] private Color onFocusCol;

    [SerializeField] private bool isStart = false;

    [Space]
    [SerializeField] private Button playBtn;

    public SkillSlotBtn FocusSlot
    {
        get => focusSlot;
        set
        {
            if (focusSlot != null) focusSlot.OutColor = offFocusCol;
            focusSlot = value;
            if(focusSlot != null)
            {
                focusSlot.OutColor = onFocusCol;
            }
        }
    }
    void Start()
    {
        slotMachineResetBtn.onClick.AddListener(OnClick_Reset);

        priceList = new List<int>(purchaseSlotBtns.Count);
        for(int i = 0, icount = purchaseSlotBtns.Count; i<icount; i++)
        {
            priceList.Add(0);
            var slot = purchaseSlotBtns[i];

            var _i = i;
            slot.AddListener_OnClick(() => { OnClick_SkillSlot(_i); });
        }

        slotPurchaseBtn.onClick.AddListener(OnClick_Purchase);

        if(ownSkillList.Count != 1)
        {
            Debug.LogError("useSkillList에는 사본 객체 하나만 필요합니다.");
        }
        else
        {
            ownSkillList[0].scollRect = ownSkillsScroll;
            ownSkillList[0].AddListener_OnClick(() => { OnClick_OwnSkillSlot(0); });
            ownSkillList[0].gameObject.SetActive(false);
        }

        isStart = true;

        playBtn.onClick.AddListener(OnClick_Play);

        Active();
    }

    public void Active()
    {
        if (!isStart) return;

        var GC = GameManager.GetGameController();
        if(GC != null)
        {
            currentLevelTxt.text = $"LV{GC.StageLevel - 1}";
        }

        SlotMachinePlay();
        OwnSkillsActiveAndUpdate();
        ResetPriceUpdate();
    }

    private void SlotMachinePlay()
    {
        var SC = GameManager.GetSkillController();
        var GC = GameManager.GetGameController();
        if(SC != null && GC != null)
        {
            var slotCount = purchaseSlotBtns.Count;

            if (slotCount < SC.SkillCount)
                SC.ShuffleSkills(slotCount, slotCount, SC.SkillCount);
            else SC.ShuffleSkills(SC.SkillCount, 0, SC.SkillCount);

            if (slotCount < SC.CompleteSkillCount)
                SC.ShuffleCompleteSkills(slotCount, slotCount, SC.CompleteSkillCount);
            else SC.ShuffleCompleteSkills(SC.CompleteSkillCount, 0, SC.CompleteSkillCount);


            var normal = 0;
            var complete = 0;
            slotMachineResetBtn.gameObject.SetActive(true);
            for (int i = 0; i < slotCount; i++)
            {
                if(normal >= SC.SkillCount)
                {
                    normal = 0;
                    SC.ShuffleSkills(SC.SkillCount, 0, SC.SkillCount);
                }
                if(complete >= SC.CompleteSkillCount)
                {
                    complete = 0;
                    SC.ShuffleCompleteSkills(SC.CompleteSkillCount, 0, SC.CompleteSkillCount);
                }

                purchaseSlotBtns[i].gameObject.SetActive(true);

                NSkill skill = new NSkill();
                if(Random.Range(0, 100) % completeSkillFreq == 1)
                {
                    //purchaseSlotBtns[i]
                    skill.skillData = SC.GetCompleteSkill(complete++);
                    if(GC.SkillBookInfo != null)
                    {
                        skill.level = GC.SkillBookInfo.GetCompleteLevel(skill.skillData);
                    }
                    else
                    {
                        skill.level = 0;
                    }
                }
                else
                {
                    skill.skillData = SC.GetSkill(normal++);
                    skill.level = GC.SkillTree.GetSkillLevel(skill.skillData) + 1;

                    if(!SkillAgent.ContainsAction(skill.skillData.name, skill.level))
                    {
                        normal -= 1;

                        skill.skillData = SC.GetCompleteSkill(complete++);
                        if (GC.SkillBookInfo != null)
                        {
                            skill.level = GC.SkillBookInfo.GetCompleteLevel(skill.skillData);
                        }
                        else
                        {
                            skill.level = 0;
                        }
                    }
                }

                purchaseSlotBtns[i].SkillInfo = skill;
                purchaseSlotBtns[i].OutColor = offFocusCol;
                
                if(GC.SkillBookInfo != null && skill.skillData.name != "보물상자")
                {
                    priceList[i] = GC.SkillBookInfo.GetRandomPrice();
                }
                else
                {
                    priceList[i] = 0;
                }
            }

            OnClick_SkillSlot(0);
        }
        else
        {
            slotMachineResetBtn.gameObject.SetActive(false);
            for(int i = 0, icount = purchaseSlotBtns.Count; i<icount; i++)
            {
                purchaseSlotBtns[i].gameObject.SetActive(false);
            }
        }
    }

    private void OwnSkillsActiveAndUpdate()
    {
        var GC = GameManager.GetGameController();
        if(GC != null && GC.SkillTree != null)
        {
            ownSkillsScroll.gameObject.SetActive(true);

            var skillTree = GC.SkillTree;
            var gap = skillTree.Count - ownSkillList.Count;

            if(gap > 0)
            {
                var count = ownSkillList.Count;
                for(int i = 0; i<gap; i++)
                {
                    var newOwnSkill = Instantiate(ownSkillList[0], ownSkillList[0].transform.parent);

                    var indx = i + count;
                    newOwnSkill.scollRect = ownSkillsScroll;
                    newOwnSkill.AddListener_OnClick(() => { OnClick_OwnSkillSlot(indx); });
                    ownSkillList.Add(newOwnSkill);
                }
            }
            else if(gap < 0)
            {
                for(int i = skillTree.Count, icount = i + gap; i<icount; i++)
                {
                    ownSkillList[i].gameObject.SetActive(false);
                }
            }
            

            for(int i = 0, icount = skillTree.Count; i<icount; i++)
            {
                ownSkillList[i].gameObject.SetActive(true);
                ownSkillList[i].OutColor = offFocusCol;
                ownSkillList[i].SetSkillInfo(skillTree.GetSkillData(i), skillTree.GetSkillLevel(i));
            }
        }
        else
        {
            ownSkillsScroll.gameObject.SetActive(false);
        }
    }

    private void ResetPriceUpdate()
    {
        var GC = GameManager.GetGameController();
        if (GC != null && GC.SkillBookInfo != null)
        {
            slotMachineResetBtn.gameObject.SetActive(true);
            resetPrice = GC.SkillBookInfo.GetRandomResetPrice();
            resetPriceTxt.text = $"-{GetResetPrice()}G";
        }
        else
        {
            slotMachineResetBtn.gameObject.SetActive(false);
        }
    } 

    private void OnClick_Reset()
    {
        var GM = GameManager.Instance;
        if (GM != null && GM.Inventory != null && GM.Inventory.Gold >= GetResetPrice())
        {
            GM.Inventory.Gold -= GetResetPrice();

            SlotMachinePlay();
            ResetPriceUpdate();
        }
        else WarningCall();
    }

    private void OnClick_Purchase()
    {
        var GM = GameManager.Instance;
        if (GM != null && GM.Inventory != null && GM.Inventory.Gold >= GetSkillPrice() && GM.gameController != null)
        {
            if (!GM.gameController.SkillTree.AddSkill(purchaseSlotBtns[purchaseSelectedIndx].SkillInfo.skillData))
            {
                if (!purchaseSlotBtns[purchaseSelectedIndx].SkillInfo.CompleteActive())
                {
                    Debug.LogError($"{purchaseSlotBtns[purchaseSelectedIndx].SkillInfo}는 존재하지 않습니다.");
                    return;
                }
            }

            GM.Inventory.Gold -= GetSkillPrice();
            purchaseSlotBtns[purchaseSelectedIndx].SkillInfo = null;
            OnClick_SkillSlot(purchaseSelectedIndx);
            OwnSkillsActiveAndUpdate();
        }
        else WarningCall();
    }
    #endregion
    private void OnClick_SkillSlot(int indx)
    {
        purchaseSelectedIndx = indx;

        var slot = purchaseSlotBtns[purchaseSelectedIndx];
        if(slot.SkillInfo != null)
        {
            contentPanel.SetActive(true);

            selectedSlot.SkillInfo = slot.SkillInfo;

            if (slot.SkillInfo.ContainsCompleteSkill())
            {
                selectedSlotLevel.gameObject.SetActive(false);
                selectedSlotContent.text = slot.SkillInfo.GetCompleteContent();
            }
            else
            {
                selectedSlotLevel.gameObject.SetActive(true);
                selectedSlotLevel.text = $"LV{slot.SkillInfo.level}";
                selectedSlotContent.text = slot.SkillInfo.GetContent();
            }

            selectedSlotTitle.text = slot.SkillInfo.skillData.name;
            slotPurchaseBtn.gameObject.SetActive(true);
            purchasePriceTxt.text = $"-{GetSkillPrice()}"; 
        }
        else
        {
            contentPanel.SetActive(false);
        }

        FocusSlot = slot;
    }

    private void OnClick_OwnSkillSlot(int indx)
    {
        purchaseSelectedIndx = -1;

        var slot = ownSkillList[indx];
        if (slot.SkillInfo != null)
        {
            if (slot.SkillInfo.ContainsCompleteSkill())
            {
                Debug.LogError($"{slot.SkillInfo.skillData} Complete한 스킬은 소유할 수 없습니다.");
                contentPanel.SetActive(false);
                return;
            }

            contentPanel.SetActive(true);

            selectedSlot.SkillInfo = slot.SkillInfo;
            selectedSlotLevel.gameObject.SetActive(true);
            selectedSlotLevel.text = $"LV{slot.SkillInfo.level}";
            selectedSlotContent.text = slot.SkillInfo.GetContent();

            selectedSlotTitle.text = slot.SkillInfo.skillData.name;
            slotPurchaseBtn.gameObject.SetActive(false);
        }
        else
        {
            contentPanel.SetActive(false);
        }

        FocusSlot = slot;
    }

    private void WarningCall()
    {
        var AC = AudioManager.GetAudioController();
        if (AC != null)
        {
            AC.PlaySFX(warningSFX);
        }
    }

    private int GetResetPrice()
    {
        return resetPrice;
    }
    private int GetSkillPrice()
    {
        if (purchaseSelectedIndx < 0) return 0;
        return priceList[purchaseSelectedIndx];
    }

    private void OnClick_Play()
    {
        gameObject.SetActive(false);
        var GC = GameManager.GetGameController();
        if(GC != null)
        {
            GC.OnReplay();
        }
    }
}
