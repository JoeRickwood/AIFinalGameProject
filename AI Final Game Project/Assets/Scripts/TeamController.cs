using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TeamController : MonoBehaviour
{
    [Header("Spawning")]
    public GameObject playerPrefab;
    [Space(15)]
    public Transform spawnPoint;
    public int teamPlayerCount;
    public float spawnSpacing = 1f;
    public Color teamColor;

    [Space(25)]
    public List<PlayerController> controllers;

    private void Start()
    {
        SpawnTeam();
    }

    public void SpawnTeam()
    {
        //Reset The Current Controllers In The Scene
        for (int i = 0; i < controllers.Count; i++)
        {
            Destroy(controllers[i].gameObject);
        }

        //Spawn New Controllers In
        for (int i = 0; i < teamPlayerCount; i++)
        {
            float startDistance = -((float)(teamPlayerCount - 1) / 2f);

            Vector3 position = spawnPoint.position + (spawnPoint.right * (startDistance + i) * spawnSpacing);

            GameObject cur = Instantiate(playerPrefab, position, Quaternion.identity);

            cur.GetComponent<PlayerController>().SetColor(teamColor);

            controllers.Add(cur.GetComponent<PlayerController>());
        }
    }


    private void Update()
    {
        if(GameManager.Instance.gameStarted)
        {
            SetWanderingStates();
        }
        else
        {
            SetStartingPositions();
        }     
    }

    public void SetWanderingStates()
    {
        for (int i = 0; i < controllers.Count; i++)
        {
            if (controllers[i].navAgent.remainingDistance < 0.5f)
            {
                Vector3 dest = SampleNavmeshPoint(RandomInsideUnitCircle(transform.position, 10f));

                controllers[i].navAgent.SetDestination(dest);
            }
        }
    }

    public void SetStartingPositions()
    {
        float startDistance = -((float)(controllers.Count - 1) / 2f);

        for (int i = 0; i < controllers.Count; i++)
        {
            Vector3 pos = spawnPoint.position + (spawnPoint.right * (startDistance + i) * spawnSpacing);

            controllers[i].navAgent.SetDestination(pos);
        }
    }

    public Vector3 SampleNavmeshPoint(Vector3 pos)
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(pos, out hit, 10f, 1);
        Vector3 finalPosition = hit.position;

        return finalPosition;
    }

    public Vector3 RandomInsideUnitCircle(Vector3 initialPos, float distance)
    {
        float angle = Random.Range(0f, 360f);

        return (new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * distance) + initialPos;
    }
}
