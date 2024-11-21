using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundResultUI : MonoBehaviour
{
    public static RoundResultUI Instance;
    public GameObject UIObj;
    public Text TextNode;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void HandleShowResult(int GoldDelta, int PopularityDelta)
    {
        UIObj.SetActive(true);
        TextNode.text = GoldDelta + "\n\n\n\n" + PopularityDelta;
    }

    public void HandleConfirm()
    {
        UIObj.SetActive(false);
        GameManager.Instance.HandlePlayConfirmationAudio();
        GameManager.Instance.HandleTimeSlotEnd();
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
