using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
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
        DealCards();
        //start game
    }

    void InitializePlayers()
    {
        players.Clear();

        players.Add(new Player("Player1"));

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
}
