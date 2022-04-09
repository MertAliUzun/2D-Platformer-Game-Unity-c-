using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Movement movement;
    [SerializeField] private Transform player;
    [SerializeField] private float cameraDistance;
    [SerializeField] private float cameraSpeed;
    private float cameraForward;
    void Start()
    {
        movement.facingRight = true;
    }

    void Update()
    {
        handleCamera();
    }
    void handleCamera()
    {
        movement.faceDirection();
        transform.position = new Vector3(player.position.x + cameraForward, player.position.y, transform.position.z);
        if(movement.isGrounded && !movement.changedDirection)
        {
            if (movement.facingRight)
            {
                cameraForward = Mathf.Lerp(cameraForward, (cameraDistance * player.localScale.x), Time.deltaTime * cameraSpeed);
            }
            else
            {
                cameraForward = Mathf.Lerp(cameraForward, (cameraDistance * player.localScale.x * -1), Time.deltaTime * cameraSpeed);
            }
        }
    }
}
