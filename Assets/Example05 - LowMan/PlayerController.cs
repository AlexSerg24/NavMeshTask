using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerController : MonoBehaviour
{
    public Camera cam;
    public NavMeshAgent agent;
    public ThirdPersonCharacter character;
    public Animator animator;
    public LevelGenerator level;

    public int damage;
    public int Score;
    public int maxHP = 100;
    public int currentHP;
    public string botName;
    public Color botColor;

    public GameObject fieldHP;
    public HealthBar healthBar;
    public GameObject fieldDamage;
    public GameObject fieldScore;
    public GameObject fieldName;
    public GameObject botBody;

    public GameObject target;

    void Start()
    {
        cam = FindObjectOfType<Camera>();
        level = FindObjectOfType<LevelGenerator>();
        agent.updateRotation = false;
        currentHP = maxHP;
        healthBar.SetMaxHealth(maxHP);
        fieldDamage.GetComponent<Text>().text = damage.ToString();
        fieldScore.GetComponent<Text>().text = Score.ToString();
        fieldName.GetComponent<Text>().text = botName.ToString();
        botBody.GetComponent<SkinnedMeshRenderer>().material.color = botColor;
    }

    public void SetNewScoreAndDamage(int dmg, int scr)
    {
        damage = dmg;
        Score = scr;
        fieldDamage.GetComponent<Text>().text = dmg.ToString();
        fieldScore.GetComponent<Text>().text = scr.ToString();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
        }*/

        if (target != null)
        {
            agent.SetDestination(target.transform.position);
        }

        if (agent.remainingDistance > agent.stoppingDistance)
        {
            character.Move(agent.desiredVelocity, false, false);
        }
        else
        {
            character.Move(Vector3.zero, false, false);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            TakeDamage(20);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        fieldHP.GetComponent<Text>().text = currentHP.ToString();
        healthBar.SetHealth(currentHP);

        if (currentHP <= 0)
        {
            animator.SetTrigger("DeathTrigger");
            PlayerController winBot = target.GetComponent<PlayerController>();
            winBot.SetNewScoreAndDamage(winBot.damage + 1, winBot.Score + 1);
            winBot.target = null;
            level.Bots.Remove(gameObject);
        }
    }

    public void DontMove()
    {
        character.Move(Vector3.zero, false, false);
    }
}
