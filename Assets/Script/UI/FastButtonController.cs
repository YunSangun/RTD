using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FastButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler

{
    public float TimeScale = 4f;
    private bool downed = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!downed)
        {
            Time.timeScale = TimeScale;
            downed = true;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (downed)
        {
            Time.timeScale = 1f;
            downed = false;
        }
    }


}
