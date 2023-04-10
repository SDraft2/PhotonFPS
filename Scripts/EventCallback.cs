using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventCallback : EventTrigger
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        print(eventData.selectedObject);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        print(eventData.selectedObject);
    }
}
