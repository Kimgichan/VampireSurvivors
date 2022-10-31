using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VirtualJoystick : MonoBehaviour
{
    [SerializeField] Image virtualJoystick;
    [SerializeField] CanvasScaler canvasScaler;
    [SerializeField] EventTrigger eventTrigger;
    [SerializeField] RectTransform joystick;
    [SerializeField] RectTransform handle;


    public UnityAction<BaseEventData> PointerDown;
    public UnityAction<BaseEventData> PointerUp;

    public UnityAction<Vector2> Drag;
    private Vector2 currentDragForce;
    private IEnumerator dragUpdateCor;
    private void Start()
    {
        joystick.gameObject.SetActive(false);

        var entry_pointerDown = new EventTrigger.Entry();
        entry_pointerDown.eventID = EventTriggerType.PointerDown;
        entry_pointerDown.callback.AddListener(PointDownEvent);
        eventTrigger.triggers.Add(entry_pointerDown);

        var entry_pointerUp = new EventTrigger.Entry();
        entry_pointerUp.eventID = EventTriggerType.PointerUp;
        entry_pointerUp.callback.AddListener(PointUpEvent);
        eventTrigger.triggers.Add(entry_pointerUp);

        var entry_drag = new EventTrigger.Entry();
        entry_drag.eventID = EventTriggerType.Drag;
        entry_drag.callback.AddListener(DragEvent);
        eventTrigger.triggers.Add(entry_drag);

    }

    private void OnDisable()
    {
        DragUpdateClear();
    }


    private void PointDownEvent(BaseEventData e)
    {
        if (!enabled) return;

        virtualJoystick.enabled = false;
        currentDragForce = Vector2.zero;
        handle.position = joystick.position;
        joystick.gameObject.SetActive(true);
        joystick.position = (e as PointerEventData).position;

        if (PointerDown != null)
            PointerDown(e);

        if (dragUpdateCor == null)
            dragUpdateCor = DragUpdateCor();

        StartCoroutine(dragUpdateCor);
    }

    private void DragEvent(BaseEventData e)
    {
        if (!enabled) return;

        var dir = (e as PointerEventData).position - (Vector2)joystick.position;
        var length = dir.magnitude;
        var maxRange = joystick.rect.height * (Screen.height / canvasScaler.referenceResolution.y) * 0.35f;
        if (length > maxRange)
        {
            dir = (dir / length) * maxRange;
        }

        handle.position = dir + (Vector2)joystick.position;

        currentDragForce = dir / maxRange;
    }

    private void PointUpEvent(BaseEventData e)
    {
        if (!enabled) return;


        virtualJoystick.enabled = true;
        joystick.gameObject.SetActive(false);
        if (PointerUp != null)
            PointerUp(e);

        DragUpdateClear();
    }

    private void DragUpdateClear()
    {
        if (dragUpdateCor != null)
        {
            StopCoroutine(dragUpdateCor);
            dragUpdateCor = null;
        }
    }

    private IEnumerator DragUpdateCor()
    {
        while (true)
        {
            yield return null;
            if (Drag != null)
            {
                Drag(currentDragForce);
            }
        }
    }
}
