using UnityEngine;
using System.Collections.Generic;

[RequireComponent (typeof (Rigidbody))]
public class SimpleTestChar : MonoBehaviour
{
	public struct CharFlags
	{
		public bool Grounded {get; set;}		
	}
	
	private float MaxForce = 20.0f;
	private float MaxSpeed = 1500.0f;
	private float Gravity = 100.0f;
	
	private float DeltaSinceJump = 0.0f;
	private float JumpDelay = 0.5f;
	private float JumpVelocity = 300.0f;
	private float JumpCurrentDuration = 0.0f;
	private float JumpFullDuration = 0.07f;
	//private float JumpFullDuration = 0.0f;
	
	private float WalkingSpeed = 3.0f;
	private float WalkingForce = 0.0001f;
	
	public Vector2 m_CurrentCenterPoint;     
	public Vector2 m_Velocity;
	public Vector2 m_Acceleration;
	
	private System.Random m_Randomizer;
	private CharFlags m_CharFlags;
	private CharacterController m_Controller;
	
	private Vector3 m_Movement;
	private Vector3 m_Gravity;
	private bool m_CanJump = true;
	private bool m_Punching = false;
	
	// Use this for initialization
	void Start ()
	{
    	m_Velocity = new Vector2(0.0f, 0.0f);
		m_Acceleration = new Vector2(0.0f, 0.0f);
		m_Movement = new Vector3(0.0f, 0.0f, 0.0f);
		
		animation.wrapMode = WrapMode.Loop;
		animation["Walk"].speed = 0.75F;
	    
		//animation["Punch"].speed = 10.0f;
		animation["Punch"].wrapMode = WrapMode.Once;
		animation["Punch"].AddMixingTransform(transform.Find("Armature/body/arm_R"));
			
		// Put idle and walk into lower layers (The default layer is always 0)
		// This will do two things
		// - Since shoot and idle/walk are in different layers they will not affect
		//   each other's playback when calling CrossFade.
		// - Since shoot is in a higher layer, the animation will replace idle/walk
		//   animations when faded in.
		animation["Punch"].layer = 1;
		
		animation.Play("Walk");		
		
		m_Controller = (CharacterController)GetComponent("CharacterController");	    
	}
	
	// Update is called once per frame
	void Update ()
	{
		m_CurrentCenterPoint = new Vector2(transform.position.x, transform.position.z);
		
		//Check input from user
		ProcessInput();
		
		Move1();
		
		// Reset accelertion to 0 each cycle
	  	m_Acceleration.x = 0.0f;
		m_Acceleration.y = 0.0f;		
		DeltaSinceJump += Time.deltaTime;		
	}
	
	void Move1()
	{
		Vector3 moveDirection = new Vector3(0.0f, 0.0f, 0.0f);
		moveDirection = m_Velocity;
		if (m_Controller.isGrounded)
		{
        	// We are grounded, so recalculate
        	// move direction directly from axes
        	//moveDirection = new Vector3(Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));
        	moveDirection = new Vector3(0, 0, Input.GetAxis("Horizontal"));
        	moveDirection = transform.TransformDirection(moveDirection);
        	moveDirection *= WalkingSpeed;
			
			if (Input.GetButton ("Jump"))
			{
            	JumpCurrentDuration = JumpFullDuration;
				JumpCurrentDuration -= Time.deltaTime;
				moveDirection.y += JumpVelocity*Time.deltaTime;				
        	}
    	}
		else
		{
			if(JumpCurrentDuration > 0.0f)
			{
				if(JumpCurrentDuration < Time.deltaTime)
				{
					JumpCurrentDuration -= Time.deltaTime;
					moveDirection.y += JumpVelocity*JumpCurrentDuration;
				}
				else
				{
					JumpCurrentDuration -= Time.deltaTime;
					moveDirection.y += JumpVelocity*Time.deltaTime;
				}
			}
		}

    	// Apply gravity
    	moveDirection.y -= Gravity * Time.deltaTime;
    	m_Velocity = moveDirection;
		
    	// Move the controller
    	m_Controller.Move(moveDirection * Time.deltaTime);
		
		//rigidbody.AddForce(Vector3.down*Gravity);
		//rigidbody.velocity = new Vector3(m_Velocity.x, m_Velocity.y, 0.0f);		
  	
		animation["Walk"].speed = 0.25f * m_Velocity.x;		
	}
	
	Vector2 CapForce(ref Vector2 f)
	{
		if(f.magnitude > MaxForce)
		{
			f.Normalize();
			f *= MaxForce;
		}
		return f;
	}
	
	Vector2 CapSpeed(ref Vector2 s)
	{
		if(s.magnitude > MaxSpeed)
		{
			s.Normalize();
			s *= MaxForce;
		}
		return s;
	}
	
	void ProcessInput()
	{
		Transform mixLocation = transform.Find("body");
		
		if(Input.GetKeyDown(KeyCode.LeftShift))
		{
			animation.CrossFade("Walk");
			StartRunning();					
		}
		if(Input.GetKeyUp(KeyCode.LeftShift))
			StopRunning();
		if(Input.GetKeyDown(KeyCode.E))
		{
			animation.CrossFade("Punch");
			//animation.Play("Punch");
			m_Punching = true;
		}
					
	}
	
	void StartRunning()
	{
		//float newSpeed = 1.5f;
		//float newMax = 4.0f;
		//animation.wrapMode = WrapMode.Loop;
		//foreach (AnimationState state in animation)
		//{
    //    	state.speed = newSpeed;
		//}
		//animation.Play("ArmatureAction");
		//MaxSpeed = newMax;
		//m_Running = true;
	}
	
	void StopRunning()
	{
		//float newSpeed = 0.75f;
		//float newMax = 2.0f;
		//animation.wrapMode = WrapMode.Loop;
		//foreach (AnimationState state in animation)
		//{
    //    	state.speed = newSpeed;
		//}
		//animation.Play("ArmatureAction");
		//MaxSpeed = newMax;		
		//m_Running = false;
	}	
			
	void OnCollisionStay(Collision p_Collision)
	{
		foreach(ContactPoint p in p_Collision.contacts)
		{
			float angle = Vector3.Angle(p.normal, Vector3.up);
			if(angle < 45.0f)
				m_CharFlags.Grounded = true;
		}
	}
	
	void OnCollisionExit()
	{
		m_CharFlags.Grounded = false;		
	}
}
