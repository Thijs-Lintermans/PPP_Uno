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
}
