using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour
{
    public static EndingManager Instance;
    public GameObject EndingPage;
    private bool hasShownEnding;

    public enum ENDING_TYPE
    {
        GOLD_ZERO,
        POPULARITY_ZERO,
        LOW_POP_LOW_GOLD,
        HIGH_POP_LOW_GOLD,
        LOW_POP_HIGH_GOLD,
        HIGH_POP_HIGH_GOLD,
    }

    public string[] EndingTexts;

    public void HandleGameEnding(ENDING_TYPE type)
    {
        EndingPage.SetActive(true);
        EndingPage.GetComponentInChildren<Text>().text = EndingTexts[(int)type];
    }

    public void HandleClick()
    {
        if(!hasShownEnding)
        {
            hasShownEnding = true;
            EndingPage.GetComponentInChildren<Text>().text = "THE END";
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }


    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        hasShownEnding = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
