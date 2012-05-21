using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitScript : MonoBehaviour {
	
	public int health, range, atk, def, tilesMovedThisTurn, atkRange,
		numMovesCanDo, numAbilitiesCanDo, timesMoved, abilitiesUsed, visibilityRange;
	
	
	
	public string unitName;
	public List<Ability> abilities;
    private bool isVisible;
	private int team = -1;
	public System.Action<UnitScript> PerformOnNewTurn = (us) => {Debug.Log("test" + us.name);};
	
	public TileScript parentTile;
	
	public UnitScript() {
		abilities = new List<Ability>();
	}
	
	// The visual representation
	//public GameObject unit;
	
	// Use this for initialization
	void Start () {
        timesMoved = 0;
        abilitiesUsed = 0;
		tilesMovedThisTurn = 0;
		//SetTeam(team); // color stuff
		if(team != -1)
			gameObject.name = unitName + " Team " + team;
        if(atkRange == null)
            atkRange = 1;
	}
	
	public void SetTeam(int t) {
		if(gameObject.name == "Tree") return;
		
		this.team = t;
		Debug.Log ("Setting team " + t + " of " + Game.getInstance().teams.Count);
		foreach(Renderer r in gameObject.GetComponentsInChildren<Renderer>()) {
			r.material.color = Game.getInstance().teams[t].color;
		}
		
	}
	
	public int GetTeam() { return team; }
	
	/*
	public void SetUnitType(GameObject unitType) {
		unit = Object.Instantiate(unitType) as GameObject;
	}
	*/
	
	public void SetUnitPos(Vector3 v) {
		gameObject.transform.position = v;
	}
	
	public void ClearUnit() {
		
	}
	
	public Vector3 Pos() {
		return gameObject.transform.position;
	}
	
	public int RemainingMovement() {
		return range - tilesMovedThisTurn;
	}
	
	protected string description;
	public void UpdateDescription() {
		Game.Team team = Game.getInstance().GetCurrentTeam();
		string atkString = atk + (team.atkBuf != 0 ? " (+" + team.atkBuf + ")" :"");
		string defString = def + (team.defBuf != 0 ? " (+" + team.defBuf + ")" :"");
        description = unitName +
            "\nHealth: " + health +
            "\nAtk: " + atkString +
            " Def: " + defString +
            "\nAttack Range: " + atkRange +
			"\nMoves Used: " + timesMoved + "/" + numMovesCanDo +
            "\nAbilities Used: " + abilitiesUsed + "/" + numAbilitiesCanDo;
	}
	
    public virtual string UnitDescription()
    {
		return description;
	}
	
	public int GuiYOffset = 0;
	public int GuiXOffset = 0;
	public Vector2 GuiPos() {
		Vector3 unitPos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
		float x = unitPos.x + GuiXOffset;
		float y = Screen.height - unitPos.y - (12 * Camera.main.orthographicSize) + GuiYOffset;
		return new Vector2(x, y);
	}
	
	public void SetAlpha(float f) {
		foreach(Renderer r in gameObject.GetComponentsInChildren<Renderer>()) {
			if(f == 1f)
				r.material.shader = Shader.Find("Diffuse");
			else
				r.material.shader = Shader.Find ("Transparent/Diffuse");
			r.material.color = AlphaColor(r.material.color, f);
		}
	}
	
	public static Color AlphaColor(Color c, float a) {
		return new Color(c.r, c.g, c.b, a);
	}
	
	public void NewTurn() {
		if(GetTeam() == Game.getInstance().currentTeam) {
			tilesMovedThisTurn = 0;
			timesMoved = 0;
        	abilitiesUsed = 0;
			
			UpdateDescription();
			
			PerformOnNewTurn(this);
		}
	}
	
	public void attackUnit(UnitScript defender)
	{
        if (CanUseAbility())
        {
            Game.Team teamA = Game.getInstance().teams[team];
            Game.Team teamD = Game.getInstance().teams[defender.team];
			
			float num   = ((float)(atk + teamA.atkBuf) * (float)health);
			float denom = ((float)(defender.def + teamD.defBuf) * (float)defender.health);
            int dmg = (int)(Mathf.Ceil( num / denom
				* 25.0f )); //FIX - need ability.atk
            Debug.Log("DMG" + dmg + " " + num + " " + denom);
            defender.health -= dmg;
            abilitiesUsed++;
        }
	}
	
	public void SetVisible(bool b) {
		isVisible = b;
		
		if (isVisible) {
            //SetAlpha(1f);
			foreach(Renderer r in GetComponentsInChildren<Renderer>()) {
				r.enabled = true;
			}
		}
        else {
            //SetAlpha(0f);
			foreach(Renderer r in GetComponentsInChildren<Renderer>()) {
				r.enabled = false;
			}
		}
	}
	
	public bool GetVisible() { return isVisible; }

    public bool CanUseAbility(){
        return abilitiesUsed < numAbilitiesCanDo;
    }
    public bool CanMove(){
        return timesMoved < numMovesCanDo;
    }
	
	public void OnMouseDown() {
		if(parentTile) {
			parentTile.OnMouseDown();
		}
	}
	
	public string OverUnitLabel() {
		return unitName +"\n" + health + "/100";
	}
}
