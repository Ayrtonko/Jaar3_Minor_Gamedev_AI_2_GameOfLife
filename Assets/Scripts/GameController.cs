using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject parent;
    public TextMeshProUGUI text;
    [SerializeField] public int gridWidth;
    [SerializeField] public int gridHeight;
    public GameObject tilePrefab;
    public float updateInterval = 1f;
    public bool isAutoPlaying = false;

    private Tile[,] grid;
    private List<Tile[,]> oldGrids;
    private float timeSinceLastUpdate = 0f;

    // Start is called before the first frame update
    void Start()
    {
        oldGrids ??= new List<Tile[,]>();
        
        grid = new Tile[gridWidth, gridHeight];
        Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Vector3 position = new Vector3(x, y, 0);
                GameObject tileObj = Instantiate(tilePrefab, position, rotation);
                tileObj.transform.SetParent(parent.transform);
                Tile tile = tileObj.GetComponent<Tile>();
                tile.xPos = x;
                tile.yPos = y;
                grid[x, y] = tile;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        ToggleAutoPlay();
        AutoPlayGeneration();
        AdvanceGeneration();
        UpdateGridToPreviousGeneration();
        clearGrid();
        clearHistory();
        toggleText();
    }

    void AdvanceGeneration()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UpdateGrid();
        }
    }


    void ToggleAutoPlay()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            isAutoPlaying = !isAutoPlaying;
        }
    }

    void AutoPlayGeneration()
    {
        if (!isAutoPlaying)
            return;
        timeSinceLastUpdate += Time.deltaTime;
        if (timeSinceLastUpdate >= updateInterval)
        {
            UpdateGrid();
            timeSinceLastUpdate = 0f;
        }
    }

    private int CountAliveNeighbors(int x, int y)
    {
        int count = 0;
        for (int yy = y - 1; yy <= y + 1; yy++)
        {
            for (int xx = x - 1; xx <= x + 1; xx++)
            {
                // Skip the current tile
                if (xx == x && yy == y)
                {
                    continue;
                }

                // Wrap around the edges of the grid
                int xxWrapped = (xx + gridWidth) % gridWidth;
                int yyWrapped = (yy + gridHeight) % gridHeight;

                if (grid[xxWrapped, yyWrapped].alive)
                {
                    count++;
                }
            }
        }
        return count;
    }

    private void UpdateGrid()
    {
        // Create a new grid to store the next generation
        Tile[,] newGrid = new Tile[gridWidth, gridHeight];

        // Update each tile in the grid
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Tile tile = grid[x, y];
                int aliveNeighbors = CountAliveNeighbors(x, y);

                // Apply the rules of the Game of Life
                if (tile.alive)
                {
                    if (aliveNeighbors < 2 || aliveNeighbors > 3)
                    {
                        tile.nextAliveState = false;
                    }
                    else
                    {
                        tile.nextAliveState = true;
                    }
                }
                else
                {
                    if (aliveNeighbors == 3)
                    {
                        tile.nextAliveState = true;
                    }
                    else
                    {
                        tile.nextAliveState = false;
                    }
                }

                Vector3 tilePos = new Vector3(tile.xPos, tile.yPos, 0);
                GameObject tileObj = Instantiate(tilePrefab, tilePos, Quaternion.Euler(-90f, 0f, 0f));
                Tile newTile = tileObj.GetComponent<Tile>();
                newTile.xPos = x;
                newTile.yPos = y;
                newTile.alive = tile.nextAliveState;
                newGrid[x, y] = newTile;
            }
        }

        oldGrids.Add((Tile[,]) newGrid.Clone());
        // Destroy the old grid and replace it with the new one
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Destroy(grid[x, y].gameObject);
                grid[x, y] = newGrid[x, y];
            }
        }

        Debug.Log(oldGrids.Count);
    }

    void clearGrid()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            foreach (var tile in grid)
            {
                tile.alive = false;
            }
        }
    }

    private void UpdateGridToPreviousGeneration()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (oldGrids.Count > 1)
            {
                oldGrids.RemoveAt(oldGrids.Count - 1);

                Tile[,] previousGrid = oldGrids[oldGrids.Count - 1];

                for (int y = 0; y < gridHeight; y++)
                {
                    for (int x = 0; x < gridWidth; x++)
                    {
                        Tile previousTile = previousGrid[x, y];
                        grid[x, y].alive = previousTile.alive;
                    }
                }
            }
            Debug.Log(oldGrids.Count);
        }
        
    }

    void toggleText()
    {
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            text.enabled = !text.enabled;
        }
    }

    private void clearHistory()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            oldGrids.Clear();
        }
    }
}