using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitMoveScript : MonoBehaviour {

	UnitScript us;
	// Use this for initialization
	void Start () {
		
	}
	
	private UnitScript unitScript() {
		if(!us)
			us = gameObject.GetComponent<UnitScript>();
		return us;
	}
	
	// Update is called once per frame
	    //Make the units face the right direction.
    void DirectionStuff(TileScript destination)
    {
        if (destination.directionToGo == TileScript.Direction.LEFT)
            gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        else if (destination.directionToGo == TileScript.Direction.RIGHT)
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (destination.directionToGo == TileScript.Direction.UP)
            gameObject.transform.rotation = Quaternion.Euler(0, 90, 0);
        else if (destination.directionToGo == TileScript.Direction.DOWN)
            gameObject.transform.rotation = Quaternion.Euler(0, 270, 0);
    }
		
	// Stuff for movement
	public bool doMovement;
	float startTime;
	Vector3 aboveDest;
	Vector3 aboveSource;
	TileScript destination;
	static float moveTime = 0.3f;
	bool doUpdateFog = true;
	// Update is called once per frame
	void Update () {
		
		if(doMovement) { // just the animation
			
			float time = (Time.time - startTime) / (moveTime * (float)destination.GetImpedence());
			gameObject.transform.position = Vector3.Lerp(aboveSource, aboveDest, time);	
			
			// Update fog!
			if(doUpdateFog && time >= 0.5) {
				List<TileScript> visibles = 
					Game.getInstance().movHandler.visibleTilesFrom(destination, unitScript().visibilityRange);
				foreach(TileScript s in  visibles) {
					Game.getInstance().teams[unitScript().GetTeam()].foundTiles.Add(s);
				}
				
				Game.getInstance().DoFogOfWar();
				doUpdateFog = false;
			}
			if(time >= 1) {
				doMovement = false;
				doUpdateFog = true;
				if(destList!=null && ++destListIndex != destList.Count){ 
					moveTo (destList[destListIndex]);
                    DirectionStuff(destList[destListIndex-1]);
				} else {
					
					Game.getInstance().movHandler.DeselectTile(null);
					Game.getInstance().movHandler.SelectTile(unitScript().parentTile);
					Game.getInstance().selectedTile = unitScript().parentTile;
					Object.Destroy(this); // will it go kaboom?
				}
			}
		}
	}
	
	public void moveTo(TileScript dest) {
		unitScript().parentTile = dest;
		aboveDest = dest.Pos() + new Vector3(0f, 1f, 0f);
		aboveSource = unitScript().Pos() + new Vector3(0f, 0f, 0f);
		startTime = Time.time;
		doMovement = true;
		destination = dest;
		
	}
	
	List<TileScript> destList;
	int destListIndex = 0;
	public void moveTo(List<TileScript> destList) {
		this.destList = destList;
		destListIndex = 0;
		if(destList.Count == 0) return;
		
		moveTo(destList[0]);
        DirectionStuff(destList[0]);
	}
}
