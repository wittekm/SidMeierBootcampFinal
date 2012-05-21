using UnityEngine;
using System.Collections;

public class JFocusCamera : MonoBehaviour {
	public GameObject obj1, obj2;
	
	
    Bounds CalculateBounds(GameObject go) {
        Bounds b = new Bounds(go.transform.position, Vector3.zero);
		/*
        Object[] rList = go.GetComponentsInChildren(typeof(Renderer));
        foreach (Renderer r in rList) {
            b.Encapsulate(r.bounds);
        }
        */
        return b;
    }
	
	
	public void FocusCameraOnGameObject(int direction = 1) {
		Camera c = Camera.main;
		
		Vector3 pos = (obj1.transform.position + obj2.transform.position) / 2f;
		// That's the position between the two!
		
		//pos.y = c.transform.position.y;
		//pos.z -= Camera.main.orthographicSize;
		//c.transform.LookAt(b.center);
		startTime = Time.time;
		
		// the original rotation
		fromPos = c.transform.rotation.eulerAngles;
		c.transform.LookAt(pos);
		toPos = c.transform.rotation.eulerAngles;
		c.transform.eulerAngles = fromPos;
		
		Debug.Log(toPos);
		
		if(toPos.y >= 180f) toPos.y -= 360f;
		/*
		if(originalRot == new Vector3())
			originalRot = c.transform.eulerAngles;
		*/
		doFocus = direction;
    }
	
	// 10:36 pm attempt
	/*
	public void FocusCameraOnGameObject(int direction = 1) {
		Camera c = Camera.main;
        Bounds b = CalculateBounds(obj1);
		b.Encapsulate(CalculateBounds(obj2));
		
		Vector3 pos = b.size;
		// That's the position between the two!
		
		pos.y = c.transform.position.y;
		//pos.z -= Camera.main.orthographicSize;
		//c.transform.LookAt(b.center);
		startTime = Time.time;
		fromPos = c.transform.position;
		toPos = pos;
		if(originalRot == new Vector3())
			originalRot = c.transform.eulerAngles;
		doFocus = direction;
    }
    */
	
	public void Unfocus() {
		startTime = Time.time;
		doFocus = -1;
	}

    // Update is called once per frame
	
	int doFocus;
	float startTime;
	Vector3 fromPos, toPos;
	Vector3 originalRot;
	static float moveTime = 0.5f;
	
    void Update () {
		if(doFocus == 1) { // just the animation
			//Camera.main.transform.position = Vector3.Lerp(fromPos, toPos, (Time.time - startTime) / moveTime);	
			Camera.main.orthographicSize = Mathf.Lerp(5, 3, (Time.time - startTime) / moveTime);
			Camera.main.transform.eulerAngles = Vector3.Lerp(fromPos, toPos, (Time.time - startTime) / moveTime);
			
			if((Time.time - startTime) / moveTime >= 1) {
				doFocus = 0;
			}
		}
		
		else if(doFocus == -1) {
			
			Camera.main.orthographicSize = Mathf.Lerp(3, 5, (Time.time - startTime) / moveTime);
			Camera.main.transform.eulerAngles = Vector3.Lerp(toPos, fromPos, (Time.time - startTime) / moveTime);
			
			if((Time.time - startTime) / moveTime >= 1) {
				doFocus = 0;
			}
		}
    }

}