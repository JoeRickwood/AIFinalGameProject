using UnityEngine;
using UnityEngine.AI;

public enum Team
{
    Blue,
    Green
}

public interface IControllerInput
{
    public Vector2 GetInput(PlayerController obj);
    public void UpdateRotation(PlayerController obj, ref float rotX, ref float rotY);
}

public class PlayerInput : IControllerInput
{
    public Vector2 GetInput(PlayerController obj)
    {
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    public void UpdateRotation(PlayerController obj, ref float rotX, ref float rotY)
    {
        rotX -= Input.GetAxis("Mouse Y");
        rotX = Mathf.Clamp(rotX, -90, 90);

        rotY += Input.GetAxis("Mouse X");
    }
}

public class AIInput : IControllerInput
{
    public Vector2 GetInput(PlayerController obj)
    {
        if(obj.navAgent.remainingDistance < 0.1f)
        {
            return new Vector2(0f, 0f);
        }

        Vector3 normalizedAgentVelocity = obj.GetComponent<NavMeshAgent>().velocity.normalized;

        return new Vector2(Vector3.Dot(obj.transform.right, normalizedAgentVelocity), Vector3.Dot(obj.transform.forward, normalizedAgentVelocity));
    }

    public void UpdateRotation(PlayerController obj, ref float rotX, ref float rotY)
    {
        Vector3 normalizedVelocity = obj.GetComponent<NavMeshAgent>().velocity.normalized;
        float rotation = Mathf.Atan2(normalizedVelocity.z, normalizedVelocity.x);

        rotX = 0f;
        rotY = Mathf.LerpAngle(obj.transform.eulerAngles.y, rotation, Time.deltaTime * 5f);
    }
}


public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    public CharacterController controller;
    public NavMeshAgent navAgent;
    public new MeshRenderer renderer;
    public Flag flag;


    public bool hasFlag;


    [Header("Stats")]
    public float moveSpeed = 5f;
    public Team team;
    public TeamController parentController;

    public IControllerInput inputType;
    Vector3 velocity = new Vector3(0f, 0f, 0f);
    Vector3 gravityVelocity = new Vector3(0f, 0f, 0f);
    [HideInInspector] public float rotX;
    [HideInInspector] public float rotY;


    private void Start()
    {
        inputType = new AIInput();

        navAgent.updatePosition = false;
        navAgent.updateRotation = false;
    }

    private void Update()
    {
        flag.gameObject.SetActive(hasFlag);

        inputType.UpdateRotation(this, ref rotX, ref rotY);
        Vector2 input = inputType.GetInput(this);

        velocity = (input.x * transform.right + input.y * transform.forward) * moveSpeed;

        if(controller.isGrounded)
        {
            gravityVelocity = new Vector3(0f, -1f, 0f);
        }else
        {
            gravityVelocity += Time.deltaTime * Vector3.down * 9.81f;
        }

        controller.Move(velocity * Time.deltaTime);
        controller.Move(gravityVelocity * Time.deltaTime);

        transform.rotation = Quaternion.Euler(0f, rotY, 0f);

        navAgent.nextPosition = transform.position;
    }

    public void Jump()
    {

    }

    public void SetColor(Color _col)
    {
        renderer.material = new Material(renderer.material);
        renderer.material.color = _col;
    }

    public void CaptureFlag()
    {
        hasFlag = true;
    }
}
