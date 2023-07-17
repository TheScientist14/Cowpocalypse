using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TutorialSystem : Singleton<TutorialSystem>
{
    [SerializeField] private bool loadOnStartup = false;

    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private GameObject[] arrows = new GameObject[]{};
    [SerializeField] private String[] texts = new String[]{};
    [SerializeField] private GameObject darkenBG;
    [SerializeField] private PanelComponent botPannel;
    [SerializeField] private PanelComponent sidePannel;

    Dictionary<String, GameObject> _arrowDictionary;


    private void Awake()
    {
        base.Awake();

        foreach (GameObject variableArrow in arrows)
        {
            variableArrow.SetActive(false);
        }
        darkenBG.SetActive(false);

        SetupArrowDictionary();
        
        UnityEvent openMachine = new UnityEvent();
    }

   
    private void SetupArrowDictionary()
    {
        _arrowDictionary = arrows.ToDictionary(x => x.name);
    }

    private string addReturnInString(string prmString, int prmNbOfReturns = 2)
    {
        string temp = "";

        for (int i = 0; i < prmNbOfReturns; i++)
        {
            temp += Environment.NewLine;
        }

        return prmString.Replace(".", "." + temp);

    }

    // Start is called before the first frame update
    void Start()
    {
        if (loadOnStartup)
        {
            darkenBG.SetActive(true);
            InputStateMachine.instance.SetState(new PauseState());

            tutorialText.SetText(addReturnInString(texts[0]));

            _arrowDictionary["SpawnerArrow"].SetActive(true);
            sidePannel.Open();
        }
    }
}
