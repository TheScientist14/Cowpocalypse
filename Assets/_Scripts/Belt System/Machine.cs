using _Scripts.Pooling_System;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Machine : Belt
{
    [Expandable]
    [SerializeField] private ItemData CraftedItem;

    [SerializeField]
    private ScriptablesRelativeAudio _thescriptablesRelativeAudio;
    private AudioSource _theaudioSource;
    private float _thevolume;

    public Dictionary<ItemData, int> Stock { get; set; }
    public StockUpdateEvent stockUpdated { get; private set; } = new StockUpdateEvent();
    public class StockUpdateEvent : UnityEvent<Dictionary<ItemData, int>> { }

    private void Start()
    {
        Stock = new Dictionary<ItemData, int>();
        stockUpdated.Invoke(Stock);
        gameObject.name = $"Machine: {BeltID++}";

        _theaudioSource = GetComponent<AudioSource>();
        CallSound(EnumRelativeSounds.Spawn);

        SetCrafteditem(CraftedItem);
    }

    public IEnumerator MoveQueuedItems(Item queuedItem)
    {
        Debug.Log("StartMachineMove");

        Vector3 toPosition = transform.position;

        while(queuedItem.GetItem().transform.position != toPosition)
        {
            queuedItem.GetItem().transform.position = Vector3.MoveTowards(queuedItem.transform.position, toPosition, BeltManager.instance.speed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        PoolManager.instance.DespawnObject(queuedItem);

        bool NextBeltBlock = true;

        while(NextBeltBlock == true)
        {
            foreach (ItemData item in CraftedItem.Recipes.Keys)
            {
                if (Stock[item] < CraftedItem.Recipes[item] || BeltInSequence == null || BeltInSequence.isSpaceTaken == true)
                    yield return null;
                else
                    NextBeltBlock = false;
            }
        }


        foreach(ItemData item in CraftedItem.Recipes.Keys)
            Stock[item] -= CraftedItem.Recipes[item];
        stockUpdated.Invoke(Stock);
        yield return new WaitForSeconds(CraftedItem.CraftDuration * BeltManager.instance.GetCraftingSpeedMultiplier());
        // play sound
        CallSound(EnumRelativeSounds.Activate);
        Item craftedItem = PoolManager.instance.SpawnObject(CraftedItem, transform.position);
        StartCoroutine(Output(craftedItem));
    }

    private IEnumerator Output(Item item)
    {
        bool outputed = false;
        while(outputed == false)
        {
            print("Trying Output");
            if (BeltInSequence != null && BeltInSequence.isSpaceTaken == false)
            {
                if (BeltInSequence.GetComponent<Machine>())
                {
                    isMachineBlocking = true;
                    if (MachineInSequence.GetCraftedItem() != null)
                    {
                        if (MachineInSequence.GetCraftedItem().Recipes.ContainsKey(BeltItem.GetItemData()))
                        {
                            MachineInSequence.Stock.TryGetValue(BeltItem.GetItemData(), out int amount);
                            MachineInSequence.GetCraftedItem().Recipes.TryGetValue(BeltItem.GetItemData(), out int maxAmount);
                            if (amount <= maxAmount)
                                isMachineBlocking = false;
                        }
                    }
                }
                if (!isMachineBlocking)
                {
                    Vector3 toPosition = BeltInSequence.transform.position;
                    BeltInSequence.isSpaceTaken = true;
                    isSpaceTaken = false;
                    BeltInSequence.BeltItem = item;
                    outputed = true;
                }
            }
            yield return null;
        }
    }

    public void SetCrafteditem(ItemData craftedItemData)
    {
        CraftedItem = craftedItemData;
        Stock.Clear();
        stockUpdated.Invoke(Stock);
        if(CraftedItem != null)
            foreach(ItemData item in CraftedItem.Recipes.Keys)
                Stock.Add(item, 0);
    }

    public ItemData GetCraftedItem()
    {
        return CraftedItem;
    }

    public void AddToStock(Item item)
    {
        Stock.TryGetValue(item.GetItemData(), out int amount);
        Stock[item.GetItemData()] = Stock[item.GetItemData()] + 1;
        print("AddToStock : " + Stock[item.GetItemData()] + " of type : " + item.GetItemData().name);
        stockUpdated.Invoke(Stock);
        StartCoroutine(MoveQueuedItems(item));
    }

    public void CallSound(EnumRelativeSounds _action)
    {
        _thevolume = _thescriptablesRelativeAudio.volume;

        switch(_action)
        {
            case EnumRelativeSounds.Spawn:
                PlayRelativeSound(_thescriptablesRelativeAudio._spawnAudio, false);
                // Debug.Log("Spawn audio");
                break;
            case EnumRelativeSounds.Activate:
                PlayRelativeSound(_thescriptablesRelativeAudio._activateAudio, false);
                // Debug.Log("Activate audio");
                break;
            case EnumRelativeSounds.Problem:
                PlayRelativeSound(_thescriptablesRelativeAudio._problemAudio, false);
                // Debug.Log("Problem audio");
                break;
            default:
                break;
        }
    }

    protected void PlayRelativeSound(AudioClip _audioClip, bool loop)
    {
        _theaudioSource.loop = loop;
        if(loop)
        {
            _theaudioSource.volume = _thevolume / 2;
        }
        else
        {
            _theaudioSource.volume = _thevolume;
        }
        _theaudioSource.clip = _audioClip;
        _theaudioSource.Play();

        // Debug.Log(_audioClip);
    }

}
