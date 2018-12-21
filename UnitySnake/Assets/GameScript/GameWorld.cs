using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameWorld : MonoBehaviour
{

    public GameObject tilePrefab;
    public GameObject bodyPrefab;
    public GameObject candy;
    public Snake SnakePrefab;

    public Vector2Int size;

    public Snake snake;

    public int TotalScore = 0;
    public int TotalEaten = 0;
    public int Bonus = 0;

    public GameObject TotalScoreLabel;
    public GameObject CandyScoreLabel;
    public GameObject BonusScoreLabel;

    // Use this for initialization
    void Start()
    {
        GenerateWorld();
        snake = Snake.Instantiate<Snake>(SnakePrefab);
        snake.transform.SetParent(this.transform);

        snake.Body.Add(GameObject.Instantiate(bodyPrefab, snake.transform));
        //snake.Body.Add(GameObject.Instantiate(bodyPrefab, snake.transform));
        //snake.Body.Add(GameObject.Instantiate(bodyPrefab, snake.transform));
        //snake.Body.Add(GameObject.Instantiate(bodyPrefab, snake.transform));

        var startPos = new Vector2Int( size.x/2, size.y/2);
        var startDir = Vector2Int.up;

        SpawnCandy();

        snake.Initialize(startPos, startDir);


        StartCoroutine("SlowUpdate");

    }

    void GenerateWorld()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                var tile = GameObject.Instantiate(tilePrefab);
                tile.transform.SetParent(this.transform);
                tile.transform.eulerAngles = new Vector3(-90, 0, 0);
                tile.transform.localPosition = new Vector2(x, y);

            }
        }
    }

    public void SpawnCandy()
    {
        int x = Random.Range(0, size.x);
        int y = Random.Range(0, size.y);

        var option = new Vector3(x, y, 0);

        if (!snake.Body.Any(b => b.transform.localPosition == option))
        {
            candy.transform.localPosition = option;
        }
        else
        {
            SpawnCandy();
        }
    }




    public void Tick()
    {
        if (snake.isDead)
            return;

        snake.Tick();

        if (Bonus > 0 || TotalEaten > 10)
            Bonus--;


        //snoepje
        if (snake.eatsCandy(candy.transform.localPosition))
        {
            SpawnCandy();
            snake.Body.Add(GameObject.Instantiate(bodyPrefab, snake.transform));

            //give score
            TotalEaten++;
            TotalScore += TotalEaten + Bonus;

            //reset bonus
            Bonus = TotalEaten;

        }

        //check borders
        if (snake.IsDead(size))
        {
            Debug.Log("Dead");
            StartCoroutine(snake.Dead());

        }

        TotalScoreLabel.GetComponent<TextMesh>().text = TotalScore.ToString();
        CandyScoreLabel.GetComponent<TextMesh>().text = TotalEaten.ToString();
        BonusScoreLabel.GetComponent<TextMesh>().text = Bonus.ToString();
    }

    public IEnumerator SlowUpdate()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            Tick();


        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            snake.dir = Vector3.up;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            snake.dir = Vector3.down;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            snake.dir = Vector3.left;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            snake.dir = Vector3.right;
        }
        //Tick();
    }
}
