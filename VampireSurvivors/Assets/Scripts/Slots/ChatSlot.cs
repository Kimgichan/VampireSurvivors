using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using TMPro;

public class ChatSlot : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private string ccontent;
    [Button] private void Chat()
    {
        Write("???", ccontent);
    }
#endif

    [SerializeField] TextMeshProUGUI player;
    [SerializeField] TextMeshProUGUI content;
    [SerializeField] private float gap;


    public void Write(string player, string content)
    {
        if(this.player != null)
        {
            this.player.text = string.Format(@"{0}", player);
        } 

        if(this.content != null)
        {
            this.content.text = string.Format(@"{0}", content);

            StartCoroutine(WriteCor());
        }
    }

    private IEnumerator WriteCor()
    {
        yield return null;

        var rectTR = transform as RectTransform;
        var size = rectTR.sizeDelta;
        size.y = gap + this.content.GetPreferredValues().y;

        rectTR.sizeDelta = size;
    }
}
