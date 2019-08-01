using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FastButtonController : MonoBehaviour, IPointerClickHandler

{
    public float TimeScale = 4f;
    private bool clicked = false;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (clicked)
        {
            Time.timeScale = 1f;
            clicked = false;
        }
        else
        {
            Time.timeScale = TimeScale;
            clicked = true;
        }
    }


}
