using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
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

    [Header("Stats")]
    public Team team;
    public Team opponentTeam;
    public int score;
    public PrisonZone prison;
    public CaptureZone captureZone;
    public TeamZone teamZone;

    [Space(25)]
    public PlayerController target;
    public List<PlayerController> controllers;
    public List<PlayerController> capturers;
    bool capturerAssigned;

    private void Start()
    {
        SpawnTeam();

        teamZone.Init(team);
        prison.Init(team);

        GameManager.Instance.controllers.Add(this);

        capturers = new List<PlayerController>();
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

            cur.GetComponent<PlayerController>().team = team;
            cur.GetComponent<PlayerController>().SetColor(GameManager.Instance.GetColorFromTeam(team));
            cur.GetComponent<PlayerController>().parentController = this;

            controllers.Add(cur.GetComponent<PlayerController>());
        }
    }

    public void UpdateCapturers()
    {
        if(capturers.Count <= 0)
        {
            //Add Two Capturers To The List
            int numberOfCapturers = 2;

            for (int i = 0; i < Mathf.Min(numberOfCapturers, controllers.Count); i++)
            {
                capturers.Add(controllers[i]);
            }
        }

        for(int i = 0; i < capturers.Count; i++)
        {
            if (capturers[i].currentState == AIState.Prison)
            {
                capturers.RemoveAt(i);

                //Get A New Capturer
                for (int j = 0; j < controllers.Count; j++)
                {
                    if (controllers[j].currentState != AIState.Prison && !IsCapturer(controllers[j]))
                    {
                        capturers.Add(controllers[i]);
                    }
                }
            }
        }

        for (int i = 0; i < capturers.Count; i++)
        {
            capturers[i].currentState = AIState.Capturing;
        }
    }


    public bool IsCapturer(PlayerController _controller)
    {
        for (int i = 0; i < capturers.Count; i++)
        {
            if(_controller == capturers[i])
            {
                return true;
            }
        }

        return false;
    }


    private void Update()
    {
        if(!GameManager.Instance.gameStarted)
        {
            for (int i = 0; i < controllers.Count; i++)
            {
                controllers[i].currentState = AIState.Idle;
            }
        }
        else
        {
            UpdateCapturers();

            for (int i = 0; i < controllers.Count; i++)
            {
                if (controllers[i].currentState != AIState.Prison)
                {
                    if (IsCapturer(controllers[i])) //Now check If The Player Is A "Capturer"
                    {
                        controllers[i].currentState = AIState.Capturing;
                    }
                    else if (controllers[i].currentState != AIState.Capturing)
                    {
                        if (controllers[i].target != null)
                        {
                            controllers[i].navAgent.SetDestination(controllers[i].target.transform.position);

                            continue;
                        }

                        //If Theres A Person Capturing Flag In Team Zone
                        PlayerController markedController = teamZone.GetClosestFreeMarkedPlayer(controllers[i].transform.position);

                        if (markedController != null)
                        {
                            controllers[i].target = markedController;
                            controllers[i].currentState = AIState.Chasing;
                            controllers[i].navAgent.SetDestination(markedController.transform.position);
                        }
                        else //Else Send Max 1 Or Two People To Go Capture Flag
                        {
                            controllers[i].currentState = AIState.Wandering;
                        }
                    }
                    else
                    {
                        controllers[i].currentState = AIState.Wandering;
                    }
                }
            }
        }

        for (int i = 0; i < controllers.Count; i++)
        {
            switch (controllers[i].currentState)
            {
                case AIState.Wandering:
                    SetWanderingState(controllers[i]);
                    break;
                case AIState.Idle:
                    break;
                case AIState.Prison:
                    SetPrisonState(controllers[i]);
                    break;
                case AIState.Capturing:
                    SetCaptureState(controllers[i]);
                    break;
                case AIState.Chasing:
                    SetChasingState(controllers[i]);
                    break;
                case AIState.Freeing:
                    SetFreeingState(controllers[i]);
                    break;
                default:
                    break;
            }
        }
    }

    public void SetWanderingState(PlayerController controller)
    {
        if (controller.navAgent.remainingDistance < 0.5f)
        {
            Vector3 dest = SampleNavmeshPoint(RandomInsideUnitCircle(transform.position, 10f));

            controller.navAgent.SetDestination(dest);
        }
    }

    public void SetCaptureState(PlayerController _controller)
    {
        if (!_controller.hasFlag)
        {
            _controller.navAgent.SetDestination(GetCaptureDestination());
        }
        else
        {
            _controller.navAgent.SetDestination(captureZone.transform.position);
        }
    }

    public void SetChasingState(PlayerController controller)
    {
        //Setting Target If It Does Not Exist
        if (!teamZone.MarkedPlayerExists(controller.target))
        {
            controller.target = null;
        }

        if (controller.target == null)
        {
            PlayerController newTarget = teamZone.GetClosestFreeMarkedPlayer(controller.transform.position);

            if (newTarget == null)
            {
                controller.currentState = AIState.Wandering;
                return;
            }

            controller.target = newTarget;
        }

        //Chasing Logic
        controller.navAgent.SetDestination(GetPursueDestination(controller, controller.target));

        //Within Touching Range
        if (controller.navAgent.remainingDistance <= 1.0f)
        {
            //Tag Target
            controller.target.inPrison = true;
            teamZone.RemoveMarkedPlayer(controller.target);
        }
    }

    public void SetPrisonState(PlayerController controller)
    {
        Vector3 dest = SampleNavmeshPoint(RandomInsideUnitCircle(transform.position, 10f));

        controller.navAgent.SetDestination(prison.transform.position);
    }

    public void SetFreeingState(PlayerController controller)
    {
        if(prison.GetPrisonerCount() <= 0)
        {
            controller.currentState = AIState.Wandering;
        }

        controller.navAgent.SetDestination(prison.transform.position);
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

    public Vector3 GetCaptureDestination()
    {
        TeamController teamToCapture;

        //to prevent do/while inf
        int iterations = 1000;

        do
        {
            teamToCapture = GameManager.Instance.controllers[Random.Range(0, GameManager.Instance.controllers.Count)];
        } while (teamToCapture.team == team && iterations > 0);

        //If Team Found Is Null, return a Default Location
        if (teamToCapture == null)
        {
            return Vector3.zero;
        }
        else
        {
            //return the found teams capture zone as a desitnation
            return teamToCapture.captureZone.transform.position;
        }
    }

    public Vector3 GetPursueDestination(PlayerController _pursuer, PlayerController _controller)
    {
        float distance = Vector3.Distance(_controller.transform.position, _pursuer.transform.position);

        float lookAhead = distance / _controller.moveSpeed;

        return _controller.transform.position + (_controller.velocity * lookAhead);
    }
}
