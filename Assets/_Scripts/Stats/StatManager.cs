using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StatManager : Singleton<StatManager>
{
	[SerializeField] StatData m_ExtractSpeedStatData;
	[SerializeField] StatData m_BeltSpeedStatData;
	[SerializeField] StatData m_CraftSpeedStatData;

	private Stat m_ExtractSpeedStat = new Stat();
	private Stat m_BeltSpeedStat = new Stat();
	private Stat m_CraftSpeedStat = new Stat();

	private List<Stat> m_Stats = new List<Stat>();

	public UnityEvent OnStatUpdated;

	public static int ExtractSpeedIndex = 0;
	public static int BeltSpeedIndex = 1;
	public static int CraftSpeedIndex = 2;

	protected override void Awake()
	{
		base.Awake();

		if(OnStatUpdated == null)
			OnStatUpdated = new UnityEvent();

		m_ExtractSpeedStat.StatData = m_ExtractSpeedStatData;
		m_BeltSpeedStat.StatData = m_BeltSpeedStatData;
		m_CraftSpeedStat.StatData = m_CraftSpeedStatData;

		m_Stats.Add(m_ExtractSpeedStat);
		m_Stats.Add(m_BeltSpeedStat);
		m_Stats.Add(m_CraftSpeedStat);

		Shader.SetGlobalFloat("_speed", GetStatValue(BeltSpeedIndex));
	}

	[ContextMenu("Log stat values")]
	[Button("Log Values")]
	public void LogValues()
	{
		foreach(var stat in m_Stats)
			Debug.Log($"{stat.StatData.Name} : {stat.CurrentLevel}, {stat.Value}");
	}

	public int GetStatLevel(int iStatIdx)
	{
		if(iStatIdx >= m_Stats.Count)
		{
			Debug.LogError("Invalid stat index : " + iStatIdx);
			return 0;
		}

		return m_Stats[iStatIdx].CurrentLevel;
	}

	public void AddLevelToStat(int iStatIdx)
	{
		if(iStatIdx >= m_Stats.Count)
		{
			Debug.LogError("Invalid stat index : " + iStatIdx);
			return;
		}

		if(Wallet.instance.Money < m_Stats[iStatIdx].StatData.Prices[m_Stats[iStatIdx].CurrentLevel - 1])
			return;

		Wallet.instance.Money -= m_Stats[iStatIdx].StatData.Prices[m_Stats[iStatIdx].CurrentLevel - 1];
		m_Stats[iStatIdx].CurrentLevel++;
		Shader.SetGlobalFloat("_speed", GetStatValue(BeltSpeedIndex));
		OnStatUpdated.Invoke();
	}

	public void SetLevelToStat(int iStatIdx, int iLevel)
	{
		if(iStatIdx >= m_Stats.Count)
		{
			Debug.LogError("Invalid stat index : " + iStatIdx);
			return;
		}

		m_Stats[iStatIdx].CurrentLevel = iLevel;
		Shader.SetGlobalFloat("_speed", GetStatValue(BeltSpeedIndex));
		OnStatUpdated.Invoke();
	}

	public Stat GetStat(int iStatIdx)
	{
		if(iStatIdx >= m_Stats.Count)
		{
			Debug.LogError("Invalid stat index : " + iStatIdx);
			return null;
		}

		return m_Stats[iStatIdx];
	}

	public float GetStatValue(int iStatIdx)
	{
		if(iStatIdx >= m_Stats.Count)
		{
			Debug.LogError("Invalid stat index : " + iStatIdx);
			return 0;
		}

		return m_Stats[iStatIdx].Value;
	}

}