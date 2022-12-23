using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public CharacterController controller;

    public Animator animator;

    [SerializeField] private float moveSpeed;
    public float moveHorizontal;
    public bool jump = false;
    public bool crouch = false;
    public bool slide = false;
    [SerializeField] float m_SlideTime = 0;

    [SerializeField] private AudioClip jumpA;
    [SerializeField] private AudioClip slideA;
    [SerializeField] private AudioSource source;
    void Start()
    {
        
    }
    
    void Update()
    {
        moveHorizontal = Input.GetAxisRaw("Horizontal") * moveSpeed;
        
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            StartCoroutine(JumpAni());
            source.PlayOneShot(jumpA);
        }
        else if (Input.GetButtonUp("Jump"))
        {
            jump = false;     
            
        }

        if (CharacterController.antiJumping == true)
        {
            animator.SetBool("Jumping", false);
        }
        
        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
        }

        if (Input.GetButtonDown("Slide"))
        {
            slide = true;
            StartCoroutine(StopSlide());
            source.PlayOneShot(slideA);
        }

        if (moveHorizontal > 0 || moveHorizontal < 0)
        {
            animator.SetBool("Running", true);
        }
        else if (moveHorizontal == 0)
        {
            animator.SetBool("Running", false);
        }


    }

    private void FixedUpdate()
    {
        controller.Move(moveHorizontal * Time.fixedDeltaTime, crouch, jump, slide);
        jump = false;
    }

    IEnumerator JumpAni()
    {
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("Jumping", true);
    }
    
    IEnumerator StopSlide()
    {
        animator.SetBool("Sliding", true);
        yield return new WaitForSeconds(m_SlideTime);
        animator.SetBool("Sliding", false);
        slide = false;
    }

    public void OnCrouching(bool isCrouching)
    {
        animator.SetBool("Crouching", isCrouching);
    }

    public void OnLanding()
    {
        animator.SetBool("Jumping", false);
    }

    

    public void OnWall()
    {
        animator.SetBool("WallSliding", true);
        animator.SetBool("Jumping", false);
    }

    public void OffWall()
    {
        animator.SetBool("WallSliding", false);
    }
    
}
