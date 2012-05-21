using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TileMap : MonoBehaviour {
	static int GRASS = 0;
	static int WATER = 1;
	
    public GameObject tileObject; //<- make sure this has a material set on it.
	public GameObject tilePlaneObject; // This is a plane that goes below the Tiles to look nice!	
	
    public int width;
    public int height;
	
	public Texture2D[] textures;
	public int textureType;
	public StreamReader reader;
    public TileScript tileScript;
	
	public List<List<GameObject>> tiles;
	
	public float spacing;

    public float waterAnimationTimerStart;
    public float currentTime;
    public static float waterAnimationDuration = .01f;
    // Use this for initialization
    void Start () {
		tiles = new List<List<GameObject>>();
		
		FileInfo sourceFile = new FileInfo("Assets/Maps/Map2.txt");
		reader = sourceFile.OpenText();
		int.TryParse(reader.ReadLine(), out height);
		int.TryParse(reader.ReadLine(), out width);
		
		Debug.Log(height+ " " + width);
		
		this.CreateWorld();
		DontDestroyOnLoad(this);

	   //gameObject.GetComponent<Game>().ActuallyStart();	
    }

    // Update is called once per frame
    void Update () {

    }
	
	private void ReadMap() {
		for(int z=0; z<height; z++) {
			string line = reader.ReadLine();
			List<GameObject> sublist = new List<GameObject>();
			for(int x=0; x < width; x++) {
				// Create your tile object.
				GameObject tile = Object.Instantiate(tileObject) as GameObject;
				tile.transform.parent = gameObject.transform;
				DontDestroyOnLoad(tile); // for passing around
				// Change its position to this grid reference.
				tile.transform.position = new Vector3(x * spacing, tileObject.transform.position.y, z * spacing);
				
				tileScript = tile.GetComponent<TileScript>();
				
				tileScript.x = z; tileScript.y = x; // yeah deal with it
				int inType = line[x] - 48;
				
				switch(inType)
				{
					
					case(0)://grass
					{
						tileScript.tileType = TileScript.TileType.GRASS;
						tileScript.SetTexture(textures[(int)tileScript.tileType]);
						break;
					}
					
					case(1)://water
					{
						tileScript.tileType = TileScript.TileType.WATER;
						break;
					}
					case(2)://tree
					{
						tileScript.tileType = TileScript.TileType.GRASS;
						tileScript.SetTexture(textures[(int)tileScript.tileType]);
						tileScript.AddSpecialTerrain(ObjectFactory.getInstance().create("tree"));
						break;
					}
					case(3)://factory
					{
						tileScript.tileType = TileScript.TileType.GRASS;
						tileScript.SetTexture(textures[(int)tileScript.tileType]);
						tileScript.AddSpecialTerrain(ObjectFactory.getInstance().create("factory"));
						break;	
					}
					case(4)://city
					{
						tileScript.tileType = TileScript.TileType.GRASS;
						tileScript.SetTexture(textures[(int)tileScript.tileType]);
						tileScript.AddSpecialTerrain(ObjectFactory.getInstance().create("city"));
						break;	
					}
					case(5)://fort
					{
						tileScript.tileType = TileScript.TileType.GRASS;
						tileScript.SetTexture(textures[(int)tileScript.tileType]);
						tileScript.AddSpecialTerrain(ObjectFactory.getInstance().create("fort"));
						break;	
					}
					case(6)://barracks
					{
						tileScript.tileType = TileScript.TileType.GRASS;
						tileScript.SetTexture(textures[(int)tileScript.tileType]);
						tileScript.AddSpecialTerrain(ObjectFactory.getInstance().create("barracks"));
						//At(x,z).AddSpecialTerrain(specialTerrain);
						break;	
					}
					default:
						throw new System.Exception ("why you doin this shit mang? " + inType);
						break;
				}
				sublist.Add(tile);
			}
			//reader.Read();
			tiles.Add(sublist);
		}
		
		string line2;
		while((line2 = reader.ReadLine()) != null) {
			ReadSpecialTerrainLine(line2);
		}
		
		// Done reading tiles, now do the objects
		
	}
	
	private void ReadSpecialTerrainLine(string line) {
		if(line.Trim() == "") return;
		string[] split = line.Split(' ');
		if(split.Length < 3) throw new System.Exception(" Line: \""+line+"\" needs length >= 3");
		
		string type = split[2].ToLower();
		int x, y;
		int.TryParse(split[0], out x);
		int.TryParse(split[1], out y);
		
		if(x > width || y > height) throw new System.Exception("out of bounds bro");
		
		if(At (x, y).tileType == TileScript.TileType.WATER)
			throw new System.Exception("cant place on water");
		
		if(ObjectFactory.getInstance().creationTable.ContainsKey(type)) {
			GameObject specialTerrain = ObjectFactory.getInstance().create(type);
			At(x,y).AddSpecialTerrain(specialTerrain);
		}
		else if(UnitFactory.getInstance().creationTable.ContainsKey(type)) {
			if(split.Length < 4) throw new System.Exception("needs length >= 4");
			int team; int.TryParse(split[3], out team);
			GameObject unit = UnitFactory.getInstance().create (type);
			Game.getInstance().SpawnUnitAt(x, y, team, unit);
		}
		else {
			throw new System.Exception(x + " " + y + " " + type + " is not gonna work");
		}
	}

    private void CreateWorld() {
       ReadMap ();
		
	   GameObject plane = Object.Instantiate(tilePlaneObject) as GameObject;
	   Vector3 farthestTransform = At(width-1, height-1).Pos();
	   plane.transform.position = new Vector3(farthestTransform.x / 2f, 0, farthestTransform.z / 2f);
	   plane.transform.localScale = new Vector3(width * spacing, 0.9f, height * spacing);	   
    }
	
	private TileScript tileDown;
	private TileScript tileHoveredOver;
	
	public void tileDownCallback(TileScript tileScript) {
		//Debug.Log ("Down on " + tileScript.x + ", " + tileScript.y);
		tileDown = tileScript;
	}
	
	public void tileDragoverCallback(TileScript tileScript) {
		if(tileDown == null) return;
		
		//Debug.Log ("Drag over " + tileScript.x + " " + tileScript.y);
		tileHoveredOver = tileScript;
	}
	
	public void tileUpCallback(TileScript tileScript) {
		//Debug.Log ("Up on " + tileScript.x + ", " + tileScript.y);
		
		// If we down and up on same object
		// "Click"
		if(tileDown == tileHoveredOver) {
			gameObject.GetComponent<Game>().SetSelectorOnTile(tileScript);
		}
		tileDown = null;
	}
	
	public TileScript At(int x, int y) {
		if(x >= width || y >= height)
			Debug.Log("OOPS!!! " + x + " " + y);
		
		if(tiles == null)
			Debug.Log("OOOPSOSPDOPSA");
		
		GameObject tile = tiles[x][y];
		return tile.GetComponent<TileScript>();
	}
	
	public void Each( System.Action<TileScript> f) {
		foreach(List<GameObject> list in tiles) {
			foreach(GameObject obj in list) {
				f(obj.GetComponent<TileScript>());
			}
		}
	}
}