using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField] Color32 red;
    [SerializeField] Color32 blue;
    [SerializeField] Color32 yellow;
    [SerializeField] Color32 green;
    [SerializeField] Color32 black;
    [Header("Sprites")]
    [SerializeField] Sprite reverse;
    [SerializeField] Sprite skip;
    [SerializeField] Sprite plusTwo;
    [SerializeField] Sprite plusFour;
    [Header("Center Card")]
    [SerializeField] Image baseCardColor;//this is the main color of the card
    [SerializeField] Image imageCenter; //this is the color center of the card
    [SerializeField] Image valueImageCenter; //this is for reverse sprite or skip, ...
    [SerializeField] TMP_Text valueTextCenter;
    [SerializeField] GameObject wildImage;
    [SerializeField] Image topLeftCenter;
    [SerializeField] Image bottomLeftCenter;
    [SerializeField] Image topRightCenter;
    [SerializeField] Image bottomRightCenter;
    [Header("Top Left Corner")]
    [SerializeField] Image valueImageTL;
    [SerializeField] TMP_Text valueTextTL;
    [SerializeField] GameObject wildImageTL;
    [SerializeField] Image topLeftTL;
    [SerializeField] Image bottomLeftTl;
    [SerializeField] Image topRightTL;
    [SerializeField] Image bottomRightTL;
    [Header("Bottom Right Corner")]
    [SerializeField] Image valueImageBR;
    [SerializeField] TMP_Text valueTextBR;
    [SerializeField] GameObject wildImageBR;
    [SerializeField] Image topLeftBR;
    [SerializeField] Image bottomLeftBR;
    [SerializeField] Image topRightBR;
    [SerializeField] Image bottomRightBR;

    //void OnValidate()//For debuggin only
    //{
    //    SetAllColors(CardColor.GREEN);
    //}

    Card myCard;

    public void SetCard(Card card)
    {
        myCard = card;
        SetAllColors(card.cardColor);
    }

    void SetAllColors(CardColor cardColor)
    {
        switch (cardColor)
        {
            case CardColor.RED:
            {
                baseCardColor.color = red;
                imageCenter.color = red;
            }
            break;
            case CardColor.GREEN:
                {
                    baseCardColor.color = green;
                    imageCenter.color = green;
                }
                break;
            case CardColor.YELLOW:
                {
                    baseCardColor.color = yellow;
                    imageCenter.color = yellow;
                }
                break;
            case CardColor.BLUE:
                {
                    baseCardColor.color = blue;
                    imageCenter.color = blue;
                }
                break;
            case CardColor.NONE:
                {
                    baseCardColor.color = black;
                    imageCenter.color = black;

                    //wild cards
                    topLeftCenter.color = red;
                    topRightCenter.color = blue;
                    bottomLeftCenter.color = green; 
                    bottomRightCenter.color = yellow;

                    topLeftTL.color = red;
                    topRightTL.color = blue;
                    bottomLeftTl.color = green;
                    bottomRightTL.color = yellow;

                    topLeftBR.color = red;
                    topRightBR.color = blue;
                    bottomLeftBR.color = green;
                    bottomRightBR.color = yellow;

                }
                break;
        }
    }
}
