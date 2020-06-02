using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// FIXME: Spawn player at purple, get to center and back again while dodging patrols

public class Dog : MonoBehaviour
{
    public Animator animator;

    public GameObject dogDeathAnimPrefab;
    private float moveSpeed = 2;
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
        SetAnimatorFloats();
    }

    private void SetAnimatorFloats(){
        
        Vector3 direction = targetCell.position - transform.position;

        if (direction.x > 0.1)
            direction.x = 1;
        else if (direction.x < -0.1)
            direction.x = -1;
        else
            direction.x = 0;

        if (direction.y > 0.1)
            direction.y = 1;
        else if (direction.y < -0.1)
            direction.y = -1;
        else
            direction.y = 0;

        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);
        animator.SetFloat("Speed", direction.sqrMagnitude);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Dog"))
        {
            targetCell = previousCell;
            SetAnimatorFloats();
        }

        if (collision.tag.Equals("ArmedTrap"))
        {
            // Play animation?
            Destroy(collision.gameObject);
            StartCoroutine(Die());
        }

        if (collision.tag.Equals("ArmedVacuum"))
        {
            // Play animation?
            collision.transform.GetComponent<Vacuum>().Explode();
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die() {

            GameObject dogDeath = Instantiate(dogDeathAnimPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
            yield return new WaitForSeconds(2f);
            Destroy(dogDeath);
    }


}
