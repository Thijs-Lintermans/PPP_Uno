using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public string playerName;
    public List<Card> playerHand;

    public Player(string name)
    {
        playerName = name;
        playerHand = new List<Card>();
    }

    public void DrawCard(Card card)
    {
        playerHand.Add(card);
    }

    public void PlayCard(Card card)
    {
        playerHand.Remove(card);
    }
}
