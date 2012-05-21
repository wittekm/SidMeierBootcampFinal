using System;
using UnityEngine;

public class Ability
{
	public string name;
	public int actionPoints;
	public Func<TileScript, TileScript, int> func;
	public Func<TileScript, TileScript, bool> predicate;
	public string description;
	
	public Ability ()
	{
		
	}
	
	public static bool SameTeam(TileScript atk, TileScript def) {
		if(atk.unit && def.unit) {
			return atk.GetUnitScript().GetTeam() == def.GetUnitScript().GetTeam();
		}
		return false;
	}
	
	public static bool DifferentTeam(TileScript atk, TileScript def) {
		if(atk.unit && def.unit) {
			return atk.GetUnitScript().GetTeam() != def.GetUnitScript().GetTeam();
		}
		return false;
	}
		
	public Ability (string n, int ap, Func<TileScript, TileScript, int> f, Func<TileScript, TileScript, bool> pred)
	{
		name = n;
		actionPoints = ap;
		func = f;
		predicate = pred;
		description = name + " (" + actionPoints + " AP)";
	}
	
	public void exec(TileScript atk, TileScript def) {
		if(func(atk, def) != 0)
			Game.getInstance().GetCurrentTeam().actionPoints -= actionPoints;
		updateWorldAfterAbility(atk, def);
	}
	
	private void updateWorldAfterAbility(TileScript atk, TileScript def) {
		// Check for death;
		if(!def || !def.unit) return;
		if(def.GetUnitScript().health <= 0) {
			def.RemoveUnit();
			def.Explosion();
			Game.getInstance().UnBattle();
			Game.getInstance().CheckWin();
		}
	}
	
}
