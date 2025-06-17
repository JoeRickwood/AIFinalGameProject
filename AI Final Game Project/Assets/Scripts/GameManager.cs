using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool gameStarted;

    public List<TeamController> controllers;

    private void Awake()
    {
        Instance = this;

        controllers = new List<TeamController>();
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

    public TeamController GetTeamControllerFromTeam(Team _team)
    {
        for (int i = 0; i < controllers.Count; i++)
        {
            if (controllers[i].team == _team)
            {
                return controllers[i];  
            }
        }

        return null;
    }
}
