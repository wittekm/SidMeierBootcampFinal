using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectFactory : MonoBehaviour {
	
	
	public GameObject treeType;
	public GameObject tinyExplosionType;
	public GameObject explosionType;
	public GameObject factoryType;
	public GameObject cityType;
	public GameObject fortType;
	public GameObject barrackType;
	
	public GameObject fogType;
	
	public Dictionary<string, System.Func<GameObject>> creationTable;
	Dictionary<string, string> team1;
	Dictionary<string, string> team2;
	
	private static ObjectFactory instance;
	public ObjectFactory() {
		instance = this;
		
		creationTable = new Dictionary<string, System.Func<GameObject>>();
		creationTable.Add("factory", createFactory);
		creationTable.Add ("city", createCity);
		creationTable.Add ("fort", createFort);
		creationTable.Add ("barracks", createBarracks);
		creationTable.Add ("tree", createTree);
		
		team1 = new Dictionary<string, string>();
		team2 = new Dictionary<string, string>();
		
		team1.Add ("scout", "lambo");
		team1.Add ("ranged", "archer");
		team1.Add ("healer", "healer");
		team1.Add ("medium", "construction");
		team1.Add ("heavy", "predator");
		
		team2.Add ("scout", "normandy");
		team2.Add ("ranged", "wizard");
		team2.Add ("healer", "healer");
		team2.Add ("medium", "construction");
		team2.Add ("heavy", "centurion");
	}
	
	public GameObject create(string type) {
		if(!creationTable.ContainsKey(type))
			throw new System.Exception("cant make this");
		
		System.Func<GameObject> func = null;
		creationTable.TryGetValue(type, out func);
		return func();
	}
	
	public static ObjectFactory getInstance() {
		return instance;
	}
	
	public GameObject createTinyExplosion() {
		GameObject exp = Object.Instantiate(tinyExplosionType) as GameObject;
		Object.Destroy(exp, 1.5f);
		//Object.Destroy(explosion, 3f); // destroy the explosion in 3 seconds
		return exp;
	}
	
	public GameObject createExplosion() {
		GameObject explosion = Object.Instantiate(explosionType) as GameObject;
		Object.Destroy(explosion, 3f); // destroy the explosion in 3 seconds
		return explosion;
	}
	
	
	
	// Special Terrain section
	private void RegisterSpecialTerrain(GameObject o) {
		Game.getInstance().specialTerrains.Add(o.GetComponent<SpecialTerrainScript>());
	}
	
	private void AccountForMissingUnitScript(GameObject o) {
		foreach(Component c in o.GetComponents<UnitScript>()) {
			Object.Destroy(c);
		}
		
		if(!o.GetComponent<SpecialTerrainScript>())
			o.AddComponent<SpecialTerrainScript>();
		
	}
	
	public GameObject createTree() {
		GameObject tree = Object.Instantiate(treeType) as GameObject;
		AccountForMissingUnitScript(tree);
		RegisterSpecialTerrain(tree);
		tree.name = "Tree";
		
		tree.transform.Rotate(0, Random.Range(0, 359), 0);
		
		foreach(Renderer r in tree.GetComponentsInChildren<Renderer>())
			r.material.shader = Shader.Find("Transparent/Diffuse");
		return tree;
	}
	
	// for factories
	public int createUnitAt(TileScript atk, UnitScript factory, string type) {
		bool unitBefore = (atk.unit != null);
		// if so, return 0 to say "this ability failed so don't charge me action points
		if(unitBefore) {
			Game.getInstance().consoleString = "There's already a unit at Factory.";
			return 0;
		}
		
		if(factory.GetTeam() == 0)
			team1.TryGetValue(type, out type);
		else
			team2.TryGetValue(type, out type);
		
		GameObject unit = UnitFactory.getInstance().create(type);
		Game.getInstance().SpawnUnitAt(atk.x, atk.y, factory.GetTeam(), unit);
		
		return 1;
	}
	
	public GameObject createFactory() {
		GameObject obj = Object.Instantiate(factoryType) as GameObject;
		AccountForMissingUnitScript(obj);
		RegisterSpecialTerrain(obj);
		SpecialTerrainScript unitScript = obj.GetComponent<SpecialTerrainScript>();
		obj.name = "Factory";
		unitScript.terrainDescription = "Factory";
		unitScript.GuiXOffset -= 20;
		
		unitScript.abilities.Add(new Ability(
			"Create Scout",
			25,
			(TileScript atk, TileScript def) => {
				return createUnitAt(atk, unitScript, "scout");
			},
		
			// Doesn't matter for these UnitScritps applied to SpecialTerrains
			(atk, def) => true
		));
		
		unitScript.abilities.Add(new Ability(
			"Create Ranged",
			30,
			(TileScript atk, TileScript def) => {
				return createUnitAt(atk, unitScript, "ranged");
			},
			(atk, def) => true
		));
		
		unitScript.abilities.Add(new Ability(
			"Create Medium",
			30,
			(TileScript atk, TileScript def) => {
				return createUnitAt(atk, unitScript, "medium");
			},
			(atk, def) => true
		));

        unitScript.abilities.Add(new Ability(
            "Create Healer",
            30,
            (TileScript atk, TileScript def) =>
            {
                return createUnitAt(atk, unitScript, "healer");
            },
            (atk, def) => true
        ));

		unitScript.abilities.Add(new Ability(
			"Create Heavy",
			35,
			(TileScript atk, TileScript def) => {
				return createUnitAt(atk, unitScript, "heavy");
			},
			(atk, def) => true
		));
		
		foreach(Renderer r in obj.GetComponentsInChildren<Renderer>())
			r.material.shader = Shader.Find("Transparent/Diffuse");
		return obj;
	}
	
	public GameObject createCity() {
		GameObject obj = Object.Instantiate(cityType) as GameObject;
		AccountForMissingUnitScript(obj);
		RegisterSpecialTerrain(obj);
		
		SpecialTerrainScript unitScript = obj.GetComponent<SpecialTerrainScript>();
		obj.name = "City";
		unitScript.terrainDescription = "City\n+4 AP";
		
		//Game.getInstance().specialTerrains.Add(unitScript);
		
		// add AP every turn!!!
		unitScript.PerformOnNewTurn = (UnitScript us)=>{ 
			Game.getInstance().teams[us.GetTeam()].apBuf += 4;
		};
		
		foreach(Renderer r in obj.GetComponentsInChildren<Renderer>())
			r.material.shader = Shader.Find("Transparent/Diffuse");
		return obj;
	}
	
	public GameObject createBarracks() {
		GameObject obj = Object.Instantiate(barrackType) as GameObject;
		AccountForMissingUnitScript(obj);
		SpecialTerrainScript unitScript = obj.GetComponent<SpecialTerrainScript>();
		obj.name = "Barracks";
		unitScript.terrainDescription = "Barracks\n+20 ATK";
		unitScript.GuiXOffset -= 25;
		unitScript.GuiYOffset += 15;
		
		RegisterSpecialTerrain(obj);

		// add AP every turn!!!
		unitScript.PerformOnNewTurn = (UnitScript us)=>{ 
			Game.getInstance().teams[us.GetTeam()].atkBuf += 20;
		};
		
		foreach(Renderer r in obj.GetComponentsInChildren<Renderer>())
			r.material.shader = Shader.Find("Transparent/Diffuse");
		return obj;
	}
	
	public GameObject createFort() {
		GameObject obj = Object.Instantiate(fortType) as GameObject;
		AccountForMissingUnitScript(obj);
		SpecialTerrainScript unitScript = obj.GetComponent<SpecialTerrainScript>();
		obj.name = "Fort";
		unitScript.terrainDescription = "Fort\n+20 DEF";
		unitScript.GuiXOffset -= 25;
		unitScript.GuiYOffset -= 50;
		
		RegisterSpecialTerrain(obj);
		
		//Game.getInstance().specialTerrains.Add(unitScript);
		
		// add AP every turn!!!
		unitScript.PerformOnNewTurn = (UnitScript us)=>{ 
			Game.getInstance().teams[us.GetTeam()].defBuf += 20;
		};
		
		
		foreach(Renderer r in obj.GetComponentsInChildren<Renderer>())
			r.material.shader = Shader.Find("Transparent/Diffuse");
		return obj;
	}
	
	
	/*
	 public GameObject create() {
		GameObject obj = Object.Instantiate(~*~*~*~something~*~8~*~) as GameObject;
		UnitScript unitScript = obj.GetComponent<UnitScript>();
		
		unitScript.PerformOnNewTurn = ()=>{Debug.Log ("I'm doing this on a new turn.");};
		
		foreach(Renderer r in obj.GetComponentsInChildren<Renderer>())
			r.material.shader = Shader.Find("Transparent/Diffuse");
		return obj;
	}
	*/
	
	public GameObject createFogOfWar() {
		GameObject obj;
		obj = Object.Instantiate(fogType) as GameObject;
		return obj;
	}
	
}
