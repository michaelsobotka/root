using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Vacuum : MonoBehaviour
{

    public GameObject explosionPrefab;
    private float moveSpeed = 4;
    private Transform currentCell, targetCell, previousCell;
    private Vector2 previousMove = Vector2.zero;
    private LayerMask walls;

    private GameController gameController;
    private GameSceneController gameSceneController;
    private MazeGenerator mazeGenerator;

    private void Awake()
    {
        gameObject.SetActive(false);
        gameController = GameController.GetInstance();
        gameSceneController = gameController.gameSceneController;
        mazeGenerator = gameSceneController.GetMazeGenerator();

        walls = LayerMask.GetMask("Walls");
    }

    public void Init(Transform spawnCell)
    {
        currentCell = spawnCell;
        targetCell = spawnCell;
        previousCell = spawnCell;
        transform.position = spawnCell.position;
        gameObject.SetActive(true);
        SetTarget();
    }

    void Update()
    {
        if (transform.position == targetCell.position)
        {
            previousCell = currentCell;
            currentCell = targetCell;
            SetTarget();
        }

    }

    private void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetCell.position, moveSpeed * Time.fixedDeltaTime);
    }

    private void SetTarget()
    {
        List<Transform> adjacentCells = mazeGenerator.GetAdjacentCells(currentCell);
        List<Transform> possibleTargets = new List<Transform>();
        //loop through current cells children, if active: remove coresponding target

        foreach (Transform cell in adjacentCells)
        {
            if (!Physics2D.Linecast(currentCell.position, cell.position, walls))
            {
                possibleTargets.Add(cell);
            }
        }

        if (possibleTargets.Count == 1)
        {
            targetCell = possibleTargets[0];
        }
        else
        {
            possibleTargets.Remove(previousCell);
            targetCell = possibleTargets[Random.Range(0, possibleTargets.Count)];
        }
        SetRotation();
    }


    public void SetRotation()
    {
        Vector2 direction = targetCell.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle -= 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void Explode()
    {
        GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
        Debug.Log(explosion + " spawned");
        Destroy(gameObject);
    }


}
