using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelGenerator : MonoBehaviour {

	public NavMeshSurface surface;
	public int width = 21;
	public int height = 21;

	public GameObject wall;
	public GameObject bot;

	public int initalBotsNumber = 4;
	private int currentBotNumber = 0;
	public List<GameObject> Bots;
	private List<string> BotNames = new List<string> { "Green", "White", "Black", "Yellow", "Blue", "Red", "Magenta", "Cyan"};
	private bool[,] boxMatr;

	// Use this for initialization
	void Start () {
		boxMatr = new bool[width, height];
		for (int x = 0; x < width; x++) 
        {
			for (int y = 0; y < height; y++)
				boxMatr[x, y] = false;
		}
		GenerateLevel();
		surface.BuildNavMesh();
		StartCoroutine(Wait());
		FindTargets();
	}
	
	// Create a grid based level
	void GenerateLevel()
	{
		// Loop over the grid
		for (int x = 0; x <= width; x+=2)
		{
			for (int y = 0; y <= height; y+=2)
			{
				// Should we place a wall?
				if (Random.value > .7f)
				{
					// Spawn a wall
					Vector3 pos = new Vector3(x - width / 2f, 1f, y - height / 2f);
					Instantiate(wall, pos, Quaternion.identity, transform);
					boxMatr[x, y] = true;
				} 
			}
		}

		while (initalBotsNumber > currentBotNumber)
        {
			int x = Random.Range(0, width);
			int y = Random.Range(0, height);
			if (!boxMatr[x,y])
            {
				Vector3 pos = new Vector3(x - width / 2f, 1.25f, y - height / 2f);
				SpawnBot(pos);
				boxMatr[x, y] = true;
			}
		}
	}

	public void SpawnBot(Vector3 pos)
    {
		GameObject newbot = Instantiate(bot, pos, Quaternion.identity);
		Bots.Add(newbot);
		PlayerController botController = newbot.GetComponent<PlayerController>();
		botController.damage = Random.Range(5, 21);
		botController.botName = BotNames[currentBotNumber];
		switch (currentBotNumber)
		{
			case 0:
				botController.botColor = Color.green;
				break;
			case 1:
				botController.botColor = Color.white;
				break;
			case 2:
				botController.botColor = Color.black;
				break;
			case 3:
				botController.botColor = Color.yellow;
				break;
			case 4:
				botController.botColor = Color.blue;
				break;
			case 5:
				botController.botColor = Color.red;
				break;
			case 6:
				botController.botColor = Color.magenta;
				break;
			case 7:
				botController.botColor = Color.cyan;
				break;
			default:
				botController.botColor = Color.gray;
				break;
		}
		currentBotNumber++;
	}

	public void FindTargets()
    {
		for (int i = 0; i < currentBotNumber; i++)
		{
			GameObject closest = null;
			float dist = Mathf.Infinity;
			foreach (GameObject go in Bots)
			{
				if (Bots[i] != go) 
				{
					Vector3 diff = Bots[i].transform.position - go.transform.position;
					float curDistance = diff.magnitude;
					if (curDistance < dist)
					{
						closest = go;
						dist = curDistance;
					}
				}
				if (dist != Mathf.Infinity)
				{
					//Bots[i].GetComponent<NavMeshAgent>().SetDestination(closest.transform.position);
					Bots[i].GetComponent<PlayerController>().target = go;
					go.GetComponent<PlayerController>().target = Bots[i];
				}
			}
		}
	}

	IEnumerator Wait()
	{
		yield return new WaitForSeconds(2);
	}
}
