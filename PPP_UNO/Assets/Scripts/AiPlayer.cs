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
        if (cardToPlay == null)
        {
            //switch player
            //GameManager.Instance.SwitchPlayer();
            GameManager.Instance.AiSwitchPlayer();

        }
        else//step 4: play choosen card
        {
            if(playerHand.Count == 2)
            {
                GameManager.Instance.SetUnoByAi();
                //message
                Debug.Log(playerName + " has called uno");
            }
            GameManager.Instance.PlayCard(null, cardToPlay);
            //if wild card choose best color
            if(cardToPlay.cardColor == CardColor.NONE)
            {
                GameManager.Instance.ChosenColor(SelectBestColor());
            }

            if (cardToPlay.cardValue == CardValue.SKIP)
            {
                return;
            }

            //switchplayer
            //GameManager.Instance.SwitchPlayer();
            GameManager.Instance.AiSwitchPlayer();
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

    CardColor SelectBestColor()
    {
        Dictionary<CardColor, int> colorCounts = new Dictionary<CardColor, int> 
        {
            {CardColor.RED, 0 },
            {CardColor.BLUE, 0 },
            {CardColor.GREEN, 0 },
            {CardColor.YELLOW, 0 }
        };

        foreach(Card card in playerHand)
        {
            if(card.cardColor != CardColor.NONE)
            {
                colorCounts[card.cardColor]++;
            }
        }

        CardColor bestColor = CardColor.RED;
        int maxCount = 0;
        foreach(var color in colorCounts)
        {
            if(color.Value > maxCount)
            {
                bestColor = color.Key;
                maxCount = color.Value;
            }
        }

        return bestColor;
    }
}
