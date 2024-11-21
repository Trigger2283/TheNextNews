using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewsUI : MonoBehaviour
{
    public NewsTuning.SINGLE_NEWS newsTuning;
    public GameObject ChooseNewsUI;
    public Text NewsDetail;
    public Text NewsCost;
    public Button ChooseNewsButton;
    public GameObject ChoosePartsUI;
    public Text Title;
    public Button[] ChoosePartsButtons;
    private List<int> UserChoice;
    public GameObject ResultUI;
    public Text ResultText;
    public Image ResultPic;
    public Sprite ChooseNewsPartBg;
    public GameObject GoldIcon;
    NewsTuning.NEWS_BASE_RATING_REWARD rewards;
    public enum STAGE
    {
        CHOOSE_NEWS,
        CHOOSE_TITLE,
        CHOOSE_DESCRIPTION,
        CHOOSE_PICTURE,
        SHOW_RESULT,
    }
    private STAGE currentStage;

    // Start is called before the first frame update
    void Start()
    {
        UserChoice = new List<int>();
        UserChoice.Clear();
        rewards.Gold = 0;
        rewards.Popularity = 0;
    }

    public void InitUI(NewsTuning.SINGLE_NEWS News)
    {
        ChooseNewsUI.SetActive(true);
        ChoosePartsUI.SetActive(false);
        ResultUI.SetActive(false);
        newsTuning = News;
        NewsDetail.text = newsTuning.NewsDetail;
        NewsCost.text = newsTuning.NewsCost == 0 ? "FREE" : "    "+ newsTuning.NewsCost.ToString();
        if(newsTuning.NewsCost != 0)
        {
            GoldIcon.SetActive(true);
        }
        else
        {
            GoldIcon.SetActive(false);
        }
        ChooseNewsButton.interactable = GameManager.Instance.GetTrackingDataPerCurrency(GameManager.CURRENCY.GOLD).CurrentValue >= newsTuning.NewsCost;
        HandleNewStage(STAGE.CHOOSE_NEWS);
        for (int i = 0; i < ChoosePartsButtons.Length; i++)
        {
            ChoosePartsButtons[i].GetComponent<Image>().sprite = ChooseNewsPartBg;
        }
    }

    public void HandleMadeChoice()
    {
        GameManager.Instance.HandlePlayConfirmationAudio();
        GameManager.Instance.HandleAddValue(GameManager.CURRENCY.GOLD, -newsTuning.NewsCost);
        this.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        HandleNewStage(STAGE.CHOOSE_TITLE);
    }

    public void HandleNewStage(STAGE Stage)
    {
        print("New Stage: " + Stage.ToString());
        currentStage = Stage;
        switch (Stage)
        {
            case STAGE.CHOOSE_TITLE:
                ChoosePartsUI.SetActive(true);
                ChooseNewsUI.SetActive(false);
                ResultUI.SetActive(false);
                for (int i = 0; i < ChoosePartsButtons.Length; i++)
                {
                    ChoosePartsButtons[i].GetComponent<Image>().sprite = ChooseNewsPartBg;
                }
                Title.text = "Choose a tile from followings";
                for(int i = 0;i<ChoosePartsButtons.Length;i++)
                {
                    ChoosePartsButtons[i].GetComponentInChildren<Text>().text = newsTuning.Titles[i].TitleText;
                    AddLisnterForButton(i);
                }
                break;
            case STAGE.CHOOSE_DESCRIPTION:
                Title.text = "Choose a description from followings";
                for (int i = 0; i < ChoosePartsButtons.Length; i++)
                {
                    ChoosePartsButtons[i].GetComponentInChildren<Text>().text = newsTuning.Descriptions[i].DescriptionText;
                }
                for (int i = 0; i < ChoosePartsButtons.Length; i++)
                {
                    ChoosePartsButtons[i].GetComponent<Image>().sprite = ChooseNewsPartBg;
                }
                break;
            case STAGE.CHOOSE_PICTURE:
                Title.text = "Choose a picture from followings";
                for (int i = 0; i < ChoosePartsButtons.Length; i++)
                {
                    ChoosePartsButtons[i].GetComponentInChildren<Text>().text = "";
                    ChoosePartsButtons[i].GetComponent<Image>().sprite = newsTuning.Pictures[i].PictureSprite;
                }
                break;
            case STAGE.SHOW_RESULT:
                ChoosePartsUI.SetActive(false);
                ResultUI.SetActive(true);
                HandleShowResult();
                break;
        }
    }

    private void AddLisnterForButton(int index)
    {
        ChoosePartsButtons[index].GetComponent<Button>().onClick.RemoveAllListeners();
        ChoosePartsButtons[index].GetComponent<Button>().onClick.AddListener(() => { HandlePlayerChoosePart(index); });
    }


    public void HandleShowResult()
    {
        ResultText.text = "You Choose Title: \n" + newsTuning.Titles[UserChoice[0]].TitleText + "\nYou Choose Description: \n" + newsTuning.Descriptions[UserChoice[1]].DescriptionText;
        ResultPic.sprite = newsTuning.Pictures[UserChoice[2]].PictureSprite;
    }

    public void HandlePlayerChoosePart(int index)
    {
        GameManager.Instance.HandlePlayConfirmationAudio();
        UserChoice.Add(index);
        HandleNewStage(currentStage + 1);
    }

    public void HandleRetry()
    {
        UserChoice.Clear();
        for (int i = 0; i < ChoosePartsButtons.Length; i++)
        {
            ChoosePartsButtons[i].GetComponent<Image>().sprite = null;
        }
        HandleNewStage(STAGE.CHOOSE_TITLE);
    }

    private void HandleCalculateRewards(NewsTuning.NEWS_BASE_RATING_REWARD rewardsTuning)
    {
        print("Gold is: " + rewardsTuning.Gold + " Popularity is: " + rewardsTuning.Popularity);
        rewards.Gold += rewardsTuning.Gold;
        rewards.Popularity += rewardsTuning.Popularity;
        
    }

    private void HandleSubmitAllRewards()
    {
        print("Final result is \nGold is: " + rewards.Gold + " Popularity is: " + rewards.Popularity);
        GameManager.Instance.HandleAddValue(GameManager.CURRENCY.GOLD, rewards.Gold);
        GameManager.Instance.HandleAddValue(GameManager.CURRENCY.POPULARITY, rewards.Popularity);
    }


    public void HandleConfirm()
    {
        GameManager.Instance.HandlePlayConfirmationAudio();
        //calculate results here
        //first calculate the news base level rewards
        print("Start Calculating base rewards");
        NewsTuning.NEWS_BASE_RATING_REWARD baseRewards = NewsTuning.Instance.GetBaseRewardTuningByLevel(newsTuning.NewsLevel);
        HandleCalculateRewards(baseRewards);
        print("Start Calculating rewards for each parts");
        //then calculate each parts of news.
        for(int i = 0;i<3;i++)
        {
            for(int j = 0;j<3;j++)
            {
                NewsTuning.NEWS_PARTS part = (NewsTuning.NEWS_PARTS)i;
                NewsTuning.RATING_TYPE type = (NewsTuning.RATING_TYPE)j;
                NewsTuning.RATING_LEVEL rewardLevel = NewsTuning.Instance.GetRatingLevelBaseOnTypeAndIndex(newsTuning, part, type, UserChoice[i]);
                NewsTuning.NEWS_BASE_RATING_REWARD temp = NewsTuning.Instance.GetCategoryRewardByCatAndLevel(type, rewardLevel);
                print("Calculating parts: " + part + " type: " + type);
                HandleCalculateRewards(temp);
            }
        }
        
        print("Calculation Done");
        HandleSubmitAllRewards();
        GameManager.Instance.HandleTimeSlotEnd();
        Destroy(this.gameObject);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
