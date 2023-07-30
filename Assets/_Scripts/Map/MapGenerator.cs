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

	[Serializable]
	private struct RessourceSprite
	{
		public TerrainType RessourceType;
		public Sprite Sprite;
	}

	[SerializeField] List<TerrainSprites> m_TerrainSprites = new List<TerrainSprites>();
	[SerializeField] List<RessourceSprite> m_RessourceSprites = new List<RessourceSprite>();
	private Dictionary<TerrainType, List<RandomTile>> m_BaseTerrainGeneratedSprites = new Dictionary<TerrainType, List<RandomTile>>();
	private Dictionary<TerrainType, Dictionary<TerrainType, List<RandomTile>>> m_RessourcesGeneratedSprites =
		new Dictionary<TerrainType, Dictionary<TerrainType, List<RandomTile>>>();

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

		bool areProbabilitiesValid = (Math.Abs(probabilitiesSum - 1) <= 0.01);
		if(!areProbabilitiesValid)
			Debug.LogError("The sum of the probabilitie(s) (" + probabilitiesSum + ") is not equal to 1.");

		return areProbabilitiesValid && areSpritesValid;
	}

	private bool AreSpritesValid()
	{
		if(m_TerrainSprites == null || m_RessourceSprites == null)
		{
			Debug.LogError("Null list.");
			return false;
		}

		HashSet<TerrainType> allTerrainTypesWithSprites = new HashSet<TerrainType>();
		int nbInvalidTerrainSprites = 0;
		foreach(TerrainSprites terrainSprites in m_TerrainSprites)
		{
			bool isTerrainSpritesValid = true;

			if(allTerrainTypesWithSprites.Contains(terrainSprites.TerrainType))
			{
				Debug.LogError(terrainSprites.TerrainType + " is already present.");
				isTerrainSpritesValid = false;
			}
			allTerrainTypesWithSprites.Add(terrainSprites.TerrainType);

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

		foreach(RessourceSprite ressourceSprite in m_RessourceSprites)
		{
			bool isRessourceSpriteValid = true;

			if(allTerrainTypesWithSprites.Contains(ressourceSprite.RessourceType))
			{
				Debug.LogError(ressourceSprite.RessourceType + " is already present.");
				isRessourceSpriteValid = false;
			}
			allTerrainTypesWithSprites.Add(ressourceSprite.RessourceType);

			if(ressourceSprite.Sprite == null || ressourceSprite.Sprite.texture == null)
			{
				Debug.LogError("Null sprite.");
				isRessourceSpriteValid = false;
			}
			else if(!ressourceSprite.Sprite.texture.isReadable)
			{
				Debug.LogError(ressourceSprite.Sprite.texture.name + " is not readable. Please check \"Read/Write\" in texture import settings.");
				isRessourceSpriteValid = false;
			}

			if(!isRessourceSpriteValid)
				nbInvalidTerrainSprites++;
		}

		foreach(TerrainType terrainType in Enum.GetValues(typeof(TerrainType)))
		{
			if(!allTerrainTypesWithSprites.Contains(terrainType))
			{
				Debug.LogError("Missing sprites for " + terrainType.ToString() + ".");
				nbInvalidTerrainSprites++;
			}
		}

		if(nbInvalidTerrainSprites != 0)
		{
			Debug.LogError(nbInvalidTerrainSprites + " invalid entries.");
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
		if(iBaseTexture.width != iTextureToAdd.width || iBaseTexture.height != iTextureToAdd.height)
		{
			Debug.LogError("Unable to merge 2 textures with different sizes.");
			return iBaseTexture;
		}

		// Texture2D mergedTexture = new Texture2D(iBaseTexture.width, iBaseTexture.height, iBaseTexture.format, false);
		Texture2D mergedTexture = Instantiate(iBaseTexture);
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
		m_BaseTerrainGeneratedSprites.Clear();

		if(!AreSpritesValid())
		{
			Debug.LogError("Will generate blank map texture.");
			return;
		}

		Dictionary<TerrainType, Texture2D> resizedRessourcesSprites = new Dictionary<TerrainType, Texture2D>();
		foreach(RessourceSprite ressourceSprite in m_RessourceSprites)
		{
			Dictionary<TerrainType, List<RandomTile>> terrains = new Dictionary<TerrainType, List<RandomTile>>();
			foreach(TerrainSprites terrainSprites in m_TerrainSprites)
				terrains.Add(terrainSprites.TerrainType, new List<RandomTile>());
			m_RessourcesGeneratedSprites.Add(ressourceSprite.RessourceType, terrains);
			resizedRessourcesSprites.Add(ressourceSprite.RessourceType, ResizeTexture(ressourceSprite.Sprite.texture, m_PixelPerCell, m_PixelPerCell));
		}

		foreach(TerrainSprites terrainSprite in m_TerrainSprites)
		{
			if(m_BaseTerrainGeneratedSprites.ContainsKey(terrainSprite.TerrainType))
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
					Texture2D mergedTexture = MergeTextures(baseTexture, motiveTexture);

					Sprite mergedSprite = Sprite.Create(
						mergedTexture,
						new Rect(0, 0, m_PixelPerCell, m_PixelPerCell),
						0.5f * Vector2.one,
						m_PixelPerCell);
					randomTile.Tile = TileUtility.DefaultTile(mergedSprite);
					terrainTiles.Add(randomTile);

					foreach(KeyValuePair<TerrainType, Texture2D> ressourceTexture in resizedRessourcesSprites)
					{
						RandomTile terrainWithRessourceRandomTile = new RandomTile();
						terrainWithRessourceRandomTile.Probability = randomTile.Probability;
						Sprite terrainWithRessourceSprite = Sprite.Create(
						MergeTextures(mergedTexture, ressourceTexture.Value),
						new Rect(0, 0, m_PixelPerCell, m_PixelPerCell),
						0.5f * Vector2.one,
						m_PixelPerCell);
						terrainWithRessourceRandomTile.Tile = TileUtility.DefaultTile(terrainWithRessourceSprite);
						m_RessourcesGeneratedSprites[ressourceTexture.Key][terrainSprite.TerrainType].Add(terrainWithRessourceRandomTile);
					}
				}
			}
			m_BaseTerrainGeneratedSprites.Add(terrainSprite.TerrainType, terrainTiles);
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
				TerrainType baseTerrainType;
				TerrainType terrainType = _GetTileType(gridPos, seedOffset, seedOffset2, out baseTerrainType);

				TileBase terrainTile;
				if(m_RessourcesGeneratedSprites.ContainsKey(terrainType) && m_RessourcesGeneratedSprites[terrainType].ContainsKey(baseTerrainType))
					terrainTile = GetRandomTile(m_RessourcesGeneratedSprites[terrainType][baseTerrainType]);
				else
					terrainTile = GetRandomTile(m_BaseTerrainGeneratedSprites.GetValueOrDefault(terrainType, null));

				if(terrainTile == null)
					continue;

				m_MapTilemap.SetTile(new Vector3Int(gridPos.x, gridPos.y), terrainTile);
			}
		}
		m_MapTilemap.RefreshAllTiles();
	}

	private TerrainType _GetTileType(Vector2Int iTileCoord, Vector2 iSeed1, Vector2 iSeed2, out TerrainType oBaseTerrainType)
	{
		// out of border
		int minX = -m_MapWidth / 2;
		int maxX = (m_MapWidth + 1) / 2 - 1;
		int minY = -m_MapHeight / 2;
		int maxY = (m_MapHeight + 1) / 2 - 1;
		if(iTileCoord.x <= minX || iTileCoord.x >= maxX || iTileCoord.y <= minY || iTileCoord.y >= maxY)
		{
			oBaseTerrainType = TerrainType.Border;
			return oBaseTerrainType;
		}

		oBaseTerrainType = TerrainType.Grass;

		// 1 - terrain height
		// Three states :
		//      - below water level
		//      - over rock level
		//      - plain
		float waterVal = GetPerlin(iTileCoord, iSeed1, m_TerrainScale * 2);
		waterVal += GetPerlin(iTileCoord, iSeed2, m_TerrainScale);
		if(waterVal <= m_WaterLevel)
			oBaseTerrainType = TerrainType.Water;

		float rockVal = GetPerlin(iTileCoord, iSeed1 - 10000 * Vector2.one, m_TerrainScale);
		rockVal += GetPerlin(iTileCoord, iSeed2 - 10000 * Vector2.down, m_TerrainScale / 2);
		if(waterVal >= m_RockLevel)
			oBaseTerrainType = TerrainType.Rock;

		// 2 - temperature
		// Two states :
		//      - hot
		//      - normal
		float temperature = GetPerlin(iTileCoord, iSeed1 + 1000 * Vector2.up, m_TerrainScale);
		if(oBaseTerrainType == TerrainType.Grass && temperature >= m_SandTemperature)
			oBaseTerrainType = TerrainType.Sand;

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

		if(oBaseTerrainType == TerrainType.Rock)
		{
			if(isCopper)
				return TerrainType.CopperOre;
			if(isIron)
				return TerrainType.IronOre;
			if(isCoal)
				return TerrainType.CoalOre;
		}

		if(oBaseTerrainType == TerrainType.Sand || oBaseTerrainType == TerrainType.Grass)
		{
			if(isSulfur)
				return TerrainType.SulfurOre;
		}

		if(oBaseTerrainType == TerrainType.Water || oBaseTerrainType == TerrainType.Sand || oBaseTerrainType == TerrainType.Grass)
		{
			if(isOil)
				return TerrainType.Oil;
		}

		return oBaseTerrainType;
	}

	public TerrainType GetTileType(Vector3Int iTileCoord)
	{
		Random.InitState(m_Seed);
		Vector2 seedOffset = Random.insideUnitCircle * Random.Range(-10000, 10000);
		Vector2 seedOffset2 = Random.insideUnitCircle * Random.Range(-10000, 10000);
		TerrainType baseTerrainType;
		return _GetTileType(new Vector2Int(iTileCoord.x, iTileCoord.y), seedOffset, seedOffset2, out baseTerrainType);
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
