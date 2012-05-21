using UnityEngine;
using System.Collections.Generic;

public class MovementHandler : MonoBehaviour {
	
	public GameObject selectorType;
	public GameObject movementType;
	private GameObject mySelector;
	private List<TileScript> markedTiles;
	private List<TileScript> unVisitedTiles;
	private List<TileScript> visitedTiles;
	public TileMap tileMap;
	public int minX;
	public int maxX;
	public int minY;
	public int maxY;
	public int range;
	
	// Use this for initialization
	void Start () {
		markedTiles = new List<TileScript>();
		unVisitedTiles = new List<TileScript>();
		visitedTiles = new List<TileScript>();
		tileMap = gameObject.GetComponent<TileMap>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void SelectTile(TileScript tileScript) {
		
		if(!mySelector)
			mySelector = Object.Instantiate(selectorType) as GameObject;
		
		TileMap tileMap = gameObject.GetComponent<TileMap>();
		
		mySelector.gameObject.light.color = Color.yellow;
			//Game.getInstance().GetCurrentTeam().color;
		
		putObjectAboveTile(mySelector, tileScript, 0.75f);
		
		if(tileScript.specialTerrain) {
			UnitScript specialTerrainScript = tileScript.specialTerrain.GetComponent<UnitScript>();
			if(specialTerrainScript != null) {
				
			}
		}
		
		
        //Put lights over each Tile; 
        if (tileScript.unit && tileScript.GetUnitScript().CanMove())
        {
            // The Selector light

            // Don't display movement tiles for enemies.
            if (tileScript.GetUnitScript().GetTeam() != Game.getInstance().currentTeam)
                return;

            int range = Mathf.Min(
                tileScript.GetUnitScript().RemainingMovement(),
                Game.getInstance().GetCurrentTeam().actionPoints
            );

            minX = Mathf.Max(tileScript.x - range, 0);
            maxX = Mathf.Min(tileScript.x + range + 1, tileMap.width - 1);
            minY = Mathf.Max(tileScript.y - range, 0);
            maxY = Mathf.Min(tileScript.y + range + 1, tileMap.height - 1);

            //Add all unvisited nodes to the list
            for (int i = minX; i <= maxX; i++)
            {
                for (int j = minY; j <= maxY; j++)
                {
                        if (distBetween(tileScript, tileMap.At(i, j)) > range) continue;
						
						bool visible = Game.getInstance().GetCurrentTeam().foundTiles.Contains(tileMap.At(i,j));
						
                        if(tileMap.At(i,j).tileType == TileScript.TileType.WATER && visible) continue;
                        if (tileMap.At(i, j).unit != null && tileMap.At(i, j).GetUnitScript().GetTeam() == tileScript.GetUnitScript().GetTeam()) continue;
                        if (tileMap.At(i, j).unit != null && tileMap.At(i, j).GetUnitScript().GetVisible()) continue;
                        if (tileMap.At(i, j).unit == tileScript) continue;

                    unVisitedTiles.Add(tileMap.At(i, j));
                }
            }

            tileScript.dist = 0;
            range = 25;
            getTiles(tileScript, range);

            for (int k = 0; k < visitedTiles.Count; k++)
            {
                // Added a shitty fix but anyway visitedTiles shouldn't have a repeat tile!!
                if (markedTiles.Contains(visitedTiles[k]))
                {
                    continue;
                    //Debug.Log ("uh what???" + k + ", " +  visitedTiles[k].x + ", " + visitedTiles[k].y);
                }
                GameObject movementMarker = Object.Instantiate(movementType) as GameObject;
                putObjectAboveTile(movementMarker, visitedTiles[k].x, visitedTiles[k].y, 0.6f);
                movementMarker.transform.parent = visitedTiles[k].gameObject.transform;
                markedTiles.Add(visitedTiles[k]);
                visitedTiles[k].marker = movementMarker;
            }

            if (range == 0)
            {
                mySelector.gameObject.light.color = Color.red;
            }
			
			Game.getInstance().RefreshUnitDescriptions();
        }
				
	}
	
	int ModdedImpedence(TileScript source, int x, int y)
	{
		int imp = getTile(unVisitedTiles, source.x + x, source.y + y).GetImpedence();
		bool visible = Game.getInstance().GetCurrentTeam().foundTiles.Contains(tileMap.At(source.x + x,source.y + y));
		imp = visible ? imp : 1;
		
		return imp;
	}
	
	// This = Dijkstra's Algorithm
	public void getTiles(TileScript source, int Range){

		if(source.dist <= Range)
		{
			//check if left tile is in the list
            if (source.x > minX && unVisitedTiles.Contains(tileMap.At(source.x - 1, source.y)))
            {
                if (getTile(unVisitedTiles, source.x - 1, source.y).dist > source.dist + ModdedImpedence(source, -1, 0))
                {
                     getTile(unVisitedTiles, source.x - 1, source.y).dist = source.dist + ModdedImpedence(source, -1, 0);
                     getTile(unVisitedTiles, source.x - 1, source.y).directionFrom = TileScript.Direction.RIGHT;
                }
            }
			//check if right tile is in the list
            if (source.x < maxX && unVisitedTiles.Contains(tileMap.At(source.x + 1, source.y)))
            {
                if (getTile(unVisitedTiles, source.x + 1, source.y).dist > source.dist + ModdedImpedence(source, 1, 0))
                {
                    getTile(unVisitedTiles, source.x + 1, source.y).dist = source.dist + ModdedImpedence(source, 1, 0);
                    getTile(unVisitedTiles, source.x + 1, source.y).directionFrom = TileScript.Direction.LEFT;            
                }
            }
			//check if up tile is in the list
            if (source.y > minY && unVisitedTiles.Contains(tileMap.At(source.x, source.y - 1)))
            {
                if (getTile(unVisitedTiles, source.x, source.y - 1).dist > source.dist + ModdedImpedence(source, 0, -1))
                {
                    getTile(unVisitedTiles, source.x, source.y - 1).dist = source.dist + ModdedImpedence(source, 0, -1);
                    getTile(unVisitedTiles, source.x, source.y - 1).directionFrom = TileScript.Direction.UP;
                }
            }
			//check if down tile is in the list
            if (source.y < maxY && unVisitedTiles.Contains(tileMap.At(source.x, source.y + 1)))
            {
                if (getTile(unVisitedTiles, source.x, source.y + 1).dist > source.dist + ModdedImpedence(source, 0, 1))
                {
                    getTile(unVisitedTiles, source.x, source.y + 1).dist = source.dist + ModdedImpedence(source, 0, 1);
                    getTile(unVisitedTiles, source.x, source.y + 1).directionFrom = TileScript.Direction.DOWN;
                }
            }
			//Sorts the list so the closest tiles are at the front.
			unVisitedTiles.Sort((x,y) => x.dist.CompareTo(y.dist));
			source.visited = true;
			//Current tile is added to the visited.
			TileScript visitedTileToAdd = getTile (unVisitedTiles, source.x, source.y);
			if(visitedTileToAdd != null)
			{
				visitedTiles.Add(visitedTileToAdd);
			}
			unVisitedTiles.Remove(source);
			//If unvisited empties then exit dijkstra's *Wasted time :(*
			if(unVisitedTiles.Count != 0)
				getTiles(unVisitedTiles[0], Range);
			
		}
	}
	
	public void DeselectTile(TileScript tileScript) {
		
		foreach(TileScript t in markedTiles) {
			Object.Destroy(t.marker);
		}
		
		for(int x = 0; x < tileMap.width; x++) {
			for(int y = 0; y < tileMap.height; y++) {
				TileScript t = tileMap.At (x,y);
				t.marker = null;
				t.dist = 999999;
				t.visited = false;
			}
		}
		
		markedTiles.Clear();
		unVisitedTiles.Clear();
		visitedTiles.Clear();
		if(mySelector)
 		   	mySelector.transform.position = new Vector3(0f, -400f, 0f);
		
		Game.getInstance().RefreshUnitDescriptions();
	}
	
	private void putObjectAboveTile(GameObject o, int x, int y, float height=1f) {
		TileMap tileMap = gameObject.GetComponent<TileMap>();
		o.transform.position = tileMap.At(x, y).Pos() + new Vector3(0f, height, 0f);
	}
	
	private void putObjectAboveTile(GameObject o, TileScript t, float height=1f) {
		putObjectAboveTile(o, t.x, t.y, height);
	}
	
	private static int distBetween(TileScript a, TileScript b) {
		return Mathf.Abs(a.x - b.x) + Mathf.Abs (a.y - b.y);
	}
	
	public bool MoveUnit(TileScript source, TileScript dest) {
		if(!markedTiles.Contains (dest)) return false;
		if(!source.unit) return false;

        if (source.GetUnitScript().CanMove())
        {

            source.GetUnitScript().timesMoved++;
            Debug.Log("Moved " + source.GetUnitScript().timesMoved + "times.");
            UnitScript unitToMove = source.GetUnitScript();


            //THIS WHOLE WHILE LOOP IS JUST TO FIND WHICH WAY TO GO FIRST.
            //FROM HERE
            TileScript tempSource = dest;
            while (tempSource != source)
            {

                if (tempSource.directionFrom == TileScript.Direction.LEFT)
                {
                    tempSource = tileMap.At(tempSource.x - 1, tempSource.y);
                    tempSource.directionToGo = TileScript.Direction.RIGHT;
                    if (tempSource == source)
                    {
                        source.directionToGo = TileScript.Direction.RIGHT;
                        break;
                    }
                }
                else if (tempSource.directionFrom == TileScript.Direction.RIGHT)
                {
                    tempSource = tileMap.At(tempSource.x + 1, tempSource.y);
                    tempSource.directionToGo = TileScript.Direction.LEFT;
                    if (tempSource == source)
                    {
                        source.directionToGo = TileScript.Direction.LEFT;
                        break;
                    }
                }
                else if (tempSource.directionFrom == TileScript.Direction.DOWN)
                {
                    tempSource = tileMap.At(tempSource.x, tempSource.y - 1);
                    tempSource.directionToGo = TileScript.Direction.UP;
                    if (tempSource == source)
                    {
                        source.directionToGo = TileScript.Direction.UP;
                        Debug.Log("source will go " + source.directionToGo);
                        break;
                    }
                }
                else if (tempSource.directionFrom == TileScript.Direction.UP)
                {
                    tempSource = tileMap.At(tempSource.x, tempSource.y + 1);
                    tempSource.directionToGo = TileScript.Direction.DOWN;
                    if (tempSource == source)
                    {
                        source.directionToGo = TileScript.Direction.DOWN;
                        break;
                    }
                }
            }
            //TO HERE

            // This makes the animations better so the idiot unit doesn't turn left at the end of moving
            if (dest.directionFrom == TileScript.Direction.LEFT)
                dest.directionToGo = TileScript.Direction.RIGHT;
            else if (dest.directionFrom == TileScript.Direction.RIGHT)
                dest.directionToGo = TileScript.Direction.LEFT;
            else if (dest.directionFrom == TileScript.Direction.UP)
                dest.directionToGo = TileScript.Direction.DOWN;
            else if (dest.directionFrom == TileScript.Direction.DOWN)
                dest.directionToGo = TileScript.Direction.UP;


            //THE ACTUAL MOVEMENT.

            List<TileScript> tilesToMoveTo = new List<TileScript>();

            while (source != dest)
            {
                Game.Team team = GetComponent<Game>().GetCurrentTeam();

                team.actionPoints -= source.GetImpedence();
                unitToMove.tilesMovedThisTurn += source.GetImpedence();

                if (source.directionToGo == TileScript.Direction.LEFT)
                {
					if(!FoundInterruption(tilesToMoveTo, source, -1, 0))
                        source = tileMap.At(source.x - 1, source.y);
                    else
                        break;
				}
                else if (source.directionToGo == TileScript.Direction.RIGHT)
                {
					if(!FoundInterruption(tilesToMoveTo, source, 1, 0))
                        source = tileMap.At(source.x + 1, source.y);
                    else
                        break;
                }
                else if (source.directionToGo == TileScript.Direction.DOWN)
                {
					if(!FoundInterruption(tilesToMoveTo, source, 0, -1))
                        source = tileMap.At(source.x, source.y-1);
                    else
                        break;
                }
                else if (source.directionToGo == TileScript.Direction.UP)
                {
					if(!FoundInterruption(tilesToMoveTo, source, 0, 1))
                        source = tileMap.At(source.x, source.y + 1);
                    else
                        break;
                }
            }
			
			if(foundWater) {
				foundWater = false;
				if(unitToMove.tilesMovedThisTurn < unitToMove.range) {
					unitToMove.timesMoved--;
					unitToMove.tilesMovedThisTurn--;
				}
				Debug.Log ("FUCK THIS CODE");
			}


            /*
            foreach(TileScript tile in tilesToMoveTo) {
                foreach(TileScript visTile in visibleTilesFrom(tile)) {
                    Game.getInstance().GetCurrentTeam().foundTiles.Add (visTile);
                }
            }
            */

            // reset soruce to the original
            source = unitToMove.parentTile;

            unitToMove.gameObject.AddComponent<UnitMoveScript>().moveTo(tilesToMoveTo); // animation
            tilesToMoveTo[tilesToMoveTo.Count - 1].AddUnit(source.unit); // sets unit's new location
            source.unit = null;
			
			if(unitToMove.CanMove())
				unitToMove.tilesMovedThisTurn = 0;


            //Game.getInstance().UnitMovedCallback();
            return true;
        }
        else
		    return false;
	}
	
	public List<GameObject> canUseAbilityOn(TileScript attacker) {
		if(!attacker.unit)
			return new List<GameObject>();
		
		List<GameObject> list = new List<GameObject>();
		TileMap tileMap = gameObject.GetComponent<TileMap>();
        
		int range = attacker.GetUnitScript().atkRange;
		
		int minX = Mathf.Max(attacker.x - range, 0);
		int maxX = Mathf.Min(attacker.x + range+1, tileMap.width-1);
		int minY = Mathf.Max(attacker.y - range, 0);
		int maxY = Mathf.Min(attacker.y + range+1, tileMap.height-1);
		
		// Calculate which tiles to call 'attackable'
		for(int i = minX; i <= maxX; i++) {
			for(int j = minY; j <= maxY; j++) {
				TileScript dest = tileMap.At (i, j);
				
				if(i == attacker.x && j == attacker.y) continue;
				if(distBetween(attacker, dest) > range) continue;
                if (dest.specialTerrain && dest.specialTerrain.name == "Tree")
                {   // Can only attack someone in a tree if you are next to them.
                    if (distBetween(attacker, dest) > 1) continue;
                }

				if(dest.unit) { 
					// thats for attack only
					//if(attacker.GetUnitScript().GetTeam() == dest.GetUnitScript().GetTeam()) continue;
					list.Add(dest.gameObject);
				}
			}
		}
		
		return list;
	}
	
	bool foundWater = false;
	
	bool FoundInterruption(List<TileScript> tilesToMoveTo, TileScript source, int x, int y) {
		TileScript tile = tileMap.At(source.x + x, source.y + y);
		bool isUnit = tile.unit != null;
		bool foundWater = tile.tileType == TileScript.TileType.WATER;
		
		if (!isUnit && !foundWater )
        {
            tilesToMoveTo.Add(tile);
            source = tileMap.At(source.x + x, source.y + y);
			return false;
        }
        else if(foundWater) {
        	this.foundWater = true;
        	return true;
        } else {
        	return true;
        }
		
		throw new System.Exception("Shouldn't get here");
		return true;
	}
	
		
	public List<TileScript> visibleTilesFrom(TileScript from) {
		int range = from.GetUnitScript().visibilityRange;
		return visibleTilesFrom(from, range);
	}
	
	public List<TileScript> visibleTilesFrom(TileScript from, int range) {
		
		// range of 1 = see 5 tiles
		range++;
		
		// Can't see anything from a unit-less tile
		List<TileScript> list = new List<TileScript>();
		TileMap tileMap = gameObject.GetComponent<TileMap>();
		
		int minX = Mathf.Max(from.x - range, 0);
		int maxX = Mathf.Min(from.x + range+1, tileMap.width-1);
		int minY = Mathf.Max(from.y - range, 0);
		int maxY = Mathf.Min(from.y + range+1, tileMap.height-1);
		
		// Calculate which tiles to call 'attackable'
		for(int i = minX; i <= maxX; i++) {
			for(int j = minY; j <= maxY; j++) {
				TileScript dest = tileMap.At (i, j);
				
				if(distBetween(from, dest) <= range)
					list.Add(dest);
			}
		}
		
		return list;
	}
	
	
	// Searches a list for the tile at (x,y) and returns it. If no tile is found it returns the first tile. So don't use it unless you know it's there
	public TileScript getTile(List<TileScript> tileList, int x, int y)
	{
		for(int i = 0; i < tileList.Count; i++)
		{
			if(tileList[i].x == x && tileList[i].y == y)
				return tileList[i];
		}
		return null;
	}
	
}
