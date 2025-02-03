using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    List<Card> cardDeck = new List<Card>();
    // Start is called before the first frame update
    void Start()
    {
        InitializeDeck();
    }

    public void InitializeDeck()
    {
        cardDeck.Clear();//empty the existing deck

        foreach (CardColor color in System.Enum.GetValues(typeof(CardColor)))
        {
            foreach(CardValue cardValue in System.Enum.GetValues(typeof(CardValue)))
            {
                if(color != CardColor.NONE && cardValue != CardValue.WILD && cardValue != CardValue.WILD_DRAW_FOUR)
                {
                    cardDeck.Add(new Card(color, cardValue));
                    cardDeck.Add(new Card(color, cardValue));//Two cards of each card
                }
            }
        }

        for(int i = 0; i < 4; i++)
        {
            cardDeck.Add(new Card(CardColor.NONE, CardValue.WILD));
            cardDeck.Add(new Card(CardColor.NONE, CardValue.WILD_DRAW_FOUR));
        }

        ShuffleCardDeck();
    }

    public void ShuffleCardDeck()
    {
        //Fisher Yates
        for(int i =0; i < cardDeck.Count; i++)
        {
            Card temp = cardDeck[i];
            int randomIndex = Random.Range(0, cardDeck.Count);
            cardDeck[i] = cardDeck[randomIndex];
            cardDeck[randomIndex] = temp;
        }
    }
}
