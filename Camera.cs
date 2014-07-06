using UnityEngine;
using System.Collections;

public class Camera : MonoBehaviour {
	
	public float damping;
    public float moveSpeed;
	public float timeLeft;
    private float gyroX;
    private float gyroY;
    private float gyroZ;
    public bool canFire;
    
    public GameObject sniperPoint;
    
    public GameObject sniperSound;

	// Use this for initialization
	void Start () {
	timeLeft = 3.0f;
	canFire =true;
	}
	
	// Update is called once per frame
	void Update () {
		gyroX = (-Input.GetAxis("GyroX") *360)+90f;
        gyroY = -Input.GetAxis("GyroY") * 360;
        gyroZ = Input.GetAxis("GyroZ");
        
		transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Mathf.Clamp(gyroX,-30,30),Mathf.Clamp(gyroY,-30,30) , 0.0f), damping);

		if(Input.GetButton("Left") && canFire)
		{
			RaycastHit hit;
			sniperSound.audio.Play();
			
			if(Physics.Raycast(sniperPoint.transform.position,transform.forward,out hit))
			{
				if(hit.transform.tag == "target")
				{
					hit.rigidbody.AddForce(-Vector3.up *1000);
					Debug.Log("Hit");
				}
			}
			
			canFire = false;
			
		}
			
		if(!canFire)
		{
			timeLeft -= Time.deltaTime;
			if(timeLeft <=0)
			{
				canFire =true;
				timeLeft = 3;
			}
		}
	}
}
