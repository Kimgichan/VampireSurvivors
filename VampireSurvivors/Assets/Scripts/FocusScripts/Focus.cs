using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Focus : MonoBehaviour
{
    protected virtual void TurnOnActive()
    {
        GameManager.GetFocusController()?.AddFocus(this);
    }

    protected virtual void TurnOffActive()
    {
        GameManager.GetFocusController()?.RemoveFocus(this);
    }
}
