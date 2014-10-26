using System;
using UnityEngine;
using System.Collections;
using Voxel;


public class GameManager : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        BlockGame game = new BlockGame();
        game.Load();
        game.Start();
    }
}
