using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class MapGenerator : Singleton<MapGenerator>
{
	private int m_Seed;

	[SerializeField] Grid m_WorldGridGeometry;

	[Header("MapTexture")]
	[SerializeField] int m_MapWidth = 200;
	[SerializeField] int m_MapHeight = 200;
	[SerializeField] int m_PixelPerCell = 16;

	[Serializable]
	private struct RandomSprite
	{
		public Sprite Sprite;
		[Range(0, 1)]
		public double Probability;
	}

	[Serializable]
	private struct RandomTile
	{
		public TileBase Tile;
		[Range(0, 1)]
		public double Probability;
	}

	[Serializable]
	private struct TerrainSprites
	{
		public TerrainType TerrainType;
		public List<RandomSprite> BaseSprites;
		public List<RandomSprite> RandomMotives;
	}

	[SerializeField] List<TerrainSprites> m_TerrainSprites = new List<TerrainSprites>();
	private Dictionary<TerrainType, List<RandomTile>> m_TerrainGeneratedSprites = new Dictionary<TerrainType, List<RandomTile>>();

	[SerializeField] Tilemap m_TilemapPrefab;
	private Tilemap m_MapTilemap = null;

	[Header("WorldGeneration")]
	[SerializeField] float m_TerrainScale = 10;
	[SerializeField] float m_RessourcesScale = 1;

	[SerializeField] float m_WaterLevel = 0.3f;
	[SerializeField] float m_RockLevel = 0.8f;
	[SerializeField] float m_SandTemperature = 0.6f;
	[SerializeField] float m_CoalFrequency = 0.05f;
	[SerializeField] float m_CopperFrequency = 0.05f;
	[SerializeField] float m_IronFrequency = 0.05f;
	[SerializeField] float m_SulfurFrequency = 0.03f;
	[SerializeField] float m_OilFrequency = 0.02f;

	protected void Start()
	{
		InitTerrainSprites();
		GenerateRandomMap();
	}

	private static bool AreRandomSpritesValid(List<RandomSprite> iRandomSprites)
	{
		if(iRandomSprites == null || iRandomSprites.Count == 0)
		{
			Debug.LogError("Empty list.");
			return false;
		}

		int nbInvalidSprites = 0;
		double probabilitiesSum = 0;
		foreach(RandomSprite randomSprite in iRandomSprites)
		{
			bool isRandomSpriteValid = true;

			probabilitiesSum += randomSprite.Probability;
			if(!(0 < randomSprite.Probability && randomSprite.Probability <= 1))
			{
				Debug.LogError("Probability out of validity range: " + randomSprite.Probability + " is not in ]0, 1].");
				isRandomSpriteValid = false;
			}

			if(randomSprite.Sprite == null || randomSprite.Sprite.texture == null)
			{
				Debug.LogError("Null sprite.");
				isRandomSpriteValid = false;
			}
			else if(!randomSprite.Sprite.texture.isReadable)
			{
				Debug.LogError(randomSprite.Sprite.texture.name + " is not readable. Please check \"Read/Write\" in texture import settings.");
				isRandomSpriteValid = false;
			}

			if(!isRandomSpriteValid)
				nbInvalidSprites++;
		}
		bool areSpritesValid = (nbInvalidSprites == 0);
		if(!areSpritesValid)
			Debug.LogError(nbInvalidSprites + " invalid sprite(s).");

		bool areProbabilitiesValid = (probabilitiesSum == 1);
		if(!areProbabilitiesValid)
			Debug.LogError("The sum of the probabilitie(s) (" + probabilitiesSum + ") is not equal to 1.");

		return areProbabilitiesValid && areSpritesValid;
	}

	private bool AreTerrainSpritesValid()
	{
		if(m_TerrainSprites == null)
		{
			Debug.LogError("No terrain sprites defined.");
			return false;
		}

		HashSet<TerrainType> terrainWithSprites = new HashSet<TerrainType>();
		int nbInvalidTerrainSprites = 0;
		foreach(TerrainSprites terrainSprites in m_TerrainSprites)
		{
			bool isTerrainSpritesValid = true;

			if(terrainWithSprites.Contains(terrainSprites.TerrainType))
			{
				Debug.LogError(terrainSprites.TerrainType + " is already present.");
				isTerrainSpritesValid = false;
			}
			terrainWithSprites.Add(terrainSprites.TerrainType);

			if(!AreRandomSpritesValid(terrainSprites.BaseSprites))
			{
				Debug.LogError("BaseSprites are not valid for " + terrainSprites.TerrainType + ".");
				isTerrainSpritesValid = false;
			}
			if(!AreRandomSpritesValid(terrainSprites.RandomMotives))
			{
				Debug.LogError("Motives are not valid for " + terrainSprites.TerrainType + ".");
				isTerrainSpritesValid = false;
			}

			if(!isTerrainSpritesValid)
				nbInvalidTerrainSprites++;
		}

		foreach(TerrainType terrainType in Enum.GetValues(typeof(TerrainType)))
		{
			if(!terrainWithSprites.Contains(terrainType))
			{
				Debug.LogError("Missing sprites for " + terrainType.ToString() + ".");
				nbInvalidTerrainSprites++;
			}
		}

		if(nbInvalidTerrainSprites != 0)
		{
			Debug.LogError(nbInvalidTerrainSprites + " invalid TerrainSprites.");
			return false;
		}

		return true;
	}

	private static Texture2D ResizeTexture(Texture2D iTexture, int iWidth, int iHeight)
	{
		if(iTexture.width == iWidth && iTexture.height == iWidth)
			return iTexture;

		RenderTexture renderTexture = new RenderTexture(iWidth, iHeight, 24);
		RenderTexture.active = renderTexture;
		Graphics.Blit(iTexture, renderTexture);

		Texture2D resizedText = Instantiate(iTexture);
		resizedText.Reinitialize(iWidth, iHeight);
		resizedText.ReadPixels(new Rect(0, 0, iWidth, iHeight), 0, 0);
		resizedText.Apply();
		return resizedText;
	}

	private static Texture2D MergeTextures(Texture2D iBaseTexture, Texture2D iTextureToAdd)
	{
		Texture2D mergedTexture = Instantiate(iBaseTexture);
		if(iBaseTexture.width != iTextureToAdd.width || iBaseTexture.height != iTextureToAdd.height)
		{
			Debug.LogError("Unable to merge 2 textures with different sizes.");
			return mergedTexture;
		}

		Color32[] basePixelColors = iBaseTexture.GetPixels32();
		Color32[] pixelColorsToAdd = iTextureToAdd.GetPixels32();
		int pixelIndex = 0;
		Color32 addedCol;
		float alpha, oneMinusAlpha;
		while(pixelIndex < basePixelColors.Length && pixelIndex < pixelColorsToAdd.Length)
		{
			ref Color32 colRef = ref basePixelColors[pixelIndex];
			addedCol = pixelColorsToAdd[pixelIndex];
			alpha = addedCol.a / 255f;
			oneMinusAlpha = 1 - alpha;

			colRef.r = (byte)(colRef.r * oneMinusAlpha + addedCol.r * alpha);
			colRef.g = (byte)(colRef.g * oneMinusAlpha + addedCol.g * alpha);
			colRef.b = (byte)(colRef.b * oneMinusAlpha + addedCol.b * alpha);

			pixelIndex++;
		}

		mergedTexture.SetPixels32(basePixelColors);
		mergedTexture.Apply();
		return mergedTexture;
	}

	public void InitTerrainSprites()
	{
		m_TerrainGeneratedSprites.Clear();

		if(!AreTerrainSpritesValid())
		{
			Debug.LogError("Will generate blank map texture.");
			return;
		}

		foreach(TerrainSprites terrainSprite in m_TerrainSprites)
		{
			if(m_TerrainGeneratedSprites.ContainsKey(terrainSprite.TerrainType))
				continue;

			List<RandomTile> terrainTiles = new List<RandomTile>();
			foreach(RandomSprite randomSprite in terrainSprite.BaseSprites)
			{
				Texture2D baseTexture = ResizeTexture(randomSprite.Sprite.texture, m_PixelPerCell, m_PixelPerCell);
				foreach(RandomSprite randomMotive in terrainSprite.RandomMotives)
				{
					RandomTile randomTile = new RandomTile();
					randomTile.Probability = randomSprite.Probability * randomMotive.Probability;
					Texture2D motiveTexture = ResizeTexture(randomMotive.Sprite.texture, m_PixelPerCell, m_PixelPerCell);
					Sprite mergedSprite = Sprite.Create(
						MergeTextures(baseTexture, motiveTexture),
						new Rect(0, 0, m_PixelPerCell, m_PixelPerCell),
						0.5f * Vector2.one,
						m_PixelPerCell);
					randomTile.Tile = TileUtility.DefaultTile(mergedSprite);

					terrainTiles.Add(randomTile);
				}
			}
			m_TerrainGeneratedSprites.Add(terrainSprite.TerrainType, terrainTiles);
		}
	}

	public int GetSeed()
	{
		return m_Seed;
	}

	[Button]
	public void GenerateRandomMap()
	{
		InitMap(Random.Range(int.MinValue, int.MaxValue)); // #TODO improve random seed generation
	}

	[Button]
	public void RegenerateMap()
	{
		InitMap(m_Seed);
	}

	private static Sprite GetRandomSprite(List<RandomSprite> iRandomSprites)
	{
		if(iRandomSprites == null)
			return null;

		double randVal = Random.value;
		foreach(RandomSprite randomSprite in iRandomSprites)
		{
			if(randVal <= randomSprite.Probability)
				return randomSprite.Sprite;

			randVal -= randomSprite.Probability;
		}

		return null;
	}

	private static TileBase GetRandomTile(List<RandomTile> iRandomTiles)
	{
		if(iRandomTiles == null)
			return null;

		double randVal = Random.value;
		foreach(RandomTile randomTile in iRandomTiles)
		{
			if(randVal <= randomTile.Probability)
				return randomTile.Tile;

			randVal -= randomTile.Probability;
		}

		return null;
	}

	public void InitMap(int iSeed)
	{
		if(m_MapTilemap != null)
			Destroy(m_MapTilemap.gameObject);

		m_Seed = iSeed;
		Random.InitState(iSeed);
		Vector2 seedOffset = Random.insideUnitCircle * Random.Range(-10000, 10000);
		Vector2 seedOffset2 = Random.insideUnitCircle * Random.Range(-10000, 10000);

		m_MapTilemap = Instantiate(m_TilemapPrefab);
		m_MapTilemap.transform.parent = m_WorldGridGeometry.transform;

		int xStart = -m_MapWidth / 2;
		int yStart = -m_MapHeight / 2;
		for(int xx = 0; xx < m_MapWidth; xx++)
		{
			for(int yy = 0; yy < m_MapHeight; yy++)
			{
				Vector2Int gridPos = new Vector2Int(xStart + xx, yStart + yy);
				TerrainType terrainType = _GetTileType(gridPos, seedOffset, seedOffset2);

				TileBase terrainTile = GetRandomTile(m_TerrainGeneratedSprites.GetValueOrDefault(terrainType, null));
				if(terrainTile == null)
					continue;
				m_MapTilemap.SetTile(new Vector3Int(gridPos.x, gridPos.y), terrainTile);
			}
		}
		m_MapTilemap.RefreshAllTiles();
	}

	private TerrainType _GetTileType(Vector2Int iTileCoord, Vector2 iSeed1, Vector2 iSeed2)
	{
		TerrainType terrainType = TerrainType.Grass;

		// out of border
		int minX = -m_MapWidth / 2;
		int maxX = (m_MapWidth + 1) / 2 - 1;
		int minY = -m_MapHeight / 2;
		int maxY = (m_MapHeight + 1) / 2 - 1;
		if(iTileCoord.x <= minX || iTileCoord.x >= maxX || iTileCoord.y <= minY || iTileCoord.y >= maxY)
			return TerrainType.Border;

		// 1 - terrain height
		// Three states :
		//      - below water level
		//      - over rock level
		//      - plain
		float waterVal = GetPerlin(iTileCoord, iSeed1, m_TerrainScale * 2);
		waterVal += GetPerlin(iTileCoord, iSeed2, m_TerrainScale);
		if(waterVal <= m_WaterLevel)
			terrainType = TerrainType.Water;

		float rockVal = GetPerlin(iTileCoord, iSeed1 - 10000 * Vector2.one, m_TerrainScale);
		rockVal += GetPerlin(iTileCoord, iSeed2 - 10000 * Vector2.down, m_TerrainScale / 2);
		if(waterVal >= m_RockLevel)
			terrainType = TerrainType.Rock;

		// 2 - temperature
		// Two states :
		//      - hot
		//      - normal
		float temperature = GetPerlin(iTileCoord, iSeed1 + 1000 * Vector2.up, m_TerrainScale);
		if(terrainType == TerrainType.Grass && temperature >= m_SandTemperature)
			terrainType = TerrainType.Sand;

		// 3 - spawning ressources
		// iron, copper & coal ores     in rocks
		// sulfur                       in grass & sand
		// oil                          in water, sand & grass
		bool isCoal = GetPerlin(iTileCoord, iSeed1 + 100 * Vector2.up, m_RessourcesScale) <= m_CoalFrequency;
		bool isCopper = GetPerlin(iTileCoord, iSeed1 + 100 * Vector2.down, m_RessourcesScale) <= m_CopperFrequency;
		bool isIron = GetPerlin(iTileCoord, iSeed1 + 100 * Vector2.left, m_RessourcesScale) <= m_IronFrequency;
		bool isSulfur = GetPerlin(iTileCoord, iSeed1 + 100 * Vector2.right, m_RessourcesScale) <= m_SulfurFrequency;
		float oilF = GetPerlin(iTileCoord, iSeed1 - 1000 * Vector2.one, m_RessourcesScale);
		oilF += GetPerlin(iTileCoord, iSeed2 - 1000 * Vector2.one, m_RessourcesScale / 2);
		bool isOil = oilF <= m_OilFrequency;

		if(terrainType == TerrainType.Rock)
		{
			if(isCopper)
				terrainType = TerrainType.CopperOre;
			if(isIron)
				terrainType = TerrainType.IronOre;
			if(isCoal)
				terrainType = TerrainType.CoalOre;
		}

		if(terrainType == TerrainType.Sand || terrainType == TerrainType.Grass)
		{
			if(isSulfur)
				terrainType = TerrainType.SulfurOre;
		}

		if(terrainType == TerrainType.Water || terrainType == TerrainType.Sand || terrainType == TerrainType.Grass)
		{
			if(isOil)
				terrainType = TerrainType.Oil;
		}

		return terrainType;
	}

	public TerrainType GetTileType(Vector3Int iTileCoord)
	{
		Random.InitState(m_Seed);
		Vector2 seedOffset = Random.insideUnitCircle * Random.Range(-10000, 10000);
		Vector2 seedOffset2 = Random.insideUnitCircle * Random.Range(-10000, 10000);
		return _GetTileType(new Vector2Int(iTileCoord.x, iTileCoord.y), seedOffset, seedOffset2);
	}

	public TerrainType GetTileType(Vector3 iWorldCoord)
	{
		Vector3Int gridCoord = m_WorldGridGeometry.WorldToCell(iWorldCoord);
		return GetTileType(gridCoord);
	}

	private static float GetPerlin(Vector2Int iPos, Vector2 iSeed, float iScale = 1)
	{
		float scale;
		if(iScale != 0)
			scale = 1 / iScale;
		else
			scale = 1;

		return Mathf.PerlinNoise(iPos.x * scale + iSeed.x, iPos.y * scale + iSeed.y);
	}
}
