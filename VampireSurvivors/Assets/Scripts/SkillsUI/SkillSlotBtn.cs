using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class SkillSlotBtn : SkillSlot, IPointerClickHandler
{
    [SerializeField] protected Image outBG;
    protected UnityAction clickEvent;

    [SerializeField] protected bool isDrag;

    public Color OutColor
    {
        get
        {
            if(outBG != null)
            {
                return outBG.color;
            }
            else
            {
                return Color.black;
            }
        }
        set
        {
            if(outBG != null)
            {
                outBG.color = value;
            }
        }
    }

    public void AddListener_OnClick(UnityAction call)
    {
        if (clickEvent != null)
        {
            clickEvent += call;
        }
        else clickEvent = call;
    }
    public void RemoveListener_OnClick(UnityAction call)
    {
        if(clickEvent != null)
        {
            clickEvent -= call;
        }
    }
    public void RemoveAllListener_OnClick()
    {
        clickEvent = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isDrag)
        {
            return;
        }
        Debug.Log("Å¬¸¯");
        if (clickEvent != null)
        {
            clickEvent();
        }
    }
}
