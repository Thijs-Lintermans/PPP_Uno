using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPlayer : Player
{
    public AiPlayer(string name) : base(name, false)
    {

    }

    //Logic How AI is going to play or draw or decide turns
    public override void TakeTurn()
    {
        //AI should decide what to play
        Debug.Log("Ai takes a turn");
    }
}
