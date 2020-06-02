using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    private int mazeRows;
    private int mazeColumns;
    private GameObject cellPrefab;

    private int centreSize = 2;

    private Dictionary<Vector2, Cell> allCells = new Dictionary<Vector2, Cell>();
    private Dictionary<Transform, Vector2> transformToGridPos;

    private List<Cell> unvisited = new List<Cell>();
    private List<Cell> stack = new List<Cell>();
    private List<Transform> deadEndCells = new List<Transform>();

    private Cell[] centreCells = new Cell[4];

    private Cell currentCell;
    private Cell checkCell;
    private Cell goalCell;

    private Vector2[] neighbourPositions = new Vector2[] { new Vector2(-1, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, -1) };
    private float cellSize;

    private GameObject mazeParent;

    public void Init(int mazeRows, int mazeColumns, int centreSize, GameObject cellPrefab)
    {
        this.mazeRows = mazeRows;
        this.mazeColumns = mazeColumns;
        this.centreSize = centreSize;
        this.cellPrefab = cellPrefab;
    }

    public void InitTransformToGridPos()
    {
        transformToGridPos = new Dictionary<Transform, Vector2>();
        foreach (KeyValuePair<Vector2, Cell> pair in allCells)
        {
            transformToGridPos.Add(pair.Value.cellObject.transform, pair.Key);
        }
    }

    public List<Transform> GetDogSpawnCells()
    {
        List<Transform> cornerCells = new List<Transform>();

        cornerCells.Add((allCells[new Vector2(1, 1)]).cellObject.transform);
        cornerCells.Add((allCells[new Vector2(1, mazeRows)]).cellObject.transform);
        cornerCells.Add((allCells[new Vector2(mazeColumns, mazeRows)]).cellObject.transform);
        cornerCells.Add((allCells[new Vector2(mazeColumns, 1)]).cellObject.transform);

        return cornerCells;
    }

    public GameObject GetCellAt(Vector2 pos)
    {
        return allCells[pos].cellObject;
    }

    public List<Transform> GetDeadEndCells()
    {
        return deadEndCells;
    }

    public List<Transform> GetCornerCells()
    {
        List<Transform> cells = new List<Transform>();

        int r = Random.Range(1, mazeRows);

        cells.Add(allCells[new Vector2(1, Random.Range(2, mazeRows - 1))].cellObject.transform);
        cells.Add(allCells[new Vector2(mazeColumns, Random.Range(2, mazeRows - 1))].cellObject.transform);
        cells.Add(allCells[new Vector2(Random.Range(2, mazeColumns), mazeRows)].cellObject.transform);
        cells.Add(allCells[new Vector2(Random.Range(2, mazeColumns), 1)].cellObject.transform);

        return cells;
    }

    public List<Transform> GetItemSpawnCells()
    {
        List<Cell> cells = new List<Cell>(allCells.Values);

        foreach (Cell centre in centreCells)
        {
            cells.Remove(centre);
        }

        foreach (Cell edge in GetEdgeCells())
        {
            cells.Remove(edge);
        }

        List<Transform> itemCells = new List<Transform>();

        foreach (Cell cell in cells)
        {
            itemCells.Add(cell.cellObject.transform);
        }
        return itemCells;
    }

    //public List<Vector2> GetCenterCellPositions()
    //{
    //    List<Vector2> positions = new List<Vector2>();

    //    positions.Add(new Vector2(mazeColumns / 2, mazeRows / 2 + 1));
    //    positions.Add(new Vector2(mazeColumns / 2 + 1, mazeRows / 2 + 1));
    //    positions.Add(new Vector2(mazeColumns / 2, mazeRows / 2));
    //    positions.Add(new Vector2(mazeColumns / 2 + 1, mazeRows / 2));

    //    return positions;
    //}

    public void GenerateMaze()
    {
        deadEndCells = new List<Transform>();
        if (mazeParent != null) DeleteMaze();
        CreateLayout();
        InitTransformToGridPos();
    }
    public void CreateLayout()
    {
        InitValues();
        Vector2 startPos = new Vector2(-(cellSize * (mazeColumns / 2)) + (cellSize / 2), -(cellSize * (mazeRows / 2)) + (cellSize / 2));
        Vector2 spawnPos = startPos;

        for (int x = 1; x <= mazeColumns; x++)
        {
            for (int y = 1; y <= mazeRows; y++)
            {
                GenerateCell(spawnPos, new Vector2(x, y));
                spawnPos.y += cellSize;
            }
            spawnPos.y = startPos.y;
            spawnPos.x += cellSize;
        }

        CreateCentre();
        RunAlgorithm();
    }

    public void RunAlgorithm()
    {
        unvisited.Remove(currentCell);

        while (unvisited.Count > 0)
        {
            List<Cell> unvisitedNeighbours = GetUnvisitedNeighbours(currentCell);
            if (unvisitedNeighbours.Count > 0)
            {
                checkCell = unvisitedNeighbours[Random.Range(0, unvisitedNeighbours.Count)];
                stack.Add(currentCell);
                CompareWalls(currentCell, checkCell);
                currentCell = checkCell;
                unvisited.Remove(currentCell);
            }
            else if (stack.Count > 0)
            {
                deadEndCells.Add(currentCell.cellObject.transform);

                currentCell = stack[stack.Count - 1];
                stack.Remove(currentCell);
            }
        }
    }

    private List<Cell> GetEdgeCells()
    {
        List<Cell> edgeCells = new List<Cell>();

        foreach (KeyValuePair<Vector2, Cell> cell in allCells)
        {
            if (cell.Key.x == 0 || cell.Key.x == mazeColumns || cell.Key.y == 0 || cell.Key.y == mazeRows)
            {
                edgeCells.Add(cell.Value);
            }
        }
        return edgeCells;
    }

    public List<Cell> GetUnvisitedNeighbours(Cell curCell)
    {
        List<Cell> neighbours = new List<Cell>();
        Cell nCell = curCell;
        Vector2 cPos = curCell.gridPos;

        foreach (Vector2 p in neighbourPositions)
        {
            Vector2 nPos = cPos + p;
            if (allCells.ContainsKey(nPos)) nCell = allCells[nPos];
            if (unvisited.Contains(nCell)) neighbours.Add(nCell);
        }

        return neighbours;
    }

    public List<Transform> GetAdjacentCells(Transform cell)
    {
        List<Transform> neighbours = new List<Transform>();

        Vector2 cellPos = transformToGridPos[cell];

        foreach (Vector2 direction in neighbourPositions)
        {
            Vector2 neighbourPos = cellPos + direction;
            if (allCells.ContainsKey(neighbourPos))
                neighbours.Add(allCells[neighbourPos].cellObject.transform);
        }
        return neighbours;
    }
    
    public Transform GetClosestCell (Transform t)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = t.position;
        foreach(Transform cell in transformToGridPos.Keys)
        {
            Vector3 directionToTarget = cell.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if(dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = cell;
            }
        }
     
        return bestTarget;
    }

    private Cell GetCellByTransform(Transform c)
    {
        foreach (KeyValuePair<Vector2, Cell> pair in allCells)
        {
            if (c == pair.Value.cellObject.transform)
            {
                return pair.Value;
            }
        }
        return null;
    }

    public void CompareWalls(Cell cCell, Cell nCell)
    {
        if (nCell.gridPos.x < cCell.gridPos.x)
        {
            RemoveWall(nCell.cScript, 2);
            RemoveWall(cCell.cScript, 1);
        }
        else if (nCell.gridPos.x > cCell.gridPos.x)
        {
            RemoveWall(nCell.cScript, 1);
            RemoveWall(cCell.cScript, 2);
        }
        else if (nCell.gridPos.y > cCell.gridPos.y)
        {
            RemoveWall(nCell.cScript, 4);
            RemoveWall(cCell.cScript, 3);
        }
        else if (nCell.gridPos.y < cCell.gridPos.y)
        {
            RemoveWall(nCell.cScript, 3);
            RemoveWall(cCell.cScript, 4);
        }
    }
    public void RemoveWall(CellScript cScript, int wallID)
    {
        if (wallID == 1) cScript.wallL.SetActive(false);
        else if (wallID == 2) cScript.wallR.SetActive(false);
        else if (wallID == 3) cScript.wallU.SetActive(false);
        else if (wallID == 4) cScript.wallD.SetActive(false);
    }

    public void CreateCentre()
    {
        centreCells[0] = allCells[new Vector2((mazeColumns / 2), (mazeRows / 2) + 1)];
        RemoveWall(centreCells[0].cScript, 4);
        RemoveWall(centreCells[0].cScript, 2);
        centreCells[1] = allCells[new Vector2((mazeColumns / 2) + 1, (mazeRows / 2) + 1)];
        RemoveWall(centreCells[1].cScript, 4);
        RemoveWall(centreCells[1].cScript, 1);
        centreCells[2] = allCells[new Vector2((mazeColumns / 2), (mazeRows / 2))];
        RemoveWall(centreCells[2].cScript, 3);
        RemoveWall(centreCells[2].cScript, 2);
        centreCells[3] = allCells[new Vector2((mazeColumns / 2) + 1, (mazeRows / 2))];
        RemoveWall(centreCells[3].cScript, 3);
        RemoveWall(centreCells[3].cScript, 1);

        List<int> rndList = new List<int> { 0, 1, 2, 3 };
        int startCell = rndList[Random.Range(0, rndList.Count)];
        rndList.Remove(startCell);
        currentCell = centreCells[startCell];
        foreach (int c in rndList)
        {
            unvisited.Remove(centreCells[c]);
        }
    }

    public void GenerateCell(Vector2 pos, Vector2 keyPos)
    {
        Cell newCell = new Cell();

        newCell.gridPos = keyPos;

        newCell.cellObject = Instantiate(cellPrefab, pos, cellPrefab.transform.rotation);

        if (mazeParent != null) newCell.cellObject.transform.parent = mazeParent.transform;

        newCell.cellObject.name = "Cell - X:" + keyPos.x + " Y:" + keyPos.y;

        newCell.cScript = newCell.cellObject.GetComponent<CellScript>();

        allCells[keyPos] = newCell;
        unvisited.Add(newCell);
    }

    public void DeleteMaze()
    {
        if (mazeParent != null) Destroy(mazeParent);

    }

    public void InitValues()
    {
        if (IsOdd(mazeRows)) mazeRows--;
        if (IsOdd(mazeColumns)) mazeColumns--;

        if (mazeRows <= 3) mazeRows = 4;
        if (mazeColumns <= 3) mazeColumns = 4;

        cellSize = cellPrefab.transform.localScale.x;

        mazeParent = new GameObject();
        mazeParent.transform.position = Vector2.zero;
        mazeParent.name = "Maze";
    }

    public bool IsOdd(int value)
    {
        return value % 2 != 0;
    }

    public class Cell
    {
        public Vector2 gridPos;
        public GameObject cellObject;
        public CellScript cScript;
    }
}


