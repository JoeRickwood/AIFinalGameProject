using System.Collections.Generic;
using UnityEngine;

public class PrisonZone : MonoBehaviour
{
    public float prisonTimer;

    public new MeshRenderer renderer;
    Material material;
    public Team team;

    List<PlayerController> prisonerList;

    public void Init(Team _team)
    {
        team = _team;

        prisonerList = new List<PlayerController>();
    }

    private void Start()
    {
        material = new Material(renderer.material);
        renderer.material = material;
        material.color = GameManager.Instance.GetColorFromTeam(team);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if(player == null)
        {
            return;
        }

        if (player != null && player.team == team)
        {
            other.GetComponent<PlayerController>().insidePrisonZone = true;
        }

        //If The Player has been Prisoner Tagged, Put Them On The List
        if(player.inPrison == true)
        {
            prisonerList.Add(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player == null)
        {
            return;
        }

        if (player != null && player.team == team)
        {
            other.GetComponent<PlayerController>().insidePrisonZone = false;
        }
    }

    public void RemovePlayerFromPrison(PlayerController player)
    {
        prisonerList.Remove(player);
    }

    public int GetPrisonerCount()
    {
        return prisonerList.Count;   
    }
}
