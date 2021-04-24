using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Working with
public class PlayerMove : MonoBehaviour
{
    public CharacterController playerController;
    public Transform playerBody;
    public Transform groundCheckPoint;
    public float moveSpeed;
    public float gravity;
    public float jumpHeight;
    public float groundCheckHeight;

    private float pushToGroundSpeed = 2;

    [SerializeField]
    Vector3 velocity;

    void Start()
    {
    }

    void Update()
    {
        bool isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckHeight);
        var forwardBackDelta = Input.GetAxis("Vertical") * playerBody.forward;
        var leftRightDelta = Input.GetAxis("Horizontal") * playerBody.right;
        var moveDirection = (forwardBackDelta + leftRightDelta).normalized;
        var groundMovement = moveDirection * moveSpeed * Time.deltaTime;

        if (isGrounded)
        {
            if (Mathf.Sign(velocity.y) == Mathf.Sign(gravity))
            {
                velocity.y = Mathf.Sign(gravity) * pushToGroundSpeed;
            }

            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = -Mathf.Sign(gravity) * Mathf.Sqrt(Mathf.Abs(jumpHeight * gravity * 2));
            }
        }

        velocity.y += gravity * Time.deltaTime;
        var fallMovement = velocity * Time.deltaTime;

        playerController.Move(fallMovement + groundMovement);

    }
}
