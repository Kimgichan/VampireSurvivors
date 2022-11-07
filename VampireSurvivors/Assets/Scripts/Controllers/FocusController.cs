using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusController : MonoBehaviour
{
    [SerializeField] private FocusData focusData;
    [SerializeField] private List<Focus> useFocusList;
    [SerializeField] private int capacity;
    [SerializeField] private int length;
    [SerializeField] private float focusScale;
    [SerializeField] private float speed;

    private IEnumerator activeCor;


    public float FocusScale => focusScale;
    public FocusData FocusData => focusData;

    private IEnumerator Start()
    {
        while (GameManager.Instance == null)
            yield return null;

        if(GameManager.Instance.focusController != null)
        {
            Destroy(gameObject);
            yield break;
        }

        GameManager.Instance.focusController = this;
        useFocusList.Capacity = capacity;
    }

    public void AddFocus(Focus focus)
    {
        if (focus == null) return;
        if (useFocusList.Contains(focus)) return;


        if(activeCor == null)
        {
            TurnOnActive();
        }

        length += 1;
        for(int i = 0, icount = useFocusList.Count; i<icount; i++)
        {
            if(useFocusList[i] == null)
            {
                useFocusList[i] = focus;
                return;
            }
        }

        useFocusList.Add(focus);
    }
    public void RemoveFocus(Focus focus)
    {
        if (focus == null) return;
        
        for(int i = 0; i<length; i++)
        {
            if(useFocusList[i] == (object)focus)
            {
                length -= 1;
                useFocusList[i] = useFocusList[length];
                useFocusList[length] = null;

                if(length < 1)
                {
                    TurnOffActive();
                }
                return;
            }
        }
    }

    private void TurnOnActive()
    {
        if(activeCor != null)
        {
            StopCoroutine(activeCor);
        }

        activeCor = ActiveCor();
        StartCoroutine(activeCor);
    }

    private void TurnOffActive()
    {
        if(activeCor != null)
        {
            StopCoroutine(activeCor);
            activeCor = null;
        }
    }

    private IEnumerator ActiveCor()
    {
        var maxTurn = false;
        focusScale = FocusData.MaxScale;

        while (true)
        {
            yield return null;
            //var TSC = GameManager.GetTimeScaleController();
            //if (TSC == null) continue;

            if (maxTurn) // 확장
            {
                var TSC = GameManager.GetTimeScaleController();
                if (TSC != null)
                {
                    focusScale += TSC.GameTimeScaleUpdate * speed;
                }
                else
                    focusScale += Time.deltaTime * speed;

                var max = FocusData.MaxScale;
                if(focusScale >= max)
                {
                    focusScale = max;
                    maxTurn = false;
                }
            }
            else // 축소
            {
                var TSC = GameManager.GetTimeScaleController();
                if (TSC != null)
                {
                    focusScale -= TSC.GameTimeScaleUpdate * speed;
                }
                else
                    focusScale -= Time.deltaTime * speed;

                var min = FocusData.MinScale;
                if(focusScale <= min)
                {
                    focusScale = min;
                    maxTurn = true;
                }
            }
        }
    }
}
