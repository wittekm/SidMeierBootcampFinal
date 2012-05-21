using UnityEngine;
using System.Collections;

public class Sound : MonoBehaviour {
	
	public AudioSource newTurn;
	public AudioSource bgm;
	
	private static Sound instance;
	public Sound() {
		instance = this;
	}
	public static Sound getInstance() { return instance; }
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
