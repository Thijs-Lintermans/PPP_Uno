using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    Color32 red;
    Color32 blue;
    Color32 yellow;
    Color32 green;
    Color32 black;
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
    [Header("Card Back")]
    [SerializeField] GameObject cardBack;

    //void OnValidate()//For debuggin only
    //{
    //    SetAllColors(CardColor.GREEN);
    //}

    Card myCard;
    public Card MyCard => myCard;
    Player cardOwner;
    public Player Owner => cardOwner;

    public void SetCard(Card card, Player owner)
    {
        var Colors = GameManager.Instance.GetColors();
        red = Colors.red;
        green = Colors.green;
        yellow = Colors.yellow;
        blue = Colors.blue;
        black = Colors.black;

        myCard = card;
        SetAllColors(card.cardColor);
        SetValue(card.cardValue);
        cardOwner = owner;
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

    void SetValue(CardValue cardValue)
    {
        //deactivate specials
        wildImage.SetActive(false);
        wildImageBR.SetActive(false);
        wildImageTL.SetActive(false);

        valueImageCenter.gameObject.SetActive(false);
        valueImageBR.gameObject.SetActive(false);
        valueImageTL.gameObject.SetActive(false);
        switch (cardValue)
        {
            case CardValue.SKIP:
                {
                    valueImageCenter.sprite = skip;
                    valueImageCenter.gameObject.SetActive(true);
                    valueImageBR.sprite = skip;
                    valueImageBR.gameObject.SetActive(true);
                    valueImageTL.sprite = skip;
                    valueImageTL.gameObject.SetActive(true);
                    valueTextCenter.text = "";
                    valueTextTL.text = "";
                    valueTextBR.text = "";
                }
                break;
            case CardValue.REVERSE:
                {
                    valueImageCenter.sprite = reverse;
                    valueImageCenter.gameObject.SetActive(true);
                    valueImageBR.sprite = reverse;
                    valueImageBR.gameObject.SetActive(true);
                    valueImageTL.sprite = reverse;
                    valueImageTL.gameObject.SetActive(true);
                    valueTextCenter.text = "";
                    valueTextTL.text = "";
                    valueTextBR.text = "";
                }
                break;
            case CardValue.DRAW_TWO:
                {
                    valueImageCenter.sprite = plusTwo;
                    valueImageCenter.gameObject.SetActive(true);
                    valueTextCenter.text = "";
                    valueTextTL.text = "+2";
                    valueTextBR.text = "+2";
                }
                break;
            case CardValue.WILD_DRAW_FOUR:
                {
                    valueImageCenter.sprite = plusFour;
                    valueImageCenter.gameObject.SetActive(true);
                    valueTextCenter.text = "";
                    valueTextTL.text = "+4";
                    valueTextBR.text = "+4";
                }
                break;
            case CardValue.WILD:
                {
                    wildImage.SetActive(true);
                    wildImageBR.SetActive(true);
                    wildImageTL.SetActive(true);
                    valueTextCenter.text = "";
                    valueTextTL.text = "";
                    valueTextBR.text = "";
                }
                break;
            default:
                {
                    valueTextCenter.text = ((int)cardValue).ToString();
                    valueTextTL.text = ((int)cardValue).ToString();
                    valueTextBR.text = ((int)cardValue).ToString();
                }
                break;
        }
    }

    public void ShowCard()
    {
        cardBack.SetActive(false);
    }

    public void HideCard()
    {
        cardBack.SetActive(true);
    }
}
