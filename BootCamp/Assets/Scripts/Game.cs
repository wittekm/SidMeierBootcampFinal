using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {
	public int initialAP;
	public UnitFactory factory;
	public MovementHandler movHandler;
	public TileScript selectedTile = null;

    public Texture2D black;

    public bool switchingPlayers;
	public TileMap tileMap;
	
	public HashSet<UnitScript> units;
	public HashSet<SpecialTerrainScript> specialTerrains;
	
	public class Team {
		public Team(int ap, Color c) { 
			actionPoints = ap;
			color = c;
			foundTiles = new HashSet<TileScript>();
			lastView = new Vector3(0,0,0);
		}
		public int actionPoints;
		public Color color;
		public HashSet<TileScript> foundTiles;
		
		public int atkBuf, defBuf, apBuf;
		
		public Vector3 lastView;
	}
	
	public List<Team> teams;
	public int currentTeam;
	
	private GUIStyle bigFontStyle;
	private GUIStyle tinyFontStyle;

	
	public Game() {
		teams = new List<Team>();
		
		bigFontStyle = new GUIStyle();
		bigFontStyle.fontSize = 16;
		bigFontStyle.normal.textColor = Color.white;
		bigFontStyle.alignment = TextAnchor.UpperCenter;
		
		tinyFontStyle = new GUIStyle(bigFontStyle);
		tinyFontStyle.fontSize = 10;
		tinyFontStyle.fontStyle = FontStyle.Bold;
		
		units = new HashSet<UnitScript>();
		specialTerrains = new HashSet<SpecialTerrainScript>();
		teams.Add (new Team(initialAP, Color.blue));
		teams.Add (new Team(initialAP, Color.red));
		currentTeam = 0;
		instance = this;
	}

	// Use this for initialization
	void Start () {
        switchingPlayers = false;
		DontDestroyOnLoad(this);
		
		tileMap = gameObject.GetComponent<TileMap>();
		ActuallyStart();
	}
	
	static Game instance;
	public static Game getInstance() {
		return instance;
	}
	
	// Called from TileMap
	public void ActuallyStart() {
		
		if(Sound.getInstance() == null) throw new System.Exception("fdskjfhksdf");
		Sound.getInstance().bgm.Play();
		Camera.main.transform.position = new Vector3(tileMap.width * 0.25f, 5f, tileMap.height * 0.25f);
		
		/*
		tileMap.At (1, 6).AddSpecialTerrain(ObjectFactory.getInstance().createTree());
		tileMap.At (0, 6).AddSpecialTerrain(ObjectFactory.getInstance().createTree());
		tileMap.At (1, 5).AddSpecialTerrain(ObjectFactory.getInstance().createTree());
		tileMap.At (0, 5).AddSpecialTerrain(ObjectFactory.getInstance().createTree());
		tileMap.At (1, 4).AddSpecialTerrain(ObjectFactory.getInstance().createTree());
		tileMap.At (0, 4).AddSpecialTerrain(ObjectFactory.getInstance().createTree());
		
		tileMap.At (0, 9).AddSpecialTerrain(ObjectFactory.getInstance().createFactory());
		tileMap.At (0, 8).AddSpecialTerrain(ObjectFactory.getInstance().createCity());
		tileMap.At (1, 8).AddSpecialTerrain(ObjectFactory.getInstance().createFort());
		tileMap.At (1, 9).AddSpecialTerrain(ObjectFactory.getInstance().createBarracks());
		*/
		
		/*
		SpawnUnitAt(2, 2, 0, factory.createConstructionGuy());
		SpawnUnitAt(7, 1, 1, factory.createConstructionGuy());
		SpawnUnitAt(7, 4, 0, factory.createScout());
		SpawnUnitAt(5, 6, 1, factory.createPriest());
		
		SpawnUnitAt(5, 8, 1, factory.createScout());
		*/
		
		NewTurn();
		DoFogOfWar();
		
		// yeah, this is dumb. it doesn't do fog of war right unless i do it and i'm too lazy to debug
	}
	
	public void SpawnUnitAt(int i, int j, int team, int range, GameObject o) {
		TileMap tileMap = gameObject.GetComponent<TileMap>();
		
		TileScript tileScript = tileMap.At(i, j);
		if(!tileScript.AddUnit(o)) {
			units.Remove(o.GetComponent<UnitScript>());
			Object.Destroy(o);
			consoleString = "oops, Unit at this position";
		}
		tileScript.GetUnitScript().SetTeam(team);
        tileScript.GetUnitScript().atkRange = range;
	}
	
	public void SpawnUnitAt(int i, int j, int team, GameObject o) {
		TileMap tileMap = gameObject.GetComponent<TileMap>();
		
		TileScript tileScript = tileMap.At(i, j);
		if(!tileScript.AddUnit(o)) {
			units.Remove(o.GetComponent<UnitScript>());
			Object.Destroy(o);
			consoleString = "oops, Unit at this position";
		}
		tileScript.GetUnitScript().SetTeam(team);
	}
	
	public void SpawnSoldierAt(int i, int j, int team, int range) {
		SpawnUnitAt(i, j, team, range, factory.createConstructionGuy());
	}
	
		
	public void SpawnCircleAt(int i, int j, int team, int range) {
		SpawnUnitAt(i, j, team, range, factory.createCircleUnit());
	}
	
	
	public void SetSelectorOnTile(TileScript tileScript) {
		if(defendingUnit) UnBattle();
		defendingUnit = null;
		
		if(selectedTile == tileScript) { // clicked tile again; deselect
			Debug.Log ("Deselect");
			movHandler.DeselectTile(tileScript);
			selectedTile = null;
		}
		else if(selectedTile) { // average case; deselect the previous tile
			Debug.Log ("there is a SelectedTile");
			
			if(! movHandler.MoveUnit(selectedTile, tileScript)) {
			
				// If the move handler doesn't deal with deselect/reselect, fuck it, we'll do it here
				movHandler.DeselectTile (selectedTile);
				movHandler.SelectTile(tileScript);
				selectedTile = tileScript;
				
			}
		}
		else { // no previous tile to deselect
			Debug.Log ("No previous tile");
			movHandler.SelectTile(tileScript);
			selectedTile = tileScript;
		}
	}
	
	public void GoBattle(UnitScript unit) {
		if(defendingUnit == unit) return;

		//Application.LoadLevel(1); // BattleScene
		JFocusCamera cam = GetComponent<JFocusCamera>();
		cam.obj1 = unit.parentTile.gameObject;
		cam.obj2 = selectedTile.gameObject;
		cam.FocusCameraOnGameObject();
		
		unit.gameObject.transform.LookAt(selectedTile.unit.transform);
		selectedTile.unit.transform.LookAt(unit.gameObject.transform);
		
		//Rect box = new Rect(0, Screen.height - unit.abilities.Count*20, 100, unit.abilities.Count*20);
		defendingUnit = unit;
	}
	
	UnitScript defendingUnit;
	
	public void UnBattle() {
		JFocusCamera cam = GetComponent<JFocusCamera>();
		cam.Unfocus();
		defendingUnit = null;
	}
	
	public Team GetCurrentTeam() {
		if(currentTeam >= teams.Count) throw new System.Exception("what "+ currentTeam + teams.Count);
		return teams[currentTeam];
	}
	
	// Jut for testing
	public void OnGUI() {
		
		if(GUIWinner()) return;

        if (switchingPlayers)
        {
            GUIStyle style = new GUIStyle();
            style.normal.background = black;
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "Switch Seats", style);
        }
		// Shows current team, action points, etc
		GeneralGUIInfo();
		
		
		if(selectedTile) {
			// Show my description
			UnitScript unit = selectedTile.GetUnitScript();
			try {
			if(unit != null )
				GUI.Box(new Rect(Screen.width - 200, 0, 200, 125), "");
				GUI.Label(new Rect(Screen.width - 200, 0, 200, 125), unit.UnitDescription(), bigFontStyle);
			} catch(System.Exception e) {
				
			}
			// Don't show battle stuff for enemies!
			if(unit && unit.GetTeam() != currentTeam)
				return;
			
			// Don't show choice of attackable dudes unless no defending unit
			if(!defendingUnit)
				BattleGUIShowAttackables(unit);
			// If there is a defending unit, then show abilities!
			else
				BattleGUIShowAbilities();
			
			if(selectedTile.specialTerrain)
				SpecialTerrainShowAbilities();
		}
		
        if(!switchingPlayers)
        {
			GUIShowDescriptions(); // name + 100/100
		}
		
		GUIConsole();
	}
	
	private void GUIShowDescriptions() {
		// Show health
        foreach (UnitScript unit in units)
        {
            if (unit.GetVisible())
            {
                Vector2 guiPos = unit.GuiPos();
                Rect healthRect = new Rect(guiPos.x-15, guiPos.y, 50, 50);
                GUI.Label(healthRect, unit.OverUnitLabel(), tinyFontStyle);
            }
        }
		
		// Show terrain descriptions
        foreach (SpecialTerrainScript unit in specialTerrains)
        {
            if (unit.GetVisible())
            {
                Vector2 guiPos = unit.GuiPos();
                Rect healthRect = new Rect(guiPos.x, guiPos.y, 50, 50);
                GUI.Label(healthRect, unit.UnitDescription(), tinyFontStyle);
            }
        }
	}
	
	private void GeneralGUIInfo() {
        GUIStyle coloredStyle = new GUIStyle(bigFontStyle);
        coloredStyle.normal.textColor = GetCurrentTeam().color;
        coloredStyle.alignment = TextAnchor.UpperLeft;
		GUI.Box(new Rect(0, 0, 400, 100), "");

        if (!switchingPlayers)
        {
			Team team = GetCurrentTeam();
			int remainingAP = GetCurrentTeam().actionPoints;
			int totalAP = initialAP + GetCurrentTeam().apBuf;
            GUI.Label(new Rect(10, 0, 390, 30), "Action Points: " + remainingAP + " / " + totalAP, coloredStyle);
			
			string buffs = "";
			if(team.atkBuf + team.defBuf + team.apBuf > 0) {
				buffs = "Buffs:\n";
				if(team.atkBuf != 0)
					buffs += "ATK: +"+team.atkBuf+ " ";
				if(team.defBuf != 0)
					buffs += "DEF: +"+team.defBuf+ " ";
				if(team.atkBuf != 0)
					buffs += "AP: +"+team.apBuf;
			}
			GUI.Label(new Rect(10, 30, 400, 50), buffs, coloredStyle);

            if (GUI.Button(new Rect(300, 70, 100, 30), "End Turn"))
            {
                EndTurn();
            }
        }
        else{
            GUI.Label(new Rect((Screen.width/2)-200, (Screen.height/2) - 50, 400, 200), "TEAM " + currentTeam + "'S TURN", coloredStyle);
            if (GUI.Button(new Rect((Screen.width / 2) - 50, (Screen.height / 2), 100, 50), "Ready"))
            {
				Sound.getInstance().newTurn.Play();
                EndTurn();
            }
        }
	}
	
	private void BattleGUIShowAttackables(UnitScript unit) {
		// Show all people we can attack
		List<GameObject> attackables = movHandler.canUseAbilityOn(selectedTile);
		
		for(int i = 0; i < attackables.Count; i++) {
			unit = attackables[i].GetComponent<TileScript>().GetUnitScript();
			
			// Make sure there's an ability than can affect the other dude
			if(selectedTile.GetUnitScript().abilities.Find( 
				(abil) => abil.predicate(selectedTile, unit.parentTile)
				)
				 == null)
				continue;
			
			if(unit && unit.GetVisible() && !switchingPlayers) {
				Vector2 pos = unit.GuiPos();
				Rect r = new Rect(pos.x - 50, pos.y+50, 110, 40);
				if( GUI.Button(r, unit.unitName ) ) {
					GoBattle(unit);
				}
			}
		}
	}
	
	// Show abilities during battle
	private void BattleGUIShowAbilities() {
		if(defendingUnit != null && selectedTile != null) {
			UnitScript attackingUnit = selectedTile.GetUnitScript();
			
			for(int i = 0; i < attackingUnit.abilities.Count; i++) {
				Ability a = attackingUnit.abilities[i];
				Rect abilityRect = new Rect(410 + (140*i), 0, 140, 40);
				bool abilityUsableHere = a.predicate(attackingUnit.parentTile, defendingUnit.parentTile);
				
				if(a.actionPoints <= GetCurrentTeam().actionPoints && abilityUsableHere && attackingUnit.CanUseAbility()) {
					if(GUI.Button(abilityRect, a.description) ){
						Debug.Log (a.predicate(attackingUnit.parentTile, defendingUnit.parentTile));
						a.exec(attackingUnit.parentTile, defendingUnit.parentTile);
						
						// This redoes Dijkstra's algorithm and recalcs movement tiles. Hacky!
						movHandler.DeselectTile(selectedTile);
						movHandler.SelectTile(selectedTile);
					}
				}
				else
					GUI.Label(abilityRect, a.description + " (unable)");
			}
		}
	}
	
	// Show abilities of specia lterrain (factories)
	private void SpecialTerrainShowAbilities() {
		if(selectedTile != null && selectedTile.specialTerrain != null) {
			UnitScript specialTerrain = selectedTile.specialTerrain.GetComponent<UnitScript>();
			if(specialTerrain == null) return;
			if(specialTerrain.GetTeam() != currentTeam) return;
			
			for(int i = 0; i < specialTerrain.abilities.Count; i++) {
				Ability a = specialTerrain.abilities[i];
				Rect abilityRect = new Rect(410 + (140*(i%3)), 60+(50*(i/3)), 140, 40);
				if(a.actionPoints <= GetCurrentTeam().actionPoints) {
					if(GUI.Button(abilityRect, a.description) ){
						a.exec(selectedTile, selectedTile);
						
						// This redoes Dijkstra's algorithm and recalcs movement tiles. Hacky!
						movHandler.DeselectTile(selectedTile);
						movHandler.SelectTile(selectedTile);
					}
				}
				else
					GUI.Label(abilityRect, a.description + " (unable)");
			}
		}
	}
	
	
	bool GUIWinner() {
		if(gameWinner != -1) {
			GUIStyle winner = new GUIStyle(bigFontStyle);
			winner.fontSize = 96;
			winner.normal.textColor = GetCurrentTeam().color;
			GUI.Label(new Rect(20, Screen.height / 2f - 150, Screen.width - 40, 300), "gg!", winner);
			return true;
		} else return false;
	}
	
	
	public string consoleString;
	
	void GUIConsole() {
		Rect consoleRect = new Rect(10, 100, 400, 50);
		GUI.Label (consoleRect, "CONSOLE: " + consoleString);
	}
	
	// Team stuff, you know, actual turns!
	void EndTurn() {
		consoleString = "";
		
		if(defendingUnit)
			UnBattle();
		
		movHandler.DeselectTile(selectedTile);
		selectedTile = null;
		GetCurrentTeam().lastView = GameObject.Find("FP").transform.position;
		
		CheckWin();
		
		ChangePlayer();
				
		if(switchingPlayers)
		{	
			currentTeam++;
			if(currentTeam == teams.Count) currentTeam = 0;
			NewTurn();
			
			DoFogOfWar();
		}
		
		GameObject.Find ("CameraLight").light.color = GetCurrentTeam().color;
	}
	
	void MoveCameraToLastPosition() {
		if(GetCurrentTeam().lastView != new Vector3(0,0,0))
			GameObject.Find("FP").transform.position = GetCurrentTeam().lastView;
		else {
			foreach(UnitScript unit in units) {
				if(unit.GetTeam() == currentTeam) {
					GameObject.Find("FP").transform.position = unit.Pos() + new Vector3(-12, 4, -12);
					break;
				}
			}
		}
	}
	
	
    void ChangePlayer()
    {
        if (switchingPlayers == true)
            switchingPlayers = false;
        else
            switchingPlayers = true;
    }
	
	void NewTurn() {
		// Reset AP et al
		GetCurrentTeam().actionPoints = initialAP;
		GetCurrentTeam().atkBuf = 0;
		GetCurrentTeam().defBuf = 0;
		GetCurrentTeam().apBuf = 0;
		
		foreach(UnitScript unit in units) {
			unit.NewTurn();
		}
		
		// These now provide the buffs 
		foreach(UnitScript unit in specialTerrains) {
			unit.NewTurn();
		}
		
		GetCurrentTeam().actionPoints += GetCurrentTeam().apBuf;
		
		MoveCameraToLastPosition();
	}
	
	public void DoFogOfWar() {
		
		// Update foundTiles
		foreach(UnitScript unit in units) {
			// Don't do it for things currently moving, let them explore by themselves!
			if(unit.GetComponent<UnitMoveScript>()) continue;
			
			if(unit.GetTeam() == currentTeam)
				foreach(TileScript s in movHandler.visibleTilesFrom(unit.parentTile)) {
					GetCurrentTeam().foundTiles.Add(s);
				}
		}
		
		
		// Update visual representation: units
		foreach(UnitScript unit in units) {
			// Don't do it for things currently moving, let them explore by themselves!
			if(unit.GetComponent<UnitMoveScript>()) continue;
			
			if(unit.GetTeam() == currentTeam) {
                unit.SetVisible(true);
			}
			// Unit occupies a tile we know
			else if(GetCurrentTeam().foundTiles.Contains(unit.parentTile)) {
                unit.SetVisible(true);
			}
			else {
                unit.SetVisible(false);
			}
		}
		
		foreach(SpecialTerrainScript unit in specialTerrains) {
			if(unit.GetTeam() == currentTeam) {
				//Debug.Log ("My name is " + unit.name + " and im on the right team");
                unit.SetVisible(true);
			}
			// Unit occupies a tile we know
			else if(GetCurrentTeam().foundTiles.Contains(unit.parentTile)) {
				//if(unit.name == "City")
					//throw new System.Exception ("My name is " + unit.name + " and im found");
                unit.SetVisible(true);
			}
			else {
				//Debug.Log ("My name is " + unit.name + " and im invisible");
                unit.SetVisible(false);
			}
		}
		
		tileMap.Each( (TileScript s) => 
		{  
			//Debug.Log (":: " + s.x + " " + s.y + " " + GetCurrentTeam().foundTiles.Contains(s));
			s.SetFogOfWar(! GetCurrentTeam().foundTiles.Contains(s));
		});
		
		//Debug.Log (GetCurrentTeam().foundTiles.Count);
	}
	
	int gameWinner = -1;
	public void CheckWin() {
		HashSet<UnitScript> unitsOnTeamZero = new HashSet<UnitScript>(units);
		HashSet<UnitScript> unitsOnTeamOne = new HashSet<UnitScript>(units);
		unitsOnTeamZero.RemoveWhere( unitScript => unitScript.GetTeam() == 1 );
		unitsOnTeamOne.RemoveWhere( unitScript => unitScript.GetTeam() == 0 );
		
		// if teams[0] has 0 units, team1 won
		if(unitsOnTeamZero.Count == 0)
			gameWinner = 1;
		// if teams[0] has all the units, team0 won
		else if(unitsOnTeamOne.Count == 0)
			gameWinner = 0;
		
		bool foundCommander = false;
		
		// check team0 has commander
		foreach(UnitScript unit in unitsOnTeamZero) {
			if(unit.unitName == "Commander") {
				Debug.Log("FOUND COMMANDER SOMEHOW???");
				foundCommander = true;
				break;
			}
		}
		if(!foundCommander) {
			gameWinner = 1;
			return;
		}
		foundCommander = false;
		// check team1 has commander
		foreach(UnitScript unit in unitsOnTeamOne) {
			if(unit.unitName == "Commander") {
				foundCommander = true;
				break;
			}
		}
		if(!foundCommander) {
			gameWinner = 0;
			return;
		}
	}
	
	public void Update() {
		if (Input.GetButtonDown ("Fire1")) {
			// Construct a ray from the current mouse coordinates
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit = new RaycastHit();
			if (Physics.Raycast(ray, out hit)) {
				if(hit.collider.gameObject.name == "BottomPlane")
					movHandler.DeselectTile(null);
				
				TileScript ts = hit.collider.gameObject.GetComponent<TileScript>();
				if(ts != null) {
					ts.OnMouseDown();
				}
			}
		}
		
		float wasd = Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical"));
				
		if (wasd != 0f) {
			if(defendingUnit) {
				defendingUnit = null;
				UnBattle();
			}
			return; // skip shitty mouse camera thing
		}
		
        /*if(!defendingUnit) {
			Vector3 newVec = getNewCameraVelocity();
            GameObject.Find("FP").GetComponent<CharacterMotor>().SetVelocity(newVec);
		}*/
	}

    public Vector3 getNewCameraVelocity()
    {
        Vector3 pos = Input.mousePosition;
        Vector3 vel;
        if ((pos.x <= Screen.width && pos.x >= Screen.width * .92) || (pos.x >= 0 && pos.x <= Screen.width * .08) || (pos.y <= Screen.height && pos.y >= Screen.height * .92) || (pos.y >= 0 && pos.y <= Screen.height * .08))
        {
            vel.x = ((pos.x - (Screen.width / 2)) / Screen.width) * 12;
            vel.y = ((pos.y - (Screen.height / 2)) / Screen.height) * 12;
            vel.z = 0;
            return vel;
        }
        else
            return new Vector3(0, 0, 0);
    }
	public void RefreshUnitDescriptions() {
		foreach(UnitScript unit in units) {
			unit.UpdateDescription();
		}
	}
	
	
}
