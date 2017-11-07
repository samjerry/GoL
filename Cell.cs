using UnityEngine;
using System.Collections;

public class Cell : MonoBehaviour {

	public enum States {
		Dead, Alive
	}

	public Material livingMaterial;
	public Material deadMaterial;

	[HideInInspector] public GameOfLife gameOfLife;
	[HideInInspector] public int x, y;
	[HideInInspector] public Cell[] neighbours;

	[HideInInspector] public States state;
	private States nextState;

	private MeshRenderer meshRenderer;

	void Awake () {
		meshRenderer = GetComponent <MeshRenderer> ();
	}

	// this method implements cells' behaviour
	public void CellUpdate () {
		nextState = state;
		int aliveCells = GetAliveCells ();
		if (state == States.Alive) { // if cell is alive
			if (aliveCells != 2 && aliveCells != 3) // if cell less than 2 and more than 3 alive neighbours
				nextState = States.Dead;
		} else { // if cell if dead
			if (aliveCells == 3) // if cell has 3 alive neighbours
				nextState = States.Alive;
		}
	}

	// apply new cell's state and update its material
	public void CellApplyUpdate () {
		state = nextState;
		UpdateMaterial ();
	}

	// pass parent object and store x-axis and y-axis coordinates
	public void Init (GameOfLife gol, int x, int y) {
		gameOfLife = gol;
		transform.parent = gol.transform;

		this.x = x;
		this.y = y;
	}

	// use it to set initial, random cell state
	public void SetRandomState () {
		state = (Random.Range (0, 2) == 0) ? States.Dead : States.Alive;
		UpdateMaterial ();
	}

	// change cell appearance based on its state
	private void UpdateMaterial () {
		if (state == States.Alive)
			meshRenderer.sharedMaterial = livingMaterial;
		else
			meshRenderer.sharedMaterial = deadMaterial;
	}

	// check cell's alive neighbours count
	private int GetAliveCells () {
		int ret = 0;
		for (int i = 0; i < neighbours.Length; i++) {
			if (neighbours[i] != null && neighbours [i].state == States.Alive)
				ret++;
		}
		return ret;
	}
}