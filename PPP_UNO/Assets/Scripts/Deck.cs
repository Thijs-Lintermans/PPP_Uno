using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Deck : MonoBehaviour, IPointerClickHandler
{
    List<Card> cardDeck = new List<Card>();
    List<Card> usedCardDeck = new List<Card>();
    // Start is called before the first frame update
    //void Start()
    //{
    //    InitializeDeck();
    //}

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

    public Card DrawCard()
    {
        if(cardDeck.Count == 0)
        {
            //deck is empty shuffle in used cards
            cardDeck.AddRange(usedCardDeck);
            usedCardDeck.Clear();
            ShuffleCardDeck();
            //check for deck size again > return null
            if(cardDeck.Count == 0)
            {
                return null;
            }
            //message no cards drawable
            return null;
        }

        Card drawnCard = cardDeck[0];
        cardDeck.RemoveAt(0);

        return drawnCard;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.humanHasTurn && !GameManager.Instance.CanPlayAnyCard())
        {
            GameManager.Instance.DrawCardFromDeck();
        }
    }

    //add used cards to a used card list
    public void AddUsedCard(Card card)
    {
        usedCardDeck.Add(card);
    }
}
