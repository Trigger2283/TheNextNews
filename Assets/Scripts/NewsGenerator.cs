using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewsGenerator : MonoBehaviour
{
    public static NewsGenerator Instance;
    public GameObject NewsPrefab;

    public class NEWS_POOL
    {
        public NEWS_POOL() 
        {
            NewsPoolFree = new List<NewsTuning.SINGLE_NEWS>();
            NewsPoolCost = new List<NewsTuning.SINGLE_NEWS>();
        }
        public void Clear()
        {
            NewsPoolFree.Clear();
            NewsPoolCost.Clear();
        }
        public List<NewsTuning.SINGLE_NEWS> NewsPoolFree;
        public List<NewsTuning.SINGLE_NEWS> NewsPoolCost;
    }

    public NEWS_POOL CurrentRoundPool;
    public NEWS_POOL GameLongPool;
    public List<GameObject> GeneratedUIs;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void InitGameLongPool()
    {
        for(int i = 0;i<NewsTuning.Instance.NewsTuningTable.Length;i++)
        {
            if (NewsTuning.Instance.NewsTuningTable[i].NewsCost != 0) GameLongPool.NewsPoolCost.Add(NewsTuning.Instance.NewsTuningTable[i]);
            else GameLongPool.NewsPoolFree.Add(NewsTuning.Instance.NewsTuningTable[i]);
        }
    }

    private void ClearCurrentRoundNewsChooseUI()
    {
        foreach (GameObject obj in GeneratedUIs)
        {
            Destroy(obj);
        }
        GeneratedUIs.Clear();
    }

    private void InitCurrentRoundPool()
    {
        CurrentRoundPool.Clear();
        ClearCurrentRoundNewsChooseUI();
        for (int i = 0;i<GameLongPool.NewsPoolFree.Count;i++)
        {
            CurrentRoundPool.NewsPoolFree.Add(GameLongPool.NewsPoolFree[i]);
        }

        for (int i = 0; i < GameLongPool.NewsPoolCost.Count; i++)
        {
            CurrentRoundPool.NewsPoolCost.Add(GameLongPool.NewsPoolCost[i]);
        }
    }

    private bool RollDice(float possibility)
    {
        return Random.Range(0f, 1f) <= possibility;
    }


    public void HandleGenerateNews(int N)
    {
        InitCurrentRoundPool();
        int index = 0;
        //handle choose logic here.
        //for freeeeeeeeeeeeee news
        for(int i = 0;i<N-1;i++)
        {
            bool findProper = false;
            //from level A - C, by default D
            for(int j = 3;j>0;j--)
            {
                NewsTuning.NEWS_LEVEL level = (NewsTuning.NEWS_LEVEL)j;
                //formula: Popularity/100 * NewsLevelTuningPossiblity
                //example: current popularity 50; for a A level news: 50/100*0.33 = 16.5%
                float possibility = ((float)GameManager.Instance.GetTrackingDataPerCurrency(GameManager.CURRENCY.POPULARITY).CurrentValue) / 100f * NewsTuning.Instance.PossibilityTuning.Possibility[j];
                bool success = RollDice(possibility);
                if (success)
                {
                    NewsTuning.SINGLE_NEWS ARandomNews;
                    bool tryFind = NewsTuning.Instance.GetNewsByLevel(CurrentRoundPool.NewsPoolFree, level, out ARandomNews);
                    if (tryFind)
                    {
                        findProper = true;
                        //yay! we finally, get a news to generate.
                        HandleDecidedNews(ARandomNews, index, true);
                        index++;
                        print("News Generation is Sucess! Level is: " + level.ToString() + "possibility is: " + possibility);
                        break;
                    }
                }
            }
            //if we could not find a proper one from level A-C, We choose a D
            if(!findProper)
            {
                NewsTuning.SINGLE_NEWS ARandomNews;
                bool tryFind = NewsTuning.Instance.GetNewsByLevel(CurrentRoundPool.NewsPoolFree, NewsTuning.NEWS_LEVEL.D, out ARandomNews);
                if (tryFind)
                {
                    findProper = true;
                    //yay! we finally, get a news to generate.
                    HandleDecidedNews(ARandomNews, index, true);
                    print("News Generation by default! Level is: D");
                    index++;
                }
            }
            //it could be, somehow, could not find any of the news.. this is by expectation.
        }

        //now it's time to select a news with cost...
        //the logic here should be easier.. random one from the pool.
        if(CurrentRoundPool.NewsPoolCost.Count>0)
        {
            int randIndex = Random.Range(0, CurrentRoundPool.NewsPoolCost.Count);
            NewsTuning.SINGLE_NEWS paidOne = CurrentRoundPool.NewsPoolCost[randIndex];
            HandleDecidedNews(paidOne, index, false);
        }
        else
        {
            print("News Generation failed for cost news..");
        }
    }


    public void OnClickNewButton(int index)
    {
        //remove it from the large pool
        NewsTuning.SINGLE_NEWS news = GeneratedUIs[index].GetComponent<NewsUI>().newsTuning;
        if(news.NewsCost!=0)
        {
            GameLongPool.NewsPoolCost.Remove(news);
        }
        else
        {
            GameLongPool.NewsPoolFree.Remove(news);
        }
        GeneratedUIs[index].GetComponent<NewsUI>().HandleMadeChoice();
        GeneratedUIs.RemoveAt(index);
        ClearCurrentRoundNewsChooseUI();
    }


    public void HandleDecidedNews(NewsTuning.SINGLE_NEWS news, int index, bool isFreeNews)
    {
        //firstly, we should remove this news from current round tuning table.
        if(isFreeNews)
        {
            CurrentRoundPool.NewsPoolFree.Remove(news);
        }
        else
        {
            CurrentRoundPool.NewsPoolCost.Remove(news);
        }
        GameObject newNews = Instantiate(NewsPrefab,this.transform);
        newNews.GetComponent<RectTransform>().anchoredPosition = new Vector2(-Screen.width / 2 + 300f + index * 450, 0);
        newNews.GetComponent<NewsUI>().InitUI(news);
        newNews.GetComponentInChildren<Button>().onClick.AddListener(() => { OnClickNewButton(index); });
        GeneratedUIs.Add(newNews);
    }



    // Start is called before the first frame update
    void Start()
    {
        CurrentRoundPool = new NEWS_POOL();
        GameLongPool = new NEWS_POOL();
        GeneratedUIs = new List<GameObject>();
        InitGameLongPool();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
