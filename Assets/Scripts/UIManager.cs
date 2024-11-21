using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Text Day;
    public Text Gold;
    public Text Popularity;
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Day.text = (GameManager.Instance.TrackingData.CurrentDay+1).ToString();
        Gold.text =  GameManager.Instance.GetTrackingDataPerCurrency(GameManager.CURRENCY.GOLD).CurrentValue.ToString();
        Popularity.text = GameManager.Instance.GetTrackingDataPerCurrency(GameManager.CURRENCY.POPULARITY).CurrentValue.ToString();
    }
}
