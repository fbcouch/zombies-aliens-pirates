using UnityEngine;
using System.Collections;

public class EnemyCharacterController : MonoBehaviour {
	
	//Public configuration
	
	//Private configuration
	float m_jumpForce = 500.0f;
	float m_maxWalkingSpeed = 4.0f;
	float m_MaxSlope = 45.0f;
	float m_Gravity = 1000.0f;
	float m_InputWalkSpeed = 2000.0f;
	
	float m_moveDamp = 0.1f;
	Transform m_parentToFollow;
	Vector3 m_parentLastPos;
	Quaternion m_targetRotation;
	float m_turnSpeed = 10.0f;
	
	float m_velocityX;
	float m_velocityZ;
	
	EnemyMovementScript m_parentScript;
	string m_walkAnimationString;
	string m_runAnimationString;
	
	bool m_grounded = false;
	bool m_canJumpWhenGrounded = true;
	
	Collision m_collision;
	Vector3 m_Acceleration = new Vector3(0.0f, 0.0f, 0.0f);
	Vector3 m_InputForce = new Vector3(0.0f, 0.0f, 0.0f);
	
	
	// Use this for initialization
	void Start ()
	{
		m_parentScript = (EnemyMovementScript)transform.parent.gameObject.GetComponent("EnemyMovementScript");
		m_parentToFollow = transform.parent;
		transform.parent = null;
		animation.wrapMode = WrapMode.Loop;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 tempPos = transform.position;
		tempPos.x = Mathf.SmoothDamp(transform.position.x, m_parentToFollow.position.x, ref m_velocityX, m_moveDamp);
		tempPos.z = Mathf.SmoothDamp(transform.position.z, m_parentToFollow.position.z, ref m_velocityZ, m_moveDamp);
		transform.position = tempPos;
		
		Vector3 tempParentCurrentPos = m_parentToFollow.position;
		Vector3 tempParentLastPos = m_parentLastPos;
		tempParentCurrentPos.y = 0;
		tempParentLastPos.y = 0;
		
		m_targetRotation = Quaternion.LookRotation(tempParentCurrentPos - tempParentLastPos);
		m_parentLastPos = m_parentToFollow.position;
		
		transform.rotation = Quaternion.Lerp(transform.rotation, m_targetRotation, m_turnSpeed * Time.deltaTime);
		
		RaycastHit[] hits = Physics.RaycastAll(new Ray(transform.position + new Vector3(0.0f, 100.0f, 0.0f), Vector3.up*-1));
		for(int i=0; i<hits.Length; i++)
		{
			RaycastHit hit = hits[i];
			if(hit.transform.gameObject == ((EnemyMovementScript)m_parentToFollow.gameObject.GetComponent("EnemyMovementScript")).currentCell)
			{
				Vector3 tempHitPos = transform.position;
				tempHitPos.y = hit.point.y;
				transform.position = tempHitPos;
			}
		}
		
		//capital IsPlaying, because otherwise true if playing anything
		//If not walking and NOT aware
		if(!animation.IsPlaying(m_walkAnimationString) && !m_parentScript.aware)
			animation.Play(m_walkAnimationString);
		//If not running and IS aware
		if(!animation.IsPlaying(m_runAnimationString) && m_parentScript.aware)
			animation.Play(m_runAnimationString);
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
