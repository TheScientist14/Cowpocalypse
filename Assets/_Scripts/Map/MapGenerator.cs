using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
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
    private struct TerrainSprite
    {
        public Sprite Sprite;
        public TerrainType TerrainType;
    }

    [SerializeField] List<TerrainSprite> m_TerrainSprites = new List<TerrainSprite>();
    private Dictionary<TerrainType, Texture2D> m_TypeTextures = new Dictionary<TerrainType, Texture2D>();

    private GameObject m_Map = null;

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
        InitSpritesDictionnary();
        GenerateRandomMap();
    }

    [Button]
    public void InitSpritesDictionnary()
    {
        m_TypeTextures.Clear();
        foreach(TerrainSprite terrainSprite in m_TerrainSprites)
        {
            if(m_TypeTextures.ContainsKey(terrainSprite.TerrainType))
                continue;
            if(terrainSprite.Sprite == null || terrainSprite.Sprite.texture == null)
            {
                Debug.LogError("Empty texture");
                continue;
            }
            if(!terrainSprite.Sprite.texture.isReadable)
            {
                Debug.LogError(terrainSprite.Sprite.texture.name + " has not been set to be readable. See in texture import settings.");
                continue;
            }

            Texture2D baseText = terrainSprite.Sprite.texture;

            if(baseText.width == m_PixelPerCell && baseText.height == m_PixelPerCell)
            {
                m_TypeTextures[terrainSprite.TerrainType] = baseText;
                continue;
            }

            // when size is not appropriate, we need to resample the texture
            RenderTexture renderTexture = new RenderTexture(m_PixelPerCell, m_PixelPerCell, 24);
            RenderTexture.active = renderTexture;
            Graphics.Blit(baseText, renderTexture);

            Texture2D resizedText = Instantiate(baseText);
            resizedText.Reinitialize(m_PixelPerCell, m_PixelPerCell);
            resizedText.ReadPixels(new Rect(0, 0, m_PixelPerCell, m_PixelPerCell), 0, 0);
            resizedText.Apply();

            m_TypeTextures[terrainSprite.TerrainType] = resizedText;
        }
    }

    public int GetSeed()
    {
        return m_Seed;
    }

    [Button]
    public void GenerateRandomMap()
    {
        InitMap(Random.Range(int.MinValue, int.MaxValue));
    }

    [Button]
    public void RegenerateMap()
    {
        InitMap(m_Seed);
    }

    public void InitMap(int iSeed)
    {
        if(m_Map != null)
            Destroy(m_Map);

        m_Seed = iSeed;
        Random.InitState(iSeed);
        Vector2 seedOffset = Random.insideUnitCircle * Random.Range(-10000, 10000);
        Vector2 seedOffset2 = Random.insideUnitCircle * Random.Range(-10000, 10000);

        m_Map = new GameObject();
        m_Map.transform.parent = transform;
        Vector3 originCellCenter = m_WorldGridGeometry.GetCellCenterWorld(Vector3Int.zero);
        Vector3 originCellBottomLeftCorner = m_WorldGridGeometry.CellToWorld(Vector3Int.zero);
        m_Map.transform.position = new Vector3((m_MapWidth % 2 == 0) ? originCellBottomLeftCorner.x : originCellCenter.x,
                                                (m_MapHeight % 2 == 0) ? originCellBottomLeftCorner.y : originCellCenter.y,
                                                0.5f);

        Texture2D mapTexture = new Texture2D(m_MapWidth * m_PixelPerCell, m_MapHeight * m_PixelPerCell, TextureFormat.RGB24, false);
        int xStart = -m_MapWidth / 2;
        int yStart = -m_MapHeight / 2;
        for(int xx = 0; xx < m_MapWidth; xx++)
        {
            for(int yy = 0; yy < m_MapHeight; yy++)
            {
                Vector2Int gridPos = new Vector2Int(xStart + xx, yStart + yy);
                TerrainType terrainType = _GetTileType(gridPos, seedOffset, seedOffset2);

                Texture2D terrainTexture = m_TypeTextures.GetValueOrDefault(terrainType, null);
                if(terrainTexture == null)
                    continue;

                mapTexture.SetPixels(xx * m_PixelPerCell, yy * m_PixelPerCell, m_PixelPerCell, m_PixelPerCell, terrainTexture.GetPixels());
            }
        }
        mapTexture.Apply();
        byte[] bytes = mapTexture.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/Art/Sprites/Terrain/DebugMapTexture.png", bytes);

        SpriteRenderer mapRenderer = m_Map.AddComponent<SpriteRenderer>();
        Sprite mapSprite = Sprite.Create(
            mapTexture, new Rect(0, 0, m_MapWidth * m_PixelPerCell, m_MapHeight * m_PixelPerCell), 0.5f * Vector2.one, m_PixelPerCell);
        mapRenderer.sprite = mapSprite;
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
