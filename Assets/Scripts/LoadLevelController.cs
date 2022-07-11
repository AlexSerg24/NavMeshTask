using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelController : MonoBehaviour
{
    private JsonController json;
    // Start is called before the first frame update
    void Start()
    {
        json = gameObject.GetComponent<JsonController>();
        json.LoadField();
    }

    // загрузка следующей сцены в соответствии с записанной в файле сценой
    public void LoadNextScene()
    {
        if (json.saves.level == "MainMenu")
        {
            SceneManager.LoadScene("Game");
        }
        else
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
