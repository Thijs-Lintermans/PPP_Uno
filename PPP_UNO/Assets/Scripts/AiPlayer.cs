using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPlayer : Player
{
    public AiPlayer(string name) : base(name, false)
    {

    }

    //Logic How AI is going to play or draw or decide turns
    public override void TakeTurn(Card topCard, CardColor topColor)
    {

        Card cardToPlay = null;
        //AI should decide what to play
        Debug.Log(playerName + " takes a turn");

        //step 1: store info about playable cards based on the top card
        List<Card> playableCards = GetPlayableCards(topCard, topColor);

        //step 2: decide what card to play
        if(playableCards.Count > 0)
        {
            cardToPlay = ChooseBestCard(playableCards);
        }
        else
        {
            //Draw a card and see if it would be playable
            GameManager.Instance.DrawCardFromDeck();
            playableCards = GetPlayableCards(topCard, topColor);
            if(playableCards.Count > 0 )
            {
                cardToPlay = ChooseBestCard(playableCards);
            }
        }
        //step 3: if can't play
        if(cardToPlay == null)
        {
            //switch player
            GameManager.Instance.SwitchPlayer();

        }
        else//step 4: play choosen card
        {
            GameManager.Instance.PlayCard(null, cardToPlay);
            //if wild card choose best color

            //switchplayer
            GameManager.Instance.SwitchPlayer();
        }

    }

    List<Card> GetPlayableCards(Card topCard, CardColor topColor)
    {
        List<Card> playableCards = new List<Card>();

        foreach(Card card in playerHand)
        {
            if(card.cardColor == topColor || card.cardValue == topCard.cardValue || card.cardColor == CardColor.NONE)
            {
                playableCards.Add(card);
            }
        }

        return playableCards;
    }

    Card ChooseBestCard(List<Card> playableCards)
    {
        Card bestActionCard = null;
        Card bestRegularCard = null;
        Card bestWildCard = null;
        int nextPlayerHandSize = GameManager.Instance.GetNextPlayerHandSize();

        //best action cards
        foreach(Card card in playableCards)
        {
            if(card.cardValue == CardValue.WILD_DRAW_FOUR)
            {
                if(nextPlayerHandSize <= 2 || bestActionCard == null)
                {
                    bestActionCard = card;
                }
            }
            else if (card.cardValue == CardValue.DRAW_TWO)
            {
                if (nextPlayerHandSize <= 2 || bestActionCard == null)
                {
                    bestActionCard = card;
                }
            }
            else if (card.cardValue == CardValue.SKIP)
            {
                if (nextPlayerHandSize <= 2 || bestActionCard == null)
                {
                    bestActionCard = card;
                }
            }
            else if (card.cardValue == CardValue.REVERSE)
            {
                if (nextPlayerHandSize <= 2 || bestActionCard == null)
                {
                    bestActionCard = card;
                }
            }
            else if (card.cardValue == CardValue.WILD)
            {
                if (nextPlayerHandSize <= 2 || bestActionCard == null)
                {
                    bestWildCard = card;
                }
            }
        }
        //best regular cards
        foreach(Card card in playableCards)
        {
            if(bestRegularCard == null || card.cardValue > bestRegularCard.cardValue)
            {
                bestRegularCard = card;
            }
        }
        //no action card found
        if(bestActionCard == null && bestWildCard != null)
        {
            bestActionCard = bestWildCard;
        }
        // make a decision
        if(bestActionCard != null)
        {
            return bestActionCard;
        }
        if(bestRegularCard != null)
        {
            return bestRegularCard;
        }
        //default case should never be reached
        return playableCards[0];
    }
}
