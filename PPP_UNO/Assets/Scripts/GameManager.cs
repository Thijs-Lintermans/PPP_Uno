using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<Player> players = new List<Player>();
    [SerializeField] Deck deck;

    [SerializeField] Transform playerHandTransform; //This holds later the player cards
    [SerializeField] List<Transform> aiHandTransforms = new List<Transform>();

    [SerializeField] GameObject cardPrefab;

    [SerializeField] int numberOfAiPlayers = 3;
    [SerializeField] int startingHandSize = 7;
    int currentPlayer = 0;
    int playDirection = 1;//1 // -1
    [Header("Gameplay")]
    [SerializeField] Transform discardPileTransform;
    [SerializeField] CardDisplay topCard;
    [SerializeField] Transform directionArrow;
    public bool humanHasTurn { get; private set; }
 
    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //Initialize new deck of cards
        deck.InitializeDeck();
        //initialize players
        InitializePlayers();
        //deal cards
        StartCoroutine(DealStartingCards());
        //start game
    }

    void InitializePlayers()
    {
        players.Clear();

        players.Add(new Player("Player1", true));

        for (int i = 0; i < numberOfAiPlayers; i++)
        {
            players.Add(new AiPlayer("AI " +  (i+1)));
        }
    }

    void DealCards()
    {
        for (int i = 0; i < startingHandSize; i++)
        {
            foreach(Player player in players)
            {
                player.DrawCard(deck.DrawCard());
            }
        }

        //display the player hand

        //display ai hands
    }

    IEnumerator DealStartingCards()
    {
        for (int i = 0; i < startingHandSize; i++)
        {
            foreach (Player player in players)
            {
                Card drawnCard = deck.DrawCard();
                player.DrawCard(drawnCard);

                //visualise cards
                Transform hand = player.IsHuman ? playerHandTransform : aiHandTransforms[players.IndexOf(player)-1];
                GameObject card = Instantiate(cardPrefab, hand, false);

                //Draw card correctly
                CardDisplay cardDisplay = card.GetComponentInChildren<CardDisplay>();
                cardDisplay.SetCard(drawnCard, player);//only for human

                // for AI players
                if (player.IsHuman)
                {
                    //show the back side
                    cardDisplay.ShowCard();
                }

                yield return new WaitForSeconds(0.1f);
            }
        }
        //hand out top card
        Card pileCard = deck.DrawCard();
        GameObject newCard = Instantiate(cardPrefab);
        MoveCardToPile(newCard); 
        CardDisplay display = newCard.GetComponentInChildren<CardDisplay>();
        display.SetCard(pileCard, null);
        display.ShowCard();
        newCard.GetComponentInChildren<CardInteraction>().enabled = false;
        //set top card
        topCard = display;

        //start the game
        Debug.Log("the game is allowed to start");
        humanHasTurn = true;
    }

    public void PlayCard(CardDisplay cardDisplay)
    {
        Card cardToPlay = cardDisplay.MyCard;
        //check if card is even playable - specially for human
        if (!IsPlayable(cardDisplay.MyCard))
        {
            Debug.Log("this card is not playable");
            return;
        }

        //Remove the player hand
        players[currentPlayer].PlayCard(cardToPlay);
        //unhide the card if its an ai player
        //move the card object to discard pile
        MoveCardToPile(cardDisplay.transform.parent.gameObject);
        //update top card
        topCard = cardDisplay;
        //implement possible logic: what should happen based card played/effects
        OnCardPlayed(topCard.MyCard);
    }

    void MoveCardToPile(GameObject currentCard)
    {
        currentCard.transform.SetParent(discardPileTransform);
        currentCard.transform.localPosition = Vector3.zero;
        currentCard.transform.localScale = Vector3.one;

        RectTransform cardRect = currentCard.GetComponent<RectTransform>();
        RectTransform pileRect = discardPileTransform.GetComponent<RectTransform>();

        cardRect.sizeDelta = pileRect.sizeDelta;

        //unhide the card
    }

    void OnCardPlayed(Card playedCard)
    {
        //Do all effects needed
        ApplyCardEffects(playedCard);
        //check if player has won > return


        SwitchPlayer();
    }

    public void DrawCardFromDeck()
    {
        Card drawnCard = deck.DrawCard();
        Player player = players[currentPlayer];

        if (drawnCard != null)
        {
            player.DrawCard(drawnCard);

            //visualise cards
            Transform hand = player.IsHuman ? playerHandTransform : aiHandTransforms[players.IndexOf(player) - 1];
            GameObject card = Instantiate(cardPrefab, hand, false);

            //Draw card correctly
            CardDisplay cardDisplay = card.GetComponentInChildren<CardDisplay>();
            cardDisplay.SetCard(drawnCard, player);//only for human

            // for AI players
            if (player.IsHuman)
            {
                //show the back side
                cardDisplay.ShowCard();
            }
        }
    }

    void SwitchPlayer()
    {
        humanHasTurn = false;
        currentPlayer += playDirection;

        if (currentPlayer > players.Count)
        {
            currentPlayer = 0;  
        }
        else if(currentPlayer < 0)
        {
            currentPlayer = players.Count - 1;
        }

        if (players[currentPlayer].IsHuman)
        {
            humanHasTurn = true;
        }
        else//AI player
        {
            //Do ai stuff time base

        }
    }

    public bool CanPlayAnyCard()
    {
        foreach (Card card in players[currentPlayer].playerHand)
        {
            if (IsPlayable(card))
            {
                return true;
            }
        }
        return false;
    }

    bool IsPlayable(Card card)
    {
        return card.cardColor == topCard.MyCard.cardColor || card.cardValue == topCard.MyCard.cardValue ||
            card.cardColor == CardColor.NONE;
    }

    //apply special card effects
    void ApplyCardEffects(Card playedCard)
    {
        switch (playedCard.cardValue)
        {
            case CardValue.SKIP:
                SkipPlayer();
                break;
            case CardValue.REVERSE:
                ReversePlayOrder();
                break;
            case CardValue.DRAW_TWO:
                MakeNextPlayerDrawCards(2);
                break;
            case CardValue.WILD:
                ChooseNewColor();
                break;
            case CardValue.WILD_DRAW_FOUR:
                ChooseNewColor();
                MakeNextPlayerDrawCards(4);
                break;

            default:
                //There is no special effect yet
                break;
        }
    }

    //check win condition

    //end game

    //skip next player
    void SkipPlayer()
    {
        int numberOfPlayer = players.Count;

        currentPlayer = (currentPlayer + 2 * playDirection + numberOfPlayer) % numberOfPlayer;

        //message about what happens - feedback to the human player
    }

    //reverse
    void ReversePlayOrder()
    {
        playDirection *= -1;//1 * -1 = -1 // -1 * -1 = 1
        //visualise this effect by the arrow
        Vector3 scale = directionArrow.localScale;
        scale.x = playDirection;
        directionArrow.localScale = scale; 
        //switch turn to next player
    }

    //make next player draw cards > +2 / +4 / +1 Forgot uno button?
    void MakeNextPlayerDrawCards(int cardAmount)
    {
        int numberOfPlayer = players.Count;

        int nextPlayerIndex = (currentPlayer + playDirection + numberOfPlayer) % numberOfPlayer;
        Player player = players[nextPlayerIndex];
        for (int i = 0; i < cardAmount; i++)
        {
            Card drawnCard = deck.DrawCard();

            if (drawnCard != null)
            {
                player.DrawCard(drawnCard);

                //visualise cards
                Transform hand = player.IsHuman ? playerHandTransform : aiHandTransforms[players.IndexOf(player) - 1];
                GameObject card = Instantiate(cardPrefab, hand, false);

                //Draw card correctly
                CardDisplay cardDisplay = card.GetComponentInChildren<CardDisplay>();
                cardDisplay.SetCard(drawnCard, player);//only for human

                // for AI players
                if (player.IsHuman)
                {
                    //show the back side
                    cardDisplay.ShowCard();
                }
            }
        }
        //message of what happened
    }

    //choose new color
    void ChooseNewColor()
    {

    }
}
