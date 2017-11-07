using UnityEngine;
using System.Collections;
using System;

public class GameOfLife : MonoBehaviour {

	public enum States {
		Idle, Running
	}

	public Cell cellPrefab;

	public float updateInterval = 0.1f; // delay between cell updates

	[HideInInspector] public Cell[,] cells; // matrix of cells
	[HideInInspector] public States state = States.Idle;
	[HideInInspector] public int sizeX; // game size in x-axis
	[HideInInspector] public int sizeY; // game size in y-axis

	private Action cellUpdates; // action which calls cells' update methods
	private Action cellApplyUpdates; // action which calls cells' apply update methods

	private IEnumerator coroutine; // reference to main coroutine

	void Awake () {
		Init (50, 50); // init game with 50x50 cells

		Run (); // start update coroutine
	}

	public void Init (int x, int y) {
		// make sure that cells' matrix is empty and there is no cell object in the scene
		if (cells != null) {
			for (int i = 0; i < sizeX; i++) {
				for (int j = 0; j < sizeY; j++) {
					GameObject.Destroy (cells [i, j].gameObject);
				}
			}
		}

		// clear actions
		cellUpdates = null;
		cellApplyUpdates = null;

		coroutine = null;

		sizeX = x;
		sizeY = y;
		SpawnCells (sizeX, sizeY);
	}

	// this method invokes actions which call update and apply methods in cells
	public void UpdateCells () {
		cellUpdates ();
		cellApplyUpdates ();
	}

	public void SpawnCells (int x, int y) {
		cells = new Cell[x, y]; // create new cells' matrix
		for (int i = 0; i < x; i++) {
			for (int j = 0; j < y; j++) {
				Cell c = Instantiate (cellPrefab, new Vector3 ((float)i, (float)j, 0f), Quaternion.identity) as Cell; // create new cell in scene
				cells [i, j] = c;
				c.Init (this, i, j); // init cell by passing this object to it
				c.SetRandomState (); 
				// register cell's methods to proper actions
				cellUpdates += c.CellUpdate;
				cellApplyUpdates += c.CellApplyUpdate;
			}
		}

		// get and set references to neighbours for every cell
		for (int i = 0; i < x; i++) {
			for (int j = 0; j < y; j++) {
				cells [i, j].neighbours = GetNeighbours (i, j);
			}
		}
	}

	// create array with adjacent cells to cell with coordinates (x,y)
	public Cell[] GetNeighbours (int x, int y) {
		Cell[] result = new Cell[8];
		result[0] = cells[x, (y + 1) % sizeY]; // top
		result[1] = cells[(x + 1) % sizeX, (y + 1) % sizeY]; // top right
		result[2] = cells[(x + 1) % sizeX, y % sizeY]; // right
		result[3] = cells[(x + 1) % sizeX, (sizeY + y - 1) % sizeY]; // bottom right
		result[4] = cells[x % sizeX, (sizeY + y - 1) % sizeY]; // bottom
		result[5] = cells[(sizeX + x - 1) % sizeX, (sizeY + y - 1) % sizeY]; // bottom left
		result[6] = cells[(sizeX + x - 1) % sizeX, y % sizeY]; // left
		result[7] = cells[(sizeX + x - 1) % sizeX, (y + 1) % sizeY]; // top left
		return result;
	}

	// this method stops current coroutine and starts new its instance
	public void Run () {
		state = States.Running;
		if (coroutine != null)
			StopCoroutine (coroutine);
		coroutine = RunCoroutine ();
		StartCoroutine (coroutine);
	}

	private IEnumerator RunCoroutine () {
		while (state == States.Running) { // while simulation is running
			UpdateCells (); // update all cells in game
			yield return new WaitForSeconds (updateInterval); // just wait...
		}
	}
}