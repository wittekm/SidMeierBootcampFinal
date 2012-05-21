using UnityEngine;
using System.Collections;

public class LightStrobe : MonoBehaviour {
	//public GameObject targetLight;
	public float minIntensity;
	public float maxIntensity;
	public float lightStep;
	
	// Strobe brighter or fainter
	private int direction = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(gameObject.light.intensity > maxIntensity) {
			//Debug.Log ("Going down");
			direction = -1;
		}
		else if(gameObject.light.intensity < minIntensity) {
			direction = 1;
		}
		gameObject.light.intensity += direction * lightStep * Time.deltaTime;
		//Debug.Log (gameObject.light.intensity);
	}
}
