using _Scripts.Pooling_System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Belt : MonoBehaviour
{
    public static int BeltID = 0;

    public Belt BeltInSequence;
    public Item BeltItem;
    public bool isSpaceTaken;
    public bool isMachineBlocking;

    public Machine MachineInSequence;

    [SerializeField]
    private ScriptablesRelativeAudio _scriptablesRelativeAudio;
    private AudioSource _audioSource;
    private float _volume;

    protected bool m_IsRunning;

    public UnityEvent OnItemMoved;

    private void Start()
    {
        isMachineBlocking = false;
        m_IsRunning = false;
        BeltInSequence = GetNextBelt();
        gameObject.name = $"Belt: {BeltID++}";

        _audioSource = GetComponent<AudioSource>();
        CallSound(EnumRelativeSounds.Activate);
        if(OnItemMoved == null)
            OnItemMoved = new UnityEvent();
    }

    protected void Update()
    {
        if(BeltInSequence == null)
            BeltInSequence = GetNextBelt();

        if(BeltInSequence == null)
            return;
        
        if(BeltItem != null && BeltItem.GetItem() != null && !m_IsRunning)
        {
            m_IsRunning = true;
            StartCoroutine(StartBeltMove());
        }
    }

    public virtual IEnumerator StartBeltMove()
    {
        isSpaceTaken = true;

        if(BeltItem.GetItem() != null && BeltInSequence != null && BeltInSequence.isSpaceTaken == false)
        {
            if(MachineInSequence)
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
            if (BeltInSequence.GetComponent<Spawner>())
                isMachineBlocking = true;
            if(!isMachineBlocking)
            {
                if(MachineInSequence != null)
                {
                    MachineInSequence.AddToStock(BeltItem);
                    isSpaceTaken = false;
                    BeltItem = null;
                }
                else
                {
                    Vector3 toPosition = BeltInSequence.transform.position;
                    BeltInSequence.isSpaceTaken = true;

                    while(BeltItem.GetItem().transform.position != toPosition)
                    {
                        BeltItem.GetItem().transform.position = Vector3.MoveTowards(BeltItem.transform.position, toPosition, BeltManager.instance.speed * Time.fixedDeltaTime);
                        yield return new WaitForFixedUpdate();
                    }
                    OnItemMoved.Invoke();
                    if(BeltInSequence != null)
                    {
                        BeltInSequence.BeltItem = BeltItem;
                        BeltItem = null;
                    }
                    else
                        PoolManager.instance.DespawnObject(BeltItem);
                    isSpaceTaken = false;

                }
            }
        }
        m_IsRunning = false;
    }

    public Belt GetNextBelt()
    {
        isMachineBlocking = false;
        Transform currentBeltTransform = transform;
        RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.up, currentBeltTransform.up, 0.1f);

        if(hit.collider != null)
        {
            Belt belt = hit.collider.GetComponent<Belt>();
            if(belt != null)
            {
                if(belt.GetComponent<Machine>())
                    MachineInSequence = belt.GetComponent<Machine>();
                return belt;
            }
        }

        return null;
    }

    protected void OnDestroy()
    {
        if(BeltItem != null)
            PoolManager.instance.DespawnObject(BeltItem);
    }

    private void CallSound(EnumRelativeSounds _action)
    {
        _volume = _scriptablesRelativeAudio.volume;

        switch(_action)
        {
            case EnumRelativeSounds.Spawn:
                PlayRelativeSound(_scriptablesRelativeAudio._spawnAudio, false);
                // Debug.Log("Spawn audio");
                break;
            case EnumRelativeSounds.Activate:
                PlayRelativeSound(_scriptablesRelativeAudio._activateAudio, true);
                // Debug.Log("Activate audio");
                break;
            case EnumRelativeSounds.Problem:
                PlayRelativeSound(_scriptablesRelativeAudio._problemAudio, false);
                // Debug.Log("Problem audio");
                break;
            default:
                break;
        }
    }

    private void PlayRelativeSound(AudioClip _audioClip, bool loop)
    {
        _audioSource.loop = loop;
        if(loop)
        {
            _audioSource.volume = _volume / 2;
        }
        else
        {
            _audioSource.volume = _volume;
        }
        _audioSource.clip = _audioClip;
        _audioSource.Play();

        // Debug.Log(_audioClip);
    }

}
