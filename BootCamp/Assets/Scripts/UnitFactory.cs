using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UnitFactory : MonoBehaviour {
	public GameObject circleUnitType;
	public GameObject constructionUnitType;
    public GameObject normandyUnitType;
    public GameObject lamboUnitType;
    public GameObject centurionUnitType;
    public GameObject predatorUnitType;
    public GameObject healerUnitType;
    public GameObject archerUnitType;
    public GameObject wizardUnitType;
	
	private static UnitFactory instance;
	public Dictionary<string, System.Func<GameObject>> creationTable;

		
	public static UnitFactory getInstance() {
		return instance;
	}
	
	public UnitFactory() {
		instance = this;
		creationTable = new Dictionary<string, System.Func<GameObject>>();
		creationTable.Add("commander", createCircleUnit);
		creationTable.Add("construction", createConstructionGuy);
		creationTable.Add("normandy",createNormandy);
		creationTable.Add("lambo",createLambo);
		creationTable.Add("centurion",createCenturion);
		creationTable.Add("predator",createPredator);
		creationTable.Add("healer",createHealer);
		creationTable.Add("archer", createArcher);
        creationTable.Add("wizard", createWizard);
	}
	
	public GameObject create(string type) {
		if(!creationTable.ContainsKey(type))
			throw new System.Exception("cant make this");
		
		System.Func<GameObject> func = null;
		creationTable.TryGetValue(type, out func);
		return func();
	}
	
	
	// Use this for initializatiosn
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	public GameObject createCircleUnit() {
		GameObject obj = Object.Instantiate(circleUnitType) as GameObject; //new GameObject("CircleUnit");
		obj.name = "Commander";
		UnitScript unitScript = obj.GetComponent<UnitScript>();
		
		unitScript.abilities.Add(new Ability(
			"inflict 30 dmg",
			5,
			// where atk and def are TileScripts
			(atk, def) => { def.GetUnitScript().health -= 30; return 1; },
		
			// can I use this ability on def?
            Ability.DifferentTeam
			));
		unitScript.GuiXOffset -= 5;
		unitScript.GuiYOffset += 25;
		
		Game.getInstance().units.Add (unitScript);
		return obj;
	}
	
	Ability basicAttack = new Ability(
			"basic attack",
			2,
			// where atk and def are TileScripts
			(TileScript atk, TileScript def) => { def.TinyExplosion(); atk.GetUnitScript().attackUnit(def.GetUnitScript()) ; return 1; },
		
			// can I use this ability on def?
			Ability.DifferentTeam
		);
	
	public GameObject createConstructionGuy() {
		GameObject obj = Object.Instantiate(constructionUnitType) as GameObject; //new GameObject("CircleUnit");
		obj.name = "Construction Dude";
		UnitScript unitScript = obj.GetComponent<UnitScript>();		
		
		unitScript.abilities.Add(basicAttack);
		
		unitScript.GuiXOffset -= 10;
		
		Game.getInstance().units.Add (unitScript);
		return obj;
	}
	
		
	public GameObject createHealer() {
		GameObject obj = Object.Instantiate(healerUnitType) as GameObject;
		obj.name = "Priest";
		UnitScript unitScript = obj.GetComponent<UnitScript>();		
		
		unitScript.abilities.Add(new Ability("Heal 50", 4, 
			(TileScript atk, TileScript def) => {
				if(def.GetUnitScript().health == 100) {
					Game.getInstance().consoleString = "Can't overheal!";
					return 0;
				}
				UnitScript unit = def.GetUnitScript();
				unit.health = Mathf.Min (unit.health+50, 100);
				return 1; 
		},
			Ability.SameTeam
		));
		
		unitScript.abilities.Add (basicAttack);
		
		unitScript.GuiXOffset -= 10;
		
		Game.getInstance().units.Add (unitScript);
		return obj;
	}
	
	public GameObject createLambo() {
		/*GameObject obj = Object.Instantiate(lamboUnitType) as GameObject;
		obj.name = "Scout";
		UnitScript unitScript = obj.GetComponent<UnitScript>();		
		
		unitScript.abilities.Add(new Ability("Reduce Range", 4, 
			(TileScript atk, TileScript def) => {
				UnitScript us = def.GetUnitScript();
				us.range = Mathf.Max (1, us.range - 1);
				return 1;
			},
			Ability.DifferentTeam
		));
		
		unitScript.abilities.Add(new Ability("Lambo Explosion", 2, 
			(TileScript atk, TileScript def) => {
				UnitScript atkUnit = atk.GetUnitScript();
				UnitScript defUnit = def.GetUnitScript();
			
				if(atkUnit.health < 2) {
					Game.getInstance().consoleString = "Not enough health!";
					return 0;
				}
				atk.TinyExplosion();
				def.TinyExplosion();
				atkUnit.health--;
				atkUnit.attackUnit(defUnit);
				return 1;
			},
			Ability.DifferentTeam
		));
		
		unitScript.GuiXOffset -= 10;
        unitScript.GuiYOffset += 20;
		
		Game.getInstance().units.Add (unitScript);
		return obj;*/
        GameObject obj = Object.Instantiate(lamboUnitType) as GameObject;
        obj.name = "Lambo";
        UnitScript unitScript = obj.GetComponent<UnitScript>();

        unitScript.abilities.Add(basicAttack);

        unitScript.GuiXOffset -= 10;
        unitScript.GuiYOffset += 20;

        Game.getInstance().units.Add(unitScript);
        return obj;
	}
	
	public GameObject createArcher() {
		GameObject obj = Object.Instantiate(archerUnitType) as GameObject;
		obj.name = "Archer";
		UnitScript unitScript = obj.GetComponent<UnitScript>();	
		
		unitScript.abilities.Add (basicAttack);
		
		unitScript.GuiXOffset -= 10;
		
		Game.getInstance().units.Add (unitScript);
		return obj;
	}
	
	public GameObject createNormandy() {
		GameObject obj = Object.Instantiate(normandyUnitType) as GameObject;
		obj.name = "SSV Normandy";
		UnitScript unitScript = obj.GetComponent<UnitScript>();	
		
		unitScript.abilities.Add (basicAttack);
		
		unitScript.GuiXOffset -= 10;
        unitScript.GuiYOffset += 20;
		
		Game.getInstance().units.Add (unitScript);
		return obj;
	}
	
	public GameObject createCenturion() {
		GameObject obj = Object.Instantiate(centurionUnitType) as GameObject;
		obj.name = "Toaster";
		UnitScript unitScript = obj.GetComponent<UnitScript>();		
		
		unitScript.abilities.Add (basicAttack);
		
		unitScript.GuiXOffset -= 10;
		
		Game.getInstance().units.Add (unitScript);
		return obj;
	}
	
	public GameObject createPredator() {
		GameObject obj = Object.Instantiate(predatorUnitType) as GameObject;
		obj.name = "Predator";
		UnitScript unitScript = obj.GetComponent<UnitScript>();	
		
		unitScript.abilities.Add (basicAttack);
		
		unitScript.GuiXOffset -= 10;
		
		Game.getInstance().units.Add (unitScript);
		return obj;
	}
    public GameObject createWizard()
    {
        GameObject obj = Object.Instantiate(wizardUnitType) as GameObject;
        obj.name = "Wizard";
        UnitScript unitScript = obj.GetComponent<UnitScript>();
		
		unitScript.abilities.Add (basicAttack);

        unitScript.GuiXOffset -= 10;

        Game.getInstance().units.Add(unitScript);
        return obj;
    }
	
	
}
