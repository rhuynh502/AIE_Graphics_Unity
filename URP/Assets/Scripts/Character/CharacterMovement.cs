using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;

    private Vector2 moveInput = new Vector2();
    private bool jumpInput = false;

    // Start is called before the first frame update
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
        }

        // Get input for forward and right movements and sets the value in the animator
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

        animator.SetFloat("Forward", moveInput.y);
        animator.SetFloat("Right", moveInput.x);

        // Get input for jumping and sets the value in the animator
        animator.SetBool("Jumping", Input.GetButton("Jump"));
    }
}
