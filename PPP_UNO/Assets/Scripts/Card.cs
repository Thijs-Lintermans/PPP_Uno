using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardColor
{
    RED,
    GREEN,
    BLUE,
    YELLOW,
    NONE
}

public enum CardValue
{
    ZERO,
    ONE,
    TWO,
    THREE,
    FOUR,
    FIVE,
    SIX,
    SEVEN,
    EIGHT,
    NINE,
    SKIP,
    REVERSE,
    DRAW_TWO,
    WILD,
    WILD_DRAW_FOUR
}

//[System.Serializable]
public class Card
{
    public CardColor cardColor;
    public CardValue cardValue;

    public Card(CardColor color, CardValue value)
    {
        this.cardColor = color;
        this.cardValue = value;
    }   
}
