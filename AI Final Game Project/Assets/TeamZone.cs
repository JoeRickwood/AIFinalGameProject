using System.Collections.Generic;
using UnityEngine;

class MarkedPlayer
{
    public PlayerController controller;
    public bool taken;

    public MarkedPlayer(PlayerController _controller)
    {
        controller = _controller;
        taken = false;
    }
}

public class TeamZone : MonoBehaviour
{
    List<MarkedPlayer> controllers;
    Team team;

    public void Init(Team _team)
    {
        team = _team;

        controllers = new List<MarkedPlayer>();
    }

    public void AddMarkedPlayer(PlayerController _controller)
    {
        controllers.Add(new MarkedPlayer(_controller));
    }

    public void RemoveMarkedPlayer(PlayerController _controller)
    {
        for (int i = 0; i < controllers.Count; i++)
        {
            if (controllers[i].controller == _controller)
            {
                controllers.RemoveAt(i);
                return;
            }
        }
    }

    public bool MarkedPlayerExists(PlayerController _controller)
    {
        for (int i = 0; i < controllers.Count; i++)
        {
            if (controllers[i].controller == _controller)
            {
                return true;
            }
        }

        return false;
    }

    public int GetMarkedPlayerCount()
    {
        return controllers.Count;
    }

    public PlayerController GetClosestFreeMarkedPlayer(Vector3 pos)
    {
        if (controllers.Count <= 0)
        {
            return null;
        }

        float tmpDistance = 0f;
        float currentDistance = Mathf.Infinity;
        int currentIndex = -1;

        for (int i = 0; i < controllers.Count; i++)
        {
            if (controllers[i].taken)
            {
                continue;
            }


            tmpDistance = Vector3.Distance(pos, controllers[i].controller.transform.position);

            if(tmpDistance < currentDistance)
            {
                currentDistance = tmpDistance;
                currentIndex = i;
            }
        }

        if(currentIndex < 0)
        {
            return null;
        }

        controllers[currentIndex].taken = true;

        return controllers[currentIndex].controller;
    }

    public PlayerController GetFreeMarkedPlayer()
    {
        for (int i = 0; i < controllers.Count; i++)
        {
            if (controllers[i].taken)
            {
                continue;
            }

            return controllers[i].controller;
        }

        return null;
    }



    private void OnTriggerEnter(Collider other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();

        if(controller == null || controller.team == team)
        {
            return;
        }

        AddMarkedPlayer(controller);  
    }


    private void OnTriggerExit(Collider other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();

        if (controller == null || controller.team == team)
        {
            return;
        }

        RemoveMarkedPlayer(controller);
    }
}
