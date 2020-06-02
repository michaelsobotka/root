using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;

    public Animator animator;

    public float movementSpeed = 1f;

    private Joystick joystick;

    private Vector2 movement = Vector2.zero;

    private GameController gameController;
    private GameSceneController gameSceneController;
    private GameSceneUIController gameSceneUIController;

    public void Awake()
    {
        gameController = GameController.GetInstance();
        gameSceneController = gameController.gameSceneController;
        gameSceneUIController = gameController.gameSceneUIController;

        joystick = gameSceneController.joystick;
    }

    private void Update()
    {
        movement.x = joystick.Horizontal * -movementSpeed;
        movement.y = joystick.Vertical * -movementSpeed;

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * movementSpeed * Time.fixedDeltaTime);
    }
}