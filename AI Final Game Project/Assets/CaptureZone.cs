using UnityEngine;

public class CaptureZone : MonoBehaviour
{
    public new MeshRenderer renderer;
    public Team team;
    Material material;

    private void Start()
    {
        material = new Material(renderer.material);
        renderer.material = material;
        material.color = GameManager.Instance.GetColorFromTeam(team);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>())
        {
            if(team != other.GetComponent<PlayerController>().team)
            {
                other.GetComponent<PlayerController>().CaptureFlag();
                other.GetComponent<PlayerController>().flag.UpdateColor(GameManager.Instance.GetColorFromTeam(team));
            }else if(other.GetComponent<PlayerController>().hasFlag)
            {
                other.GetComponent<PlayerController>().hasFlag = false;
                other.GetComponent<PlayerController>().parentController.score++;
            }
        }
    }
}
