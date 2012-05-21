using UnityEngine;
using System.Collections;

public class SpecialTerrainScript : UnitScript {

	// Use this for initialization
	
	public string terrainDescription;
	public virtual string UnitDescription()
    {
		return terrainDescription;
	}
}
