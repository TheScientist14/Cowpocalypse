using System.Collections.Generic;
using _Scripts.Save_System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    void Start()
    {
        InputMaster.instance.InputAction.Disable();
    }

    public void NewGame()
    {
        IEnumerable<ItemData> itemsData = ItemCreator.LoadAllResourceAtPath<ItemData>();
        foreach(ItemData itemData in itemsData)
            itemData.Unlocked = (itemData.Tier.Level <= 1);

        InputMaster.instance.InputAction.Enable();
        SceneManager.LoadScene(1);
    }

    public void Load()
    {
        Assert.IsTrue(SaveSystem.instance.CheckForSave());

        SaveSystem._loadOnStartup = true;
        InputMaster.instance.InputAction.Enable();
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}