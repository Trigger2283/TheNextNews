using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewsTuning : MonoBehaviour
{
    public static NewsTuning Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public enum NEWS_LEVEL
    {
        D,
        C,
        B,
        A,
    }

    public enum RATING_LEVEL
    {
        LEVEL_1,
        LEVEL_2,
        LEVEL_3,
    }

    public enum RATING_TYPE
    {
        PANIC,
        EXAGGERATE,
        ATTRACTIVE,
    }

    [Serializable]
    public struct NEWS_RATING_LEVEL
    {
        public RATING_LEVEL PanicLevel;
        public RATING_LEVEL ExaggerateLevel;
        public RATING_LEVEL AttractiveLevel;
    }

    [Serializable]
    public struct TITLE
    {
        public string TitleText;
        public NEWS_RATING_LEVEL TitleRating;
    }
    [Serializable]
    public struct DESCRIPTION
    {
        public string DescriptionText;
        public NEWS_RATING_LEVEL DescriptionRating;
    }
    [Serializable]
    public struct PICTURE
    {
        public Sprite PictureSprite;
        public NEWS_RATING_LEVEL PictureRating;
    }

    public enum NEWS_PARTS
    {
        TITLE,
        DESCRIPTION,
        PICTURE,
    }



    [Serializable]
    public struct SINGLE_NEWS
    {
        public string NewsDetail;
        public NEWS_LEVEL NewsLevel;
        public int NewsCost;
        public TITLE[] Titles;
        public DESCRIPTION[] Descriptions;
        public PICTURE[] Pictures;
    }

    [Serializable]
    public struct NEWS_LEVEL_POSSIBILITY
    {
        public float[] Possibility; 
    }


    [Serializable]
    public struct NEWS_BASE_RATING_REWARD
    {
        public int Gold;
        public int Popularity;
    }
    [Serializable]
    public struct NEWS_CATEGORY_REWARDS
    {
        public RATING_TYPE RatingCat;
        public NEWS_BASE_RATING_REWARD[] RewardsValue;//from Level 1 - 3
    }


    [Serializable]
    public struct NEWS_REWARD_TUNING
    {
        public NEWS_BASE_RATING_REWARD[] BaseRatingTuning;//Basic reward tuning from D->A
        public NEWS_CATEGORY_REWARDS[] CategoryRatingTuning;//
    }



    public SINGLE_NEWS[] NewsTuningTable;
    public NEWS_LEVEL_POSSIBILITY PossibilityTuning;
    public NEWS_REWARD_TUNING NewsRewardsTuning;

    public bool GetNewsByLevel(List<SINGLE_NEWS>NewsPool, NEWS_LEVEL Level, out SINGLE_NEWS Result)
    {
        List<SINGLE_NEWS> ResultPool = new List<SINGLE_NEWS>();
        for(int i = 0;i<NewsPool.Count;i++)
        {
            if(NewsPool[i].NewsLevel == Level)
            {
                ResultPool.Add(NewsPool[i]);
            }
        }
        if (ResultPool.Count != 0)
        {
            int rand = UnityEngine.Random.Range(0, ResultPool.Count);
            Result = ResultPool[rand];
            return true;
        }
        else
        {
            print("Could not find a specific news with Level: " + Level.ToString());
            Result = NewsTuningTable[0];
            return false;
        }
    }


    public NEWS_BASE_RATING_REWARD GetBaseRewardTuningByLevel(NEWS_LEVEL newsLevel)
    {
        return NewsRewardsTuning.BaseRatingTuning[(int)newsLevel];
    }

    public NEWS_BASE_RATING_REWARD GetCategoryRewardByCatAndLevel(RATING_TYPE type, RATING_LEVEL level)
    {
        return NewsRewardsTuning.CategoryRatingTuning[(int)type].RewardsValue[(int)level];
    }

    public RATING_LEVEL GetRatingLevelBaseOnTypeAndIndex(SINGLE_NEWS tuning, NEWS_PARTS Parts, RATING_TYPE type, int index)
    {
        switch(Parts)
        {
            case NEWS_PARTS.TITLE:
                return GetRatingLevelFromRatingLevelAndIndex(tuning.Titles[index].TitleRating, type);
            case NEWS_PARTS.DESCRIPTION:
                return GetRatingLevelFromRatingLevelAndIndex(tuning.Descriptions[index].DescriptionRating, type);
            case NEWS_PARTS.PICTURE:
                return GetRatingLevelFromRatingLevelAndIndex(tuning.Pictures[index].PictureRating, type);
        }
        return RATING_LEVEL.LEVEL_1;
    }

    public RATING_LEVEL GetRatingLevelFromRatingLevelAndIndex(NEWS_RATING_LEVEL ratingTuning, RATING_TYPE type)
    {
       switch(type)
        {
            case RATING_TYPE.PANIC:
                return ratingTuning.PanicLevel;
            case RATING_TYPE.EXAGGERATE:
                return ratingTuning.ExaggerateLevel;
            case RATING_TYPE.ATTRACTIVE:
                return ratingTuning.AttractiveLevel;
            default:
                return ratingTuning.PanicLevel;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
