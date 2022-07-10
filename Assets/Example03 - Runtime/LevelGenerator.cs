using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelGenerator : MonoBehaviour {

	public NavMeshSurface surface;
	private JsonController json;
	public int width = 21;
	public int height = 21;

	public GameObject wall;
	public GameObject bot;
	public GameObject scoreText;

	private int initalBotsNumber = 4;
	public int currentBotNumber = 0;
	private int maxBotNumber;
	public List<GameObject> Bots;
	private string[,] BotsInform;
	private string scoreInfo;
	private List<string> BotNames = new List<string> { "Green", "White", "Black", "Yellow", "Blue", "Red", "Magenta", "Cyan"};
	private bool[,] boxMatr;
	private bool[,] fightMatr;

	// Use this for initialization
	void Start () {
		json = gameObject.GetComponent<JsonController>();
		json.LoadField();
		json.saves.level = "Game";
		json.SaveAll(json.saves);
		initalBotsNumber = json.saves.botsNum;
		boxMatr = new bool[width, height];
		for (int x = 0; x < width; x++) 
        {
			for (int y = 0; y < height; y++)
				boxMatr[x, y] = false;
		}
		GenerateLevel();
		maxBotNumber = Bots.Count;
		surface.BuildNavMesh();
		StartCoroutine(FindTargets());
		BotsInform = new string[8, 3];
		for (int i = 0; i<currentBotNumber; i++)
        {
			PlayerController botInf = Bots[i].GetComponent<PlayerController>();
			BotsInform[i, 0] = botInf.botName;
			BotsInform[i, 1] = botInf.Score.ToString();
			BotsInform[i, 2] = botInf.damage.ToString();
		}
		ScoreListUpdate();
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
		botController.target = null;
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

	public IEnumerator FindTargets()
    {
		Debug.Log("Start finding");
		yield return new WaitForSeconds(5);
		for (int i = 0; i < currentBotNumber; i++)
		{
			GameObject closest = null;
			float dist = Mathf.Infinity;
			foreach (GameObject bot in Bots)
			{
				if (Bots[i] != bot) 
				{
					if (bot.GetComponent<PlayerController>().target == null)
					{
						Vector3 diff = Bots[i].transform.position - bot.transform.position;
						float curDistance = diff.magnitude;
						//Debug.Log(Bots[i].GetComponent<PlayerController>().botName + "curDistance " + curDistance + " to " + bot.GetComponent<PlayerController>().botName);
						if (curDistance < dist)
						{
							closest = bot;
							dist = curDistance;
						}
					}
				}				
			}
			if (dist != Mathf.Infinity)
			{
				//Bots[i].GetComponent<NavMeshAgent>().SetDestination(closest.transform.position);
				Bots[i].GetComponent<PlayerController>().target = closest;
				closest.GetComponent<PlayerController>().target = Bots[i];
			}
		}
	}

	public void TakeDamage(PlayerController damageBot, PlayerController damagedBot)
	{
		if (damageBot.currentHP > 0)
		{
			damageBot.currentHP -= damagedBot.damage;
			damageBot.fieldHP.GetComponent<Text>().text = damageBot.currentHP.ToString();
			damageBot.healthBar.SetHealth(damageBot.currentHP);
			if (damageBot.currentHP <= 0) damageBot.isDead = true;
		}
		if (damageBot.isDead)
		{
			damageBot.animator.SetTrigger("DeathTrigger");
			damagedBot.SetNewScoreAndDamage(damagedBot.damage + 1, damagedBot.Score + 1);
			for (int i = 0; i < maxBotNumber; i++) 
            {
				if (BotsInform[i,0] == damagedBot.botName)
				{
					BotsInform[i, 1] = damagedBot.Score.ToString();
					BotsInform[i, 2] = damagedBot.damage.ToString();
					break;
				}
			}
			ScoreListUpdate();
			damageBot.inBattle = false;
			damagedBot.inBattle = false;
			damagedBot.target = null;
			damagedBot.Crouch();
			Bots.Remove(damageBot.gameObject);
			currentBotNumber--;
			StartCoroutine(damageBot.Dead());
		}
	}
	public IEnumerator Battle(PlayerController firstBot, PlayerController secondBot)
	{
		if (!firstBot.isDead && !secondBot.isDead)
		{
			Debug.Log(firstBot.botName + " vs " + secondBot.botName);
			firstBot.Crouch();
			secondBot.Crouch();
			do
			{
				yield return new WaitForSeconds(1);
				if (!firstBot.isDead)
					TakeDamage(firstBot, secondBot);
				if (!secondBot.isDead)
					TakeDamage(secondBot, firstBot);
			} while (!firstBot.isDead && !secondBot.isDead);
			firstBot.inBattle = false;
			secondBot.inBattle = false;
			yield return new WaitForSeconds(1);
			StartCoroutine(FindTargets());
		}
		firstBot.inBattle = false;
		secondBot.inBattle = false;
	}

	private void ScoreListUpdate()
    {
		int maxScore = -1;
		int lider = -1;
		for (int i = 0; i < maxBotNumber; i++)
        {
			if (int.Parse(BotsInform[i,1]) > maxScore)
            {
				lider = i;
				maxScore = int.Parse(BotsInform[i, 1]);
			}
        }
		scoreInfo = BotsInform[lider,0] + ", score - " + BotsInform[lider, 1] + ", damage - " + BotsInform[lider, 2];
		do
		{
			for (int i=0; i< maxBotNumber; i++)
            {
				if (i != lider)
                {
					if (int.Parse(BotsInform[i, 1]) == maxScore)
                    {
						scoreInfo = scoreInfo + " \r\n" + BotsInform[i, 0] + ", score - " + BotsInform[i, 1] + ", damage - " + BotsInform[i, 2];
					}
				}
            }
			maxScore--;
		} while (maxScore >= 0);
		scoreText.GetComponent<Text>().text = scoreInfo;
	}

	public void ClickBackToMenu()
	{
		SceneManager.LoadScene("Load");
	}
}
