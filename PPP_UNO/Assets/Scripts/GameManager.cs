using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] GameObject wildPanel;
    [SerializeField] WildButton redButton;
    [SerializeField] WildButton greenButton;
    [SerializeField] WildButton blueButton;
    [SerializeField] WildButton yellowButton;
    CardColor topColor = CardColor.NONE;
    bool unoCalled;
    [Header("Colors")]
    [SerializeField] Color32 red;
    [SerializeField] Color32 blue;
    [SerializeField] Color32 yellow;
    [SerializeField] Color32 green;
    [SerializeField] Color32 black;
    [Header("UI Elements")]
    [SerializeField] GameObject winPanel;
    [SerializeField] TMP_Text winningText;
    [SerializeField] List<Image> playerHighlights = new List<Image>();
    [SerializeField] List<TMP_Text> playerCardCounts = new List<TMP_Text>();
    [SerializeField] TMP_Text messageText;
    public bool humanHasTurn { get; private set; }
 
    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        wildPanel.SetActive(false);
        redButton.SetImageColor(red);
        greenButton.SetImageColor(green);
        blueButton.SetImageColor(blue);
        yellowButton.SetImageColor(yellow);
        winPanel.SetActive(false);
        //Initialize new deck of cards
        deck.InitializeDeck();
        //initialize players
        InitializePlayers();
        //deal cards
        StartCoroutine(DealStartingCards());
        //start game
        UpdateMessageBox("Welcome to UNO!");
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
        deck.AddUsedCard(pileCard);
        //set top card
        topCard = display;
        topColor = pileCard.cardColor;
        //pick a random color if we have a wild card played
        if(topColor == CardColor.NONE)
        {
            topColor = PickRandomColor();
        }
        TintArrow();

        //start the game
        Debug.Log("the game is allowed to start");
        UpdateMessageBox("Player 1, it's your turn.");
        humanHasTurn = true;
        UpdatePlayerUI();
    }

    CardColor PickRandomColor()
    {
        CardColor[] colors = (CardColor[])Enum.GetValues(typeof(CardColor));
        int randomIndex = UnityEngine.Random.Range(0, colors.Length-1);
        return colors[randomIndex];
    }

    public void PlayCard(CardDisplay cardDisplay = null, Card card = null)
    {
        Card cardToPlay = cardDisplay?.MyCard ?? card;
        //check if card is even playable - specially for human

        if(cardDisplay == null && card != null)
        {
            cardDisplay = FindCardDisplayForCard(card);
        } 

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
        topColor = cardToPlay.cardColor;
        TintArrow();
        //implement possible logic: what should happen based card played/effects
        OnCardPlayed(topCard.MyCard);
        //unhide card
        cardDisplay.ShowCard();
        //deactive interaction
        cardDisplay.GetComponent<CardInteraction>().enabled = false;
        //add the card back to the used cards deck
        deck.AddUsedCard(cardToPlay);
        //SwitchPlayer();
    }

    //find a carddisplay based on card
    CardDisplay FindCardDisplayForCard(Card card)
    {
        Player player = players[currentPlayer];
        Transform hand = player.IsHuman ? playerHandTransform : aiHandTransforms[players.IndexOf(player) - 1];

        foreach(Transform cardTransform in hand)
        {
            CardDisplay tempDisplay = cardTransform.GetComponentInChildren<CardDisplay>();
            if(tempDisplay.MyCard == card)
            {
                return tempDisplay;
            }
        }
        return null;
    }

    void MoveCardToPile(GameObject currentCard)
    {
        currentCard.transform.SetParent(discardPileTransform);
        currentCard.transform.localPosition = Vector3.zero;
        currentCard.transform.localScale = Vector3.one;

        RectTransform cardRect = currentCard.GetComponent<RectTransform>();
        RectTransform pileRect = discardPileTransform.GetComponent<RectTransform>();

        cardRect.sizeDelta = pileRect.sizeDelta;

        Quaternion pileRotation = discardPileTransform.rotation;

        float randomZRotation = UnityEngine.Random.Range(-10f, 10f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomZRotation);

        currentCard.transform.rotation = pileRotation * randomRotation; 

        //unhide the card
    }

    void OnCardPlayed(Card playedCard)
    {
        //Do all effects needed
        ApplyCardEffects(playedCard);
        //check if player has won > return


        if(playedCard.cardValue == CardValue.SKIP)
        {
            return;
        }
        if(playedCard.cardColor == CardColor.NONE && players[currentPlayer].IsHuman)
        {
            return ;
        }
        if (players[currentPlayer].IsHuman)
        {
            SwitchPlayer();
        }
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
            //see if we have a playable card > if not swithc player
            if (!CanPlayAnyCard() && player.IsHuman)
            {
                Debug.Log("No playable card drawn, go to next player");
                SwitchPlayer();
                //message here 
            }
        }
    }

    public void SwitchPlayer(bool skipNext = false)
    {
        humanHasTurn = false;

        int numberOfPlayer = players.Count;

        if (players[currentPlayer].playerHand.Count == 1 && !unoCalled)
        {
            //message - player forgot to call uno
            UpdateMessageBox("You forgot to press the UNO button, you get 2 cards.");
            for (int  i = 0; i < 2; i++)
            {
                DrawCardFromDeck();
            }
        }
        //check for win
        if (CheckWinCondition())
        {
            winPanel.SetActive(true);
            winningText.text = players[currentPlayer].playerName + " has won the game!";
            UpdateMessageBox(players[currentPlayer].playerName + " has won the game!");
            //Debug.Log("Game over!" + players[currentPlayer].playerName + "has won the game!");
            return;
        }

        if (skipNext)
        {
            currentPlayer = (currentPlayer + 2 * playDirection + numberOfPlayer) % numberOfPlayer;
        }
        else
        {
            currentPlayer = (currentPlayer + playDirection + numberOfPlayer) % numberOfPlayer;
        }

        //update UI for card amount and highlight active player
        UpdatePlayerUI();

        //reset unocalled
        unoCalled = false;

        if (players[currentPlayer].IsHuman)
        {
            UpdateMessageBox("Player 1, it's your turn!");
            humanHasTurn = true;
        }
        else//AI player
        {
            //Do ai stuff time base
            StartCoroutine(HandleAiTurn());
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
        return card.cardColor == topColor || card.cardValue == topCard.MyCard.cardValue ||
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
    bool CheckWinCondition()
    {
        if (players[currentPlayer].playerHand.Count == 0)
        {
            return true;
        }
        return false;
    }

    //end game

    //skip next player
    void SkipPlayer()
    {
        SwitchPlayer(true);//skip over the next player

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
        UpdateMessageBox(player.playerName + " has to draw " + cardAmount + " cards.");
    }

    //choose new color
    void ChooseNewColor()
    {
        if (players[currentPlayer].IsHuman)
        {
            UpdateMessageBox("Choose a color!");
            wildPanel.SetActive(true);
            return;
        }

    }

    public (Color32 red, Color32 green, Color32 yellow, Color32 blue, Color32 black) GetColors()
    {
        return (red, green, yellow, blue, black);
    }

    //update arrow color
    void TintArrow()
    {
        switch (topColor)
        {
            case CardColor.RED:
                directionArrow.GetComponent<Image>().color = red;
                break;
            case CardColor.YELLOW:
                directionArrow.GetComponent<Image>().color = yellow;
                break;
            case CardColor.GREEN:
                directionArrow.GetComponent<Image>().color = green;
                break;
            case CardColor.BLUE:
                directionArrow.GetComponent<Image>().color = blue;
                break;
            case CardColor.NONE:
                directionArrow.GetComponent<Image>().color = black;
                break;
        }
    }

    public void ChosenColor(CardColor newColor)
    {
        topColor = newColor;
        TintArrow();
        wildPanel.SetActive(false);
        if (players[currentPlayer].IsHuman)
        {
            SwitchPlayer();
        }
    }

    //AI turn
    IEnumerator HandleAiTurn()
    {
        UpdateMessageBox(players[currentPlayer].playerName + " has the turn!");
        yield return new WaitForSeconds(2f);

        players[currentPlayer].TakeTurn(topCard.MyCard, topColor);

        //wait a little again 
        //SwitchPlayer();
    }

    //get next player hand size
    public int GetNextPlayerHandSize()
    {
        int numberOfPlayer = players.Count;
        int nextPlayerIndex = (currentPlayer + playDirection + numberOfPlayer) % numberOfPlayer;
        int nextPlayerHandSize = players[nextPlayerIndex].playerHand.Count;
        return nextPlayerHandSize; 
    }

    //uno button call
    public void unoButton()
    {
        if (players[currentPlayer].playerHand.Count == 2)
        {
            unoCalled = true;
            //message for the player
            UpdateMessageBox("You called UNO! Good Luck");
        }
        else
        {
            //for penalty if pressed wrong
            UpdateMessageBox("UNO button clicked but not correctly!");
        }
    }

    public void SetUnoByAi()
    {
        UpdateMessageBox(players[currentPlayer].playerName + " has called UNO");
        unoCalled = true;
    }

    void UpdatePlayerUI()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if(i == currentPlayer)
            {
                playerHighlights[i].color = yellow;
            }
            else
            {
                playerHighlights[i].color = Color.white;
            }

            //Text card amount
            playerCardCounts[i].text = players[i].playerHand.Count.ToString();
        }
    }

    void UpdateMessageBox(string message)
    {
        messageText.text = message;
    }

    public void AiSwitchPlayer()
    {
        StartCoroutine(SwitchPlayerDelayed());
    }

    IEnumerator SwitchPlayerDelayed()
    {
        yield return new WaitForSeconds(2f);
        SwitchPlayer();
    }
}
