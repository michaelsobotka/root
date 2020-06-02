using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneController : MonoBehaviour
{
    public GameObject playerPrefab, dogPrefab, foodPrefab, cellPrefab, scissorsPrefab, trapPrefab, vacuumPrefab, armedTrapPrefab, armedVacuumPrefab;
    public Joystick joystick;
    private GameController gameController;
    private MazeGenerator mazeGenerator;

    private int foodCount;



    private List<Transform> foodSpawnCells, itemSpawnCells, dogSpawnCells;

    private void Awake()
    {
        gameController = GameController.GetInstance();
        gameController.gameSceneController = this;

        foodCount = gameController.GetFoodCount();
        mazeGenerator = gameObject.AddComponent(typeof(MazeGenerator)) as MazeGenerator;
        mazeGenerator.Init(gameController.GetMazeRows(), gameController.GetMazeColumns(), 2, cellPrefab);

        mazeGenerator.GenerateMaze();

        SetupSpawnPositions();
        StartCoroutine(StartLevel()); // Move to button 'Ready' ?
    }

    public MazeGenerator GetMazeGenerator()
    {
        return mazeGenerator;
    }

    private void SetupSpawnPositions()
    {
        foodSpawnCells = mazeGenerator.GetCornerCells();
        itemSpawnCells = mazeGenerator.GetItemSpawnCells();
        dogSpawnCells = mazeGenerator.GetDogSpawnCells();
    }

    IEnumerator StartLevel()
    {
        yield return new WaitForSeconds(0.1f);
        SpawnInitialFood();
        SpawnInitialItems();
        yield return new WaitForSeconds(0.1f);
        GameObject player = Instantiate(playerPrefab);
        gameController.SetPlayer(player.GetComponent<Player>());
        StartCoroutine(SpawnInitialDogs());
    }

    private void SpawnInitialFood()
    {
        for (int i = 0; i < gameController.GetFoodCount(); i++)
        {
            SpawnFood(foodSpawnCells[i]);
        }
    }

    private void SpawnFood(Transform spawnCell)
    {
        Instantiate(foodPrefab, spawnCell.position, spawnCell.rotation);
    }

    private void SpawnInitialItems()
    {
        int random = Random.Range(0, itemSpawnCells.Count);
        SpawnScissors(itemSpawnCells[random]);
        itemSpawnCells.RemoveAt(random);

        random = Random.Range(0, itemSpawnCells.Count);
        SpawnTrap(itemSpawnCells[random]);
        itemSpawnCells.RemoveAt(random);

        random = Random.Range(0, itemSpawnCells.Count);
        SpawnVacuum(itemSpawnCells[random]);
        itemSpawnCells.RemoveAt(random);
    }

    private void SpawnScissors(Transform spawnCell)
    {
        Instantiate(scissorsPrefab, spawnCell.position, spawnCell.rotation);
    }
    private void SpawnTrap(Transform spawnCell)
    {
        Instantiate(trapPrefab, spawnCell.position, spawnCell.rotation);
    }
    
    private void SpawnVacuum(Transform spawnCell)
    {
        Instantiate(vacuumPrefab, spawnCell.position, spawnCell.rotation);
    }



    private IEnumerator SpawnInitialDogs(){
        for (int i = 0; i < gameController.GetDogCount(); i++){
            yield return new WaitForSeconds(1f);
            int j = Random.Range(0, dogSpawnCells.Count);
            SpawnDog(dogSpawnCells[j]);
            dogSpawnCells.RemoveAt(j);
        }
    }

    private void SpawnDog(Transform spawnCell)
    {
        GameObject newDog = Instantiate(dogPrefab, spawnCell.position, spawnCell.rotation);
        newDog.GetComponent<Dog>().Init(spawnCell);
    }

    public void ArmVacuum(Transform spawnCell)
    {
        GameObject newVacuum = Instantiate(armedVacuumPrefab, spawnCell.position, spawnCell.rotation);
        newVacuum.GetComponent<Vacuum>().Init(spawnCell);
    }

    public Transform GetNextDogSpawnCell()
    {
        // Rotate List
        Transform firstCell = dogSpawnCells[0];
        dogSpawnCells.RemoveAt(0);
        dogSpawnCells.Add(firstCell);

        return dogSpawnCells[0];
    }

    public void PlaceTrap(Transform cell)
    {
        Instantiate(armedTrapPrefab, cell.position, cell.rotation);
    }

}
