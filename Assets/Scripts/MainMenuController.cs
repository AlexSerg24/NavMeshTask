using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject LessBtn;
    public GameObject MoreBtn;
    public GameObject BotsNumber;

    private JsonController json;
    // Start is called before the first frame update
    void Start()
    {
        json = gameObject.GetComponent<JsonController>();
        json.LoadField();
        json.saves.level = "MainMenu";
        json.saves.botsNum = 4;
        json.SaveAll(json.saves);
    }

    public void LessBtnClick()
    {
        if(int.Parse(BotsNumber.GetComponent<Text>().text) > 2)
        {
            int num = int.Parse(BotsNumber.GetComponent<Text>().text) - 1;
            BotsNumber.GetComponent<Text>().text = num.ToString();
            json.saves.botsNum = num;
            json.SaveAll(json.saves);
            if (num == 2)
            {
                LessBtn.SetActive(false);
                MoreBtn.SetActive(true);
            }
            else
            {
                LessBtn.SetActive(true);
                MoreBtn.SetActive(true);
            }
        }
    }

    public void MoreBtnClick()
    {
        if (int.Parse(BotsNumber.GetComponent<Text>().text) < 8)
        {
            int num = int.Parse(BotsNumber.GetComponent<Text>().text) + 1;
            BotsNumber.GetComponent<Text>().text = num.ToString();
            json.saves.botsNum = num;
            json.SaveAll(json.saves);
            if (num == 8)
            {
                LessBtn.SetActive(true);
                MoreBtn.SetActive(false);
            }
            else
            {
                LessBtn.SetActive(true);
                MoreBtn.SetActive(true);
            }
        }
    }

    public void ClickPlay()
    {
        SceneManager.LoadScene("Load");
    }
}
