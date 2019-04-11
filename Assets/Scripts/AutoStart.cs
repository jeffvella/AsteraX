using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoStart : MonoBehaviour
{
    void Start()
    {
        // This is temporary while menus are not in place to start a level
        Game.StartGame();
    }
}
