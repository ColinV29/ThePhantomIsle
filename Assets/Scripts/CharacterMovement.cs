using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 5;

    private CharacterController controller;

    private Vector2 movement;
    private Vector2 aim;

    private PlayerControls playerControls;
    private PlayerInput playerInput;
    private Animator playerAnimator;

    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
    private Direction facing = Direction.Down;

    private void Awake() {
        controller = GetComponent<CharacterController>();
        playerControls = new PlayerControls();
        playerInput = GetComponent<PlayerInput>();
        playerAnimator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void onDisable()
    {
        playerControls.Disable();
    }

    
    void Update()
    {
        HandleInput();
        HandleMovement();
        HandleRotation();
    }

    //Process Inputs
    void HandleInput()
    {
        movement = playerControls.Controls.Movement.ReadValue<Vector2>();
        aim = playerControls.Controls.Aim.ReadValue<Vector2>();
    }

    //Apply Movement
    void HandleMovement()
    {
        Vector3 move = new Vector3(movement.x, movement.y, 0);
        controller.Move(move * Time.deltaTime * playerSpeed); 
    }

    //Change the direction the player is facing
    void HandleRotation()
    {
        //get mouse location
        Ray ray = Camera.main.ScreenPointToRay(aim);
        Plane groundPlane = new Plane(Vector3.forward, Vector3.zero);
        float rayDistance;
        Vector3 point;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            point = ray.GetPoint(rayDistance);
            //get mouse location in relation to player
            point.x = point.x - gameObject.transform.position.x;
            point.y = point.y - gameObject.transform.position.y;
            //updating the facing value based on the magnitude and direction of the mouse in relation to the player
            if (Mathf.Abs(point.x) > Mathf.Abs(point.y))
            {
                if (point.x >= 0) {
                    facing = Direction.Right;
                }
                else {
                    facing = Direction.Left;
                }
            }
            else
            {
                if (point.y >= 0) {
                    facing = Direction.Up;
                }
                else {
                    facing = Direction.Down;
                }
            }
        }
        
        switch(facing)
        {
            case Direction.Up:
                playerAnimator.SetInteger("facing", 0);
                break;
            case Direction.Down:
                playerAnimator.SetInteger("facing", 1);
                break;
            case Direction.Left:
                playerAnimator.SetInteger("facing", 2);
                break;
            case Direction.Right:
                playerAnimator.SetInteger("facing", 3);
                break;
            default: break;
        }
        // Debug.Log(facing + " " + playerAnimator.GetInteger("facing"));
    }
}
