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

    // ��������� �������� ����� � ����� ���� ��� ��������
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
        // �������� �� ������� ���� � ��������� �� ���, ��� ������� ��������
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

        // ������ �� ��������� ���?
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
    }

    // �������� ���� �� �����
    public IEnumerator Dead()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }

    // ��������� ���� ���� ��� ��������, �� ���, �� ��������...
    public void Crouch()
    {
        animator.SetTrigger("Crouch");
    }
}
