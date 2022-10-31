using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.timeScaleController == null) return;
        if (GameManager.Instance.gameController == null) return;

        var GC = GameManager.Instance.gameController;

        if(GC.Player != null)
        {
            var pos = Vector2.Lerp(transform.position, GC.Player.transform.position, 
                GameManager.Instance.timeScaleController.GameTimeScaleUpdate);
            transform.position = new Vector3(pos.x, pos.y, transform.position.z);
        }
    }
}
