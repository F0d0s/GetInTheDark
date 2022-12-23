using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private float m_SlideSpeed = 0;
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;				// A collider that will be disabled when crouching
	
	
	[SerializeField] private float m_WallJumpForce;
	public Transform wallCheck;
	private bool m_IsWallTouch;
	[SerializeField] bool m_IsSliding;
	private bool m_IsMoving;
	[SerializeField] private float m_WallSlideSpeed;

	[SerializeField] private ParticleSystem dust;
	[SerializeField] private ParticleSystem dustSlide;
	[SerializeField] private ParticleSystem dustWall;
	

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	[SerializeField] bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
	private bool wasGrounded;
	
	[SerializeField] private LayerMask antiJump;
	[SerializeField] public static bool antiJumping;
	
	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;
	public UnityEvent OnWallEvent;
	public UnityEvent OffWallEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void Update()
	{
		if (m_IsSliding)
		{
			
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x,
				Mathf.Clamp(m_Rigidbody2D.velocity.y, -m_WallSlideSpeed, float.MaxValue));
			OnWallEvent.Invoke();
		}
		
		m_IsWallTouch = Physics2D.OverlapBox(wallCheck.position, new Vector2(0.3f, 1.4f), 0, m_WhatIsGround);
		if (m_IsWallTouch && !m_Grounded)
		{
			m_IsSliding = true;
			
		}
		else
		{
			m_IsSliding = false;
			OffWallEvent.Invoke();
		}
		
	}

	private void FixedUpdate()
	{
		wasGrounded = m_Grounded;
		m_Grounded = false;
		antiJumping = false;
		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
		Collider2D[] collidersAJ = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, antiJump);
		for (int i = 0; i < collidersAJ.Length; i++)
		{
			if (collidersAJ[i].gameObject != gameObject)
			{
				antiJumping = true;
			}
		}
	}

	


	public void Move(float move, bool crouch, bool jump, bool slide)
	{
		// If crouching, check to see if the character can stand up
		if (!crouch || !slide)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			// If crouching
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				
			} else
			{
				// Enable the collider when not crouching
				

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			if (slide && m_Grounded)
			{
				if (m_FacingRight)
				{
					dustSlide.transform.localScale = new Vector2(1, 1);;
					
				}
				else
				{
					dustSlide.transform.localScale = new Vector2(-1, 1);;
					
				}

				if (move > 0 || move < 0)
				{
					createSlideDust();
				}
				move *= m_SlideSpeed;
				
			}

			if (slide || crouch)
			{
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			}
			else
			{
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;
			}
			
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				if (m_Grounded)
					createDust();
				
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				if (m_Grounded)
					createDust();
				Flip();
			}
		}
		// If the player should jump...
		if (m_Grounded  && jump)
		{
			// Add a vertical force to the player.
			m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		}

		if (m_IsSliding && jump && m_FacingRight)
		{
			m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(-1 * m_WallJumpForce, m_JumpForce));
		}
		if (m_IsSliding && jump && !m_FacingRight)
		{
			m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(1 * m_WallJumpForce, m_JumpForce));
		}
	}


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
		
	}

	public void createDust()
	{
		dust.Play();
	}

	public void createSlideDust()
	{
		dustSlide.Play();
	}

	public void createWallDust()
	{
		dustWall.Play();
	}
	
}