using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int GameLength = 5;
    public AudioSource AS;
    public enum CURRENCY
    {
        GOLD,
        POPULARITY,
    }
    [Serializable]
    public struct TUNING_DATA
    {
        public CURRENCY Type;
        public int InitialValue;
        public int LowerCap;
        public int HigherCap;
    }
    public TUNING_DATA[] TuningData;

    public struct TRACKING_DATA_PER_CURRENCY
    {
        public CURRENCY Type;
        public int CurrentValue;
        public int ChangedValueThisRound;
    }

    public struct TRACKING_DATA
    {
        public TRACKING_DATA_PER_CURRENCY[] PerCurrency;
        public int CurrentDay;
    }

    public enum TIME_SLOT
    {
        START_OF_DAY,
        CHOOSE_NEWS,
        END_OF_DAY,
    }

    public TIME_SLOT CurrentTimeSlot;


    public TRACKING_DATA TrackingData;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public TUNING_DATA GetTuningData(CURRENCY currency)
    {
        for(int i = 0;i<TuningData.Length;i++)
        {
            if (TuningData[i].Type == currency) return TuningData[i];
        }
        Debug.LogError("This should not happen, could not find: " + currency.ToString());
        return TuningData[0];
    }


    // Start is called before the first frame update
    void Start()
    {
        TrackingData = new TRACKING_DATA();
        TrackingData.PerCurrency = new TRACKING_DATA_PER_CURRENCY[Enum.GetValues(typeof(CURRENCY)).Length];
        for (int i = 0;i<Enum.GetValues(typeof(CURRENCY)).Length;i++)
        {
            TrackingData.PerCurrency[i].Type = (CURRENCY)i;
            TUNING_DATA tuning = GetTuningData((CURRENCY)i);
            TrackingData.PerCurrency[i].CurrentValue = tuning.InitialValue;
            TrackingData.PerCurrency[i].ChangedValueThisRound = 0;
        }
        TrackingData.CurrentDay = 0;
        HandleTimeSlotStart(TIME_SLOT.START_OF_DAY);
        AS = this.GetComponent<AudioSource>();
    }

    public void HandlePlayConfirmationAudio()
    {
        AS.Stop();
        AS.Play();
    }

    public TRACKING_DATA_PER_CURRENCY GetTrackingDataPerCurrency(CURRENCY type)
    {
        for(int i = 0;i<TrackingData.PerCurrency.Length;i++)
        {
            if(TrackingData.PerCurrency[i].Type == type)
            {
                return TrackingData.PerCurrency[i];
            }
        }
        Debug.LogError("This should not happen, could not find: " + type.ToString());
        return TrackingData.PerCurrency[0];
    }

    public void CheckResult()
    {
        if(TrackingData.PerCurrency[0].CurrentValue == 0)
        {
            EndingManager.Instance.HandleGameEnding(EndingManager.ENDING_TYPE.GOLD_ZERO);
        }
        else if(TrackingData.PerCurrency[1].CurrentValue == 0)
        {
            EndingManager.Instance.HandleGameEnding(EndingManager.ENDING_TYPE.POPULARITY_ZERO);
        }
        else if(TrackingData.CurrentDay>= GameLength)
        {
            //for now setup the boundry to 300 for gold, 50 for popularity
            //game actually end.
            bool lowGold = TrackingData.PerCurrency[0].CurrentValue < 300;
            bool lowPopularity = TrackingData.PerCurrency[1].CurrentValue < 50;
            if (lowGold && lowPopularity) EndingManager.Instance.HandleGameEnding(EndingManager.ENDING_TYPE.LOW_POP_LOW_GOLD);
            else if (lowGold && !lowPopularity) EndingManager.Instance.HandleGameEnding(EndingManager.ENDING_TYPE.HIGH_POP_LOW_GOLD);
            else if (!lowGold && lowPopularity) EndingManager.Instance.HandleGameEnding(EndingManager.ENDING_TYPE.LOW_POP_HIGH_GOLD);
            else EndingManager.Instance.HandleGameEnding(EndingManager.ENDING_TYPE.HIGH_POP_HIGH_GOLD);
        }
        
    }

    public void HandleAddValue(CURRENCY type, int value)
    {
        for (int i = 0; i < TrackingData.PerCurrency.Length; i++)
        {
            if (TrackingData.PerCurrency[i].Type == type)
            {
                int currentValue = TrackingData.PerCurrency[i].CurrentValue;
                TrackingData.PerCurrency[i].CurrentValue += value;
                TUNING_DATA tuning = GetTuningData((CURRENCY)i);
                TrackingData.PerCurrency[i].CurrentValue = Mathf.Clamp(TrackingData.PerCurrency[i].CurrentValue, tuning.LowerCap, tuning.HigherCap);
                TrackingData.PerCurrency[i].ChangedValueThisRound += TrackingData.PerCurrency[i].CurrentValue - currentValue;
                break;
            }
        }
    }

    public void HandleTimeSlotStart(TIME_SLOT time)
    {
        CurrentTimeSlot = time;
        switch(time)
        {
            case TIME_SLOT.START_OF_DAY:
                CheckResult();
                for (int i = 0; i < TrackingData.PerCurrency.Length; i++)
                {
                    TrackingData.PerCurrency[i].ChangedValueThisRound = 0;
                }
                HandleTimeSlotEnd();
                break;
            case TIME_SLOT.CHOOSE_NEWS:
                NewsGenerator.Instance.HandleGenerateNews(4);
                break;
            case TIME_SLOT.END_OF_DAY:
                RoundResultUI.Instance.HandleShowResult(GetTrackingDataPerCurrency(CURRENCY.GOLD).ChangedValueThisRound, GetTrackingDataPerCurrency(CURRENCY.POPULARITY).ChangedValueThisRound);
                break;
        }
    }


    public void HandleTimeSlotEnd()
    {
        TIME_SLOT nextSlot = CurrentTimeSlot == TIME_SLOT.END_OF_DAY ? TIME_SLOT.START_OF_DAY : CurrentTimeSlot + 1;
        switch(CurrentTimeSlot)
        {
            case TIME_SLOT.END_OF_DAY:
                TrackingData.CurrentDay++;
                break;
        }
        HandleTimeSlotStart(nextSlot);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
