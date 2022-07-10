using System.Collections;
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
    public bool isDead;
    public bool inBattle;

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
        isDead = false;
        inBattle = false;
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
            PlayerController targetContr = target.GetComponent<PlayerController>();
            //Debug.Log(botName + " target is " + targetContr.botName);
            if (Vector3.Distance(transform.position, target.transform.position) < 2.05f)
            {
                //Debug.Log("Should battle?");
                if (!inBattle && !targetContr.inBattle)
                {
                    inBattle = true;
                    targetContr.inBattle = true;
                    //StartCoroutine(Battle());
                    level.StartCoroutine(level.Battle(this, targetContr));
                }
            }
        }

        if (agent.remainingDistance > agent.stoppingDistance)
        {
            NavMeshPath path = new NavMeshPath();
            //Debug.Log(botName + "  remainingDistance = " + agent.remainingDistance + " calcDist - " + agent.CalculatePath(target.transform.position, path));
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
        if (currentHP > 0)
        {
            currentHP -= damage;
            fieldHP.GetComponent<Text>().text = currentHP.ToString();
            healthBar.SetHealth(currentHP);
            if (currentHP <= 0) isDead = true;
        } 
        if (isDead)
        {
            animator.SetTrigger("DeathTrigger");
            PlayerController winBot = target.GetComponent<PlayerController>();
            winBot.SetNewScoreAndDamage(winBot.damage + 1, winBot.Score + 1);
            inBattle = false;
            winBot.inBattle = false;
            winBot.target = null;
            level.Bots.Remove(gameObject);
            level.currentBotNumber--;
            StartCoroutine(Dead());
        }
    }

    IEnumerator Battle()
    {
        PlayerController targetContr = target.GetComponent<PlayerController>();
        do
        {
            yield return new WaitForSeconds(1);
            if (!targetContr.isDead)
            TakeDamage(targetContr.damage);
            if (!isDead)
            targetContr.TakeDamage(damage);
        } while (!isDead && !targetContr.isDead);
        inBattle = false;
        targetContr.inBattle = false;
        yield return new WaitForSeconds(1);
        StartCoroutine(FindOtherTarget());
    }
    public IEnumerator Dead()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }

    IEnumerator FindOtherTarget()
    {
        yield return new WaitForSeconds(2);
        level.StartCoroutine(level.FindTargets());
    }

    public void Crouch()
    {
        animator.SetTrigger("Crouch");
    }
}
