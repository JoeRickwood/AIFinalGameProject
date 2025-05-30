using Unity.VisualScripting;
using UnityEngine;

public enum CameraType
{
    Default,
    Extended
}

public class SpectatorController : MonoBehaviour
{
    public bool active = true;
    [Space(25)]
    public Camera connected;
    public CameraType type;
    [Space(10)]
    [Header("Rotation")]
    public Transform rotationTransform;
    public float mouseSensitivity = 1.0f;
    [Space(25)]
    [Header("Movement")]
    public float movementSpeed = 5f;

    Vector3 velocity;
    float rotX = 0.0f;
    float rotY = 0.0f;
    [HideInInspector]public PlayerController controlledPlayer;
    private void Update()
    {
        if (!active)
        {
            connected.enabled = false;
            return;
        }
        else
        {
            connected.enabled = true;
        }

        switch (type)
        {
            case CameraType.Default:
                connected.transform.localPosition = Vector3.Lerp(connected.transform.localPosition, new Vector3(0f, 0f, 0f), Time.deltaTime * 10f);
                break;
            case CameraType.Extended:
                connected.transform.localPosition = Vector3.Lerp(connected.transform.localPosition, new Vector3(0f, 0f, -1f) * 5f, Time.deltaTime * 5f);
                break;
        }

        if (controlledPlayer != null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            transform.position = controlledPlayer.transform.position;
            transform.rotation = Quaternion.Euler(controlledPlayer.rotX, controlledPlayer.rotY, 0f);

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GainControlOfPlayer(null);
                ChangeCameraType(CameraType.Default);
            }
        }
        else
        {
            //Only Rotate And Move Camera When Right Clicking
            if (Input.GetMouseButton(1))
            {
                //Hide Cursor
                Cursor.lockState = CursorLockMode.Locked;

                rotX -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
                rotX = Mathf.Clamp(rotX, -90f, 90f);

                rotY += Input.GetAxisRaw("Mouse X") * mouseSensitivity;

                //Apply Rotation
                rotationTransform.rotation = Quaternion.Euler(rotX, rotY, 0.0f);

                //Assign Velocity
                velocity = rotationTransform.forward * Input.GetAxisRaw("Vertical") + rotationTransform.right * Input.GetAxisRaw("Horizontal") + Vector3.up * Input.GetAxisRaw("Lift");
                velocity.Normalize(); //Normalize The Vector

                transform.position += velocity * movementSpeed * Time.deltaTime;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;

                //Clicking Can Select A Object When The Camera Is Not In Rotation Mode
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;


                    //If Our Raycast Hits Something
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                    {
                        if (hit.transform.GetComponent<PlayerController>() != null)
                        {
                            GainControlOfPlayer(hit.transform.GetComponent<PlayerController>());
                            ChangeCameraType(CameraType.Extended);

                        }
                    }
                }
            }//Unhide Cursor
        }
    }

    public void ChangeCameraType(CameraType _changeType)
    {
        switch (_changeType)
        {
            case CameraType.Default:
                transform.position = connected.transform.position;
                connected.transform.localPosition = new Vector3(0f, 0f, 0f);
                rotX = rotationTransform.transform.eulerAngles.x;
                rotY = rotationTransform.transform.eulerAngles.y;
                break;
            default:
                break;
        }

        type = _changeType;
    }


    public void GainControlOfPlayer(PlayerController _controller)
    {
        if(controlledPlayer != null)
        {
            controlledPlayer.inputType = new AIInput();
        }

        controlledPlayer = _controller;

        if(controlledPlayer != null)
        {
            _controller.inputType = new PlayerInput();
        }
    }


}
