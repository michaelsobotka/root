using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Animator animator;

    public GameObject scissorsAnimationPrefab;
    private GameController gameController;
    private GameSceneController gameSceneController;
    private GameSceneUIController gameSceneUIController;
    private MazeGenerator mazeGenerator;

    private int food = 0;
    private bool hasScissors = false;
    private bool hasTrap = false;
    private bool hasVacuum = false;
    private LayerMask walls;

    private float foodTimer = 0;
    private float foodGainTime = 1f;

    private bool dead = false;

    // Start is called before the first frame update
    void Awake()
    {
        gameController = GameController.GetInstance();
        gameSceneController = gameController.gameSceneController;
        gameSceneUIController = gameController.gameSceneUIController;
        mazeGenerator = gameSceneController.GetMazeGenerator();

        walls = LayerMask.GetMask("Walls");
    }

    // Update is called once per frame
    void Update()
    {
        // Increment the hit timer
        foodTimer += Time.deltaTime;


        if (Input.GetKeyDown("1"))
            UseScissors();
        if (Input.GetKeyDown("2"))
            UseTrap();
        if (Input.GetKeyDown("3"))
            UseVacuum();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string collisionTag = collision.tag;

        switch (collisionTag)
        {
            case "Scissors":
                Destroy(collision.gameObject);
                gameSceneUIController.SetHasScissors(true);
                hasScissors = true;
                break;
            case "Vacuum":
                Destroy(collision.gameObject);
                gameSceneUIController.SetHasVacuum(true);
                hasVacuum = true;
                break;
            case "Trap":
                Destroy(collision.gameObject);
                gameSceneUIController.SetHasTrap(true);
                hasTrap = true;
                break;
            case "Food":
                Destroy(collision.gameObject);
                GainFood();
                break;
            case "Dog":
                Destroy(collision.gameObject);
                Die();
                break;
            default:
                break;
        }
    }

    void GainFood()
    {
        if (foodTimer > foodGainTime)
        {
            food++;
            if (food >= gameController.GetFoodCount())
            {
                Destroy(gameObject);
                gameController.Win();
            }
            foodTimer = 0;
        }

    }

    private void Die()
    {
        if (!dead)
        {
            dead = true;
            Destroy(this);
            gameController.Lose();
        }
    }

    private IEnumerator AnimateScissors(Transform t)
    {
        GameObject scissorsAnimation = Instantiate(scissorsAnimationPrefab, transform.position, transform.rotation);
        yield return new WaitForSeconds(1f);
        Destroy(scissorsAnimation);
    }

    public void UseScissors()
    {
        if (!hasScissors)
            return;
        hasScissors = false;
        gameSceneUIController.SetHasScissors(false);

        Transform currentCell = mazeGenerator.GetClosestCell(transform);

        StartCoroutine(AnimateScissors(currentCell));

        List<Transform> adjacentCells = mazeGenerator.GetAdjacentCells(currentCell);

        foreach (Transform cell in adjacentCells)
        {
            RaycastHit2D hit = Physics2D.Linecast(currentCell.position, cell.position, walls);
            if (hit)
                hit.transform.gameObject.SetActive(false);

            hit = Physics2D.Linecast(currentCell.position, cell.position, walls);
            if (hit)
                hit.transform.gameObject.SetActive(false);
        }

    }

    public void UseTrap()
    {
        if (!hasTrap)
            return;
        hasTrap = false;
        gameSceneUIController.SetHasTrap(false);

        Transform currentCell = mazeGenerator.GetClosestCell(transform);

        gameSceneController.PlaceTrap(currentCell);
        Debug.Log("Trap Used!");
    }

    public void UseVacuum()
    {
        if (!hasVacuum)
            return;
        hasVacuum = false;
        gameSceneUIController.SetHasVacuum(false);

        gameSceneController.ArmVacuum(mazeGenerator.GetClosestCell(transform));

    }
}
