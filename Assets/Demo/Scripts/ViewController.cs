using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewController : MonoBehaviour {

    public float speed = 20;
    public float mouseSpeed = 60;
	
	// Update is called once per frame
	void Update () {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float y = Input.GetAxis("Mouse X");
        float x = Input.GetAxis("Mouse Y");
        float mouse = Input.GetAxis("Mouse ScrollWheel");
        transform.Translate(new Vector3(h*speed, mouse*mouseSpeed, v*speed) *Time.deltaTime ,Space.World);
        

    }
}
