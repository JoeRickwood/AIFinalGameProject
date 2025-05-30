using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool gameStarted;

    private void Awake()
    {
        Instance = this;
    }

    public Color GetColorFromTeam(Team team)
    {
        switch (team)
        {
            case Team.Blue:
                return Color.blue;
            
            case Team.Green:
                return Color.green;
                
            default:
                return Color.white;
          
        }
    }
}
