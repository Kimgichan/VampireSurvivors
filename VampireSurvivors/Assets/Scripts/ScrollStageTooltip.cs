using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;

public class ScrollStageTooltip : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private HorizontalScrollSnap horizontalScrollSnap;
    [SerializeField] private ScrollViewSnepEmphasize scrollViewSnepEmphasize;
    [SerializeField] private List<BounceBox> stages;
    [SerializeField] private List<BounceBox> tooltips;
    [SerializeField] private float pictureVelocity;
    [SerializeField] private string tooltipOpen_sfxName;
    bool tooltipOpen = false;

    //private void Start()
    //{
    //    StartCoroutine(UpdateCor());
    //}

    //public void OnBeginDrag(PointerEventData eventData)
    //{
    //    tooltipOpen = true;

    //    for(int i = 0, icount = tooltips.Count; i<icount; i++)
    //    {
    //        tooltips[i].TurnOnInverseBounce();
    //    }

    //    for(int i = 0, icount = stages.Count; i<icount; i++)
    //    {
    //        stages[i].TurnOffBounce();
    //    }
    //    scrollViewSnepEmphasize.TurnOnResizeUpdate();
    //}

    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    for (int i = 0, icount = tooltips.Count; i < icount; i++)
    //    {
    //        tooltips[i].TurnOnBounce();
    //    }

    //    scrollRect.velocity

    //    scrollViewSnepEmphasize.TurnOffResizeUpdate();
    //    stages[horizontalScrollSnap.CurrentPage].TurnOnBounce();
    //}

    public void Update()
    {
        if(scrollRect.velocity.sqrMagnitude <= pictureVelocity * pictureVelocity)
        {
            if (!tooltipOpen) return;
            tooltipOpen = false;
            for (int i = 0, icount = tooltips.Count; i < icount; i++)
            {
                tooltips[i].TurnOnBounce();
            }

            scrollViewSnepEmphasize.TurnOffResizeUpdate();
            stages[horizontalScrollSnap.CurrentPage].TurnOnBounce();
            StageChangeSFX();
        }
        else
        {
            if (tooltipOpen) return;
            tooltipOpen = true;
            for (int i = 0, icount = tooltips.Count; i < icount; i++)
            {
                tooltips[i].TurnOnInverseBounce();
            }

            for (int i = 0, icount = stages.Count; i < icount; i++)
            {
                stages[i].TurnOffBounce();
            }
            scrollViewSnepEmphasize.TurnOnResizeUpdate();
            //StageChangeSFX();
        }
    }

    public void StageChangeSFX()
    {
        var AC = AudioManager.GetAudioController();
        if(AC != null)
        {
            AC.PlaySFX(tooltipOpen_sfxName);
        }
    }

    //private IEnumerator UpdateCor()
    //{
    //    var wait = new WaitForSeconds(0.25f);
    //    while (true)
    //    {
    //        Debug.Log(scrollRect.velocity.magnitude);
    //        yield return wait;
    //    }
    //}
}
