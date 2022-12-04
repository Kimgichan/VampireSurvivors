using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillSlotScrollBtn : SkillSlotBtn, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public ScrollRect scollRect;

    public void OnDrag(PointerEventData eventData)
    {
        if(scollRect != null)
        {
            scollRect.OnDrag(eventData);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (scollRect != null)
        {
            isDrag = true;
            scollRect.OnBeginDrag(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (scollRect != null)
        {
            isDrag = false;
            scollRect.OnEndDrag(eventData);
        }
    }
}
