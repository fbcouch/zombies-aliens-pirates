using UnityEngine;
using System.Collections;

public class MainCharacterController : MonoBehaviour {
	
	Vector3 m_Acceleration = new Vector3(0.0f, 0.0f, 0.0f);
	Vector3 m_InputForce = new Vector3(0.0f, 0.0f, 0.0f);
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.A))
			m_InputForce.x = -5.0f;
		if(Input.GetKey(KeyCode.D))
			m_InputForce.x = 5.0f;
		
		m_Acceleration += m_InputForce;
		if(rigidbody.velocity.magnitude > 2.0f)
		{
			rigidbody.AddForce(rigidbody.velocity*-1.0f);
		}
		rigidbody.AddForce(new Vector3(m_Acceleration.x, 0.0f, m_Acceleration.y));
		m_Acceleration = new Vector3(0.0f, 0.0f, 0.0f);
	}
}
