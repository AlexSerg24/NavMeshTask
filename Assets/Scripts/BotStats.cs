using UnityEngine;
using UnityEngine.UI;

public class BotStats : MonoBehaviour
{
    public Stats damage;
    public Stats Score;

    public int maxHP = 100;
    public int currentHP;
    public string botName;
    public Color botColor;

    public GameObject fieldHP;


    void Awake()
    {
        currentHP = maxHP;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            TakeDamage(20);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        fieldHP.GetComponent<Text>().text = fieldHP.ToString();
    }
}
