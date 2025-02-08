using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WildButton : MonoBehaviour, IPointerClickHandler
{
    public CardColor cardrColor;
    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.Instance.ChosenColor(cardrColor);
    }

    public void SetImageColor(Color32 color)
    {
        GetComponent<Image>().color = color;
    }
}
