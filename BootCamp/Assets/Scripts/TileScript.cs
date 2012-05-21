
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileScript : MonoBehaviour {
	public enum TileType{
		GRASS,
		WATER
	};
    public enum Direction
    {
        LEFT,
        RIGHT,
        UP,
        DOWN,
    };

    public List<Texture2D> waterTextures;
    public int waterTexture;

    public float waterAnimationTimerStart;
    public float currentTime;
    public static float waterAnimationDuration = .1f;

    public Texture2D texture;
    public int x, y;

    public Direction directionToGo;
    public Direction directionFrom;
	
	public TileType tileType;
	
	public GameObject unit;
	
	public GameObject specialTerrain;
	
	public int dist;
	
	public bool visited;
	
	public GameObject marker;
	
	public GameObject fogOfWar;
	
	// Use this for initialization
	void Start () {
		dist = 999999;
		visited = false;
        waterTexture = 0;
	
		if(!fogOfWar)
			initFog();
		//fogOfWar.renderer.material.shader = Shader.Find("Transparent/Diffuse");
	}
	
	// Update is called once per frame
	void Update () {
		
		if(specialTerrain && specialTerrain.GetComponent<UnitScript>() != null) {
			if(unit) {
				specialTerrain.GetComponent<UnitScript>().SetAlpha(0.5f);
			}
			else
				specialTerrain.GetComponent<UnitScript>().SetAlpha(1f);
		}
		
		
		currentTime = Time.time;
        if (tileType == TileScript.TileType.WATER)
        {
            if (waterTexture == waterTextures.Count && (currentTime - waterAnimationDuration * (waterTexture + 1)) > waterAnimationTimerStart)
            {
                waterTexture = 0;
                waterAnimationTimerStart = Time.time;
                this.SetTexture(waterTextures[waterTexture]);
            }
            else if ((currentTime - waterAnimationDuration * (waterTexture + 1)) > waterAnimationTimerStart)
            {
                waterTexture++;
                if(waterTexture < waterTextures.Count)
                    this.SetTexture(waterTextures[waterTexture]);
            }

        }
		
	}
	
	public void OnMouseDown() {
		GameObject sceneScriptObject = GameObject.Find("SceneScriptObject");
		TileMap tileMap = sceneScriptObject.GetComponent<TileMap>();
		tileMap.tileDownCallback(this);
	}
	
	void OnMouseUp() {
		GameObject sceneScriptObject = GameObject.Find("SceneScriptObject");
		TileMap tileMap = sceneScriptObject.GetComponent<TileMap>();
		tileMap.tileUpCallback(this);
	}
	
	void OnMouseOver() {
		GameObject sceneScriptObject = GameObject.Find("SceneScriptObject");
		TileMap tileMap = sceneScriptObject.GetComponent<TileMap>();
		tileMap.tileDragoverCallback(this);
	}
	
	public bool AddUnit(GameObject unit) {
		if(this.unit != null) 
			return false;
		
		this.unit = unit;
		GetUnitScript().SetUnitPos(Pos() + new Vector3(0f,1f,0f));
		GetUnitScript().parentTile = this;
		
		if(specialTerrain && GetUnitScript().GetTeam() != -1)
			if(specialTerrain.GetComponent<UnitScript>())
				specialTerrain.GetComponent<UnitScript>().SetTeam(GetUnitScript().GetTeam());
		
		return true;
	}
	
	public bool AddSpecialTerrain(GameObject specialTerrain) {
		this.specialTerrain = specialTerrain;
		specialTerrain.transform.position = Pos() + new Vector3(0f,1f,0f);
		
		if(specialTerrain.name == "City")
			Debug.Log ("setting parent tile!!");
		specialTerrain.GetComponent<UnitScript>().parentTile = this;
		return true;
	}
	
	public bool RemoveUnit() {
		if(unit == null) return false;
		
		Game.getInstance().units.Remove(GetUnitScript());
		Object.Destroy(unit);
		unit = null;
		return false;
	}
	
	public Vector3 Pos() {
		return gameObject.transform.position;
	}
	
	public void SetTexture(Texture2D texture) {
		this.texture = texture;
		gameObject.renderer.material.mainTexture = texture;
		SetShader("Transparent/Diffuse");
	}
	
	public void SetMarked(Color c) {
		gameObject.renderer.material.color = c;
	}
	
	public void SetShader(string s) {
		gameObject.renderer.material.shader = Shader.Find(s);
	}

	public UnitScript GetUnitScript() {
 		if(unit == null) return null;
 		return unit.GetComponent<UnitScript>();
 	}
	
	// Explosion auto-deletes itself in 3 seconds.
	public GameObject Explosion() {
		GameObject explosion = ObjectFactory.getInstance().createExplosion();
		explosion.transform.position = Pos () + new Vector3(0f, 1.5f, 0f);
		return explosion;
	}
	
	public GameObject TinyExplosion() {
		GameObject explosion = ObjectFactory.getInstance().createTinyExplosion();
		explosion.transform.position = Pos () + new Vector3(0f, 1.5f, 0f);
		return explosion;
	}
	
	public int GetImpedence() {
		int intrinsicImpedence = ((tileType == TileType.WATER ? 100 : 1));
		if(specialTerrain != null) {
			if(specialTerrain.name == "Tree")
				intrinsicImpedence += 1;
		}
		
		return intrinsicImpedence;
	}
	
	public void SetFogOfWar(bool visible) {
		if(!fogOfWar) initFog();
		
		fogOfWar.renderer.enabled = visible;
		//fogOfWar.renderer
	}
	
	public void initFog() {
		if(fogOfWar) throw new System.Exception("fuck you");
		
		fogOfWar = ObjectFactory.getInstance().createFogOfWar();
		fogOfWar.transform.position = Pos () + new Vector3(0f,0.42f,0f);
		fogOfWar.renderer.material.color = new Color(0.2f, 0.2f, 0.2f, 1f);
		fogOfWar.transform.parent = gameObject.transform;
	}
}
