using UnityEngine;
using System.Collections;

public class MainCharacterController : MonoBehaviour {
	
	//Public configuration
	
	//Private configuration
	float m_jumpForce = 500.0f;
	float m_maxWalkingSpeed = 4.0f;
	float m_MaxSlope = 45.0f;
	float m_Gravity = 1000.0f;
	float m_InputWalkSpeed = 2000.0f;
	
	bool m_grounded = false;
	bool m_canJumpWhenGrounded = true;
	
	Collision m_collision;
	Vector3 m_Acceleration = new Vector3(0.0f, 0.0f, 0.0f);
	Vector3 m_InputForce = new Vector3(0.0f, 0.0f, 0.0f);
	
	
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(Input.GetKey(KeyCode.A) && rigidbody.velocity.x > -m_maxWalkingSpeed)
			m_InputForce.x -= m_InputWalkSpeed * Time.deltaTime;
		if(Input.GetKey(KeyCode.D) && rigidbody.velocity.x < m_maxWalkingSpeed)
			m_InputForce.x += m_InputWalkSpeed * Time.deltaTime;
		if(Input.GetKey(KeyCode.W))
			Jump();
		
		// Apply gravity
    	m_Acceleration.y -= m_Gravity * Time.deltaTime;
		//Apply input
		m_Acceleration += m_InputForce;
		
		rigidbody.AddForce(m_Acceleration);
		m_Acceleration = new Vector3(0.0f, 0.0f, 0.0f);
		m_InputForce = new Vector3(0.0f, 0.0f, 0.0f);
	}
	
	void Jump()
	{
		if(Input.GetKey(KeyCode.W) && m_grounded && m_canJumpWhenGrounded)
		{
			m_InputForce.y += m_jumpForce;
			m_canJumpWhenGrounded = false;
		}
	}
	
	void OnCollisionStay(Collision p_Collision)
	{
		foreach(ContactPoint p in p_Collision.contacts)
		{
			float angle = Vector3.Angle(p.normal, Vector3.up);
			if(angle < m_MaxSlope)
				m_grounded = true;
		}
	}
	
	void OnCollisionExit()
	{
		m_grounded = false;
		m_canJumpWhenGrounded = true;
	}
}
