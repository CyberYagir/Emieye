 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsAnimations : MonoBehaviour {
    public GameObject Hands;
    public float Ammout,SmoothAmmount;
    public Vector3 StartPos;
    // Use this for initialization
    void Start () {
        StartPos = Hands.transform.localPosition;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        float xrot = Input.GetAxisRaw("Mouse X") * Ammout;
        float yrot = Input.GetAxisRaw("Mouse Y") * Ammout;
        Vector3 POS = new Vector3(xrot,yrot,0);
        Hands.transform.localPosition = Vector3.Lerp(Hands.transform.localPosition, POS + StartPos,Time.fixedDeltaTime * SmoothAmmount);
    }
}
