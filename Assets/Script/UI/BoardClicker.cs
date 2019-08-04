using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardClicker : MonoBehaviour,IPointerClickHandler
{
    public GameManager manager;
    public void OnPointerClick(PointerEventData eventData)
    {
        var p = new Vector2(eventData.position.x, eventData.position.y);
        p -= GameManager.REVISE;
    }
}
