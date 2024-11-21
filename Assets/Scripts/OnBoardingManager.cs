using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnBoardingManager : MonoBehaviour
{
    public enum STAGE
    {
        OVERALL,
        OVERALL_1,
        OVERALL_2,
        GOLD,
        DAY,
        POPULARITY,
        START_GAME,
        BOARDING_END,
    }
    public STAGE CurrentStage;

    public Text TextNode;
    public GameObject Gold;
    public GameObject Day;
    public GameObject Popularity;
    // Start is called before the first frame update
    void Start()
    {
        CurrentStage = STAGE.OVERALL;
        HandleStageStart();
    }

    public void HandleStageStart()
    {
        switch(CurrentStage)
        {
            case STAGE.OVERALL:
                TextNode.text = "Greetings, boss!";
                break;
            case STAGE.OVERALL_1:
                TextNode.text = "Your job today(and everyday) is to choose a 'Good' news from variaty of them.";
                break;
            case STAGE.OVERALL_2:
                TextNode.text = "And then choose TITLE, BODY, and PICTURE for the post.";
                break;
            case STAGE.GOLD:
                TextNode.text = "This is how much money you have.\n(become rich!!!!)";
                Gold.SetActive(true);
                break;
            case STAGE.DAY:
                TextNode.text = "This is the date.\n(your shift will be end in FIVE days!)";
                Gold.SetActive(false);
                Day.SetActive(true);
                break;
            case STAGE.POPULARITY:
                TextNode.text = "This is how much percentage of audience love you.\n(hope everyone will love you)";
                Day.SetActive(false);
                Popularity.SetActive(true);
                break;
            case STAGE.START_GAME:
                TextNode.text = "Alright.It's your time to decide what kind of media you are.\nReal or fake? Exaggerated or truthful? \nWhat will you choose?";
                Popularity.SetActive(false);
                break;
            case STAGE.BOARDING_END:
                this.gameObject.SetActive(false);
                break;

        }
    }

    public void HandleClick()
    {
        CurrentStage++;
        HandleStageStart();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
