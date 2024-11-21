using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class FloodFill : MonoBehaviour
{

    // Referentie naar de grid van tiles
    private SpriteRenderer[,] _tileGrid;
    
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 10;
    [SerializeField] private int tileSize = 50;
    [SerializeField] private Color replaceColor = Color.red;
    
    private float _unitSize;
    
    void Start()
    {
        _unitSize = tileSize / 100f;
        
        InitializeGrid();
        
        // Boxcollider is nodig voor OnMouseDown
        var collider = gameObject.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(gridWidth * _unitSize, gridHeight * _unitSize);
        collider.offset = new Vector2((gridWidth * _unitSize) / 2, (gridHeight * _unitSize) / 2);
    }
    
    public void StartFloodFill(int startX, int startY, Color newColor)
    {
        if (_tileGrid == null || startX < 0 || startX >= gridWidth || startY < 0 || startY >= gridHeight) return;
        
        var targetColor = _tileGrid[startX, startY].color;
        FloodFillArea(startX, startY, targetColor, newColor);
    }

    // Recursieve floodfill implementatie -> hier moeten jullie aan de slag :)
    private void FloodFillArea(int x, int y, Color targetColor, Color replacementColor)
    {
        // Check of we binnen de grid grenzen zijn
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
            return;

        // Haal de huidige kleur van de tile op
        Color currentColor = _tileGrid[x, y].color;

        // Als de huidige kleur niet de target kleur is, stoppen we
        if (currentColor != targetColor)
            return;

        // Als de huidige kleur al de replacement kleur is, stoppen we
        if (currentColor == replacementColor)
            return;

        // Verander de kleur van de huidige tile
        _tileGrid[x, y].color = replacementColor;

        // Recursief aanroepen voor alle aangrenzende tiles
        FloodFillArea(x + 1, y, targetColor, replacementColor); // Rechts
        FloodFillArea(x - 1, y, targetColor, replacementColor); // Links
        FloodFillArea(x, y + 1, targetColor, replacementColor); // Boven
        FloodFillArea(x, y - 1, targetColor, replacementColor); // Onder
    }

    // Helper methode om de grid te initialiseren
    private void InitializeGrid()
    {
        _tileGrid = new SpriteRenderer[gridWidth, gridHeight];
    
        // Maak voor elke positie een nieuwe tile aan
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Maak een nieuwe GameObject voor de tile
                var tile = new GameObject($"Tile_{x}_{y}");
            
                // Bereken de positie waarbij we rekening houden met de tileSize
                tile.transform.position = new Vector3(x * _unitSize, y * _unitSize, 0);
                tile.transform.parent = transform;

                // Voeg een SpriteRenderer toe
                var sr = tile.AddComponent<SpriteRenderer>();
                var color = GetRandomColor();
                sr.sprite = CreateDefaultSprite(Color.white);
                sr.color = color;

                // Sla de reference op in de grid
                _tileGrid[x, y] = sr;
            }
        }
    }

    private static Color GetRandomColor()
    {
        var colors = new[]
        {
            Color.red,
            Color.blue,
            Color.green,
            Color.yellow,
            new Color(1f, 0f, 1f) // paars/magenta
        };

        // Return een random kleur uit de array
        return colors[Random.Range(0, colors.Length)];
    }

    private Sprite CreateDefaultSprite(Color targetColor)
    {
        // Maak een basis vierkante sprite
        var texture = new Texture2D(tileSize, tileSize);
        var colors = new Color[tileSize * tileSize];
        for (var i = 0; i < colors.Length; i++)
        {
            colors[i] = targetColor;
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        var pixelsPerUnit = 100f;
        return Sprite.Create(texture, new Rect(0, 0, tileSize, tileSize), Vector2.one * 0.5f, pixelsPerUnit);
    }
    
    private void OnMouseDown()
    {
        // Converteer muispositie naar wereldpositie
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // Converteer wereldpositie naar lokale positie relatief aan de grid
        var localPosition = transform.InverseTransformPoint(mousePosition);
        
        // Bereken grid coördinaten
        var x = Mathf.FloorToInt(localPosition.x / _unitSize);
        var y = Mathf.FloorToInt(localPosition.y / _unitSize);
        
        StartFloodFill(x, y, replaceColor);
    }

}
