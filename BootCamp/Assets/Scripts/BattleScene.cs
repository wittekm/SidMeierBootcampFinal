using UnityEngine;
using System.Collections;

public class BattleScene : MonoBehaviour {
	public UnitScript friendlyType;
	public UnitScript friendly;
	
	// Use this for initialization
	void Start () {
		/*
		friendly = Object.Instantiate(friendly) as UnitScript;
		friendly.SetUnitPos(new Vector3(-1f, 1f, -8f));
		*/
		GameObject sso = GameObject.Find ("SceneScriptObject");
		Game game = sso.GetComponent<Game>();
		
	    Debug.Log (game == null);
		Debug.Log (game.tileMap.At (2,2).unit);
		
		JFocusCamera cam = GetComponent<JFocusCamera>();
		cam.obj1 = game.tileMap.At (2,2).gameObject;
		cam.obj2 = game.tileMap.At (4,4).gameObject;
		cam.FocusCameraOnGameObject();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI() {
		int padding = 20;
		int height = 100;
		Rect boxSize = new Rect(padding, Screen.height-(padding+height), 
				Screen.width-2*padding, height);
		GUI.Box(boxSize, "Battle!");
	}
}
