using System.Collections;
using System.Linq;
using UnityEngine;
//using static NeuralNetwork = NeuralNetwork.NeuralNetwork;

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

    public int ticks = 0;

    public GameObject TotalScoreLabel;
    public GameObject CandyScoreLabel;
    public GameObject BonusScoreLabel;

   public  NeuralNetwork.NeuralNetwork network;
    NeuralGameManager control;

    // Use this for initialization
    public void Initialize(NeuralGameManager c)
    {
        control = c;
        //network = nn;
        network = c.CreateManager(); //  new NeuralNetwork.NeuralNetwork(2, 10, 4, Random.Range(1.1f,2.8f));

        GenerateWorld();
        snake = Snake.Instantiate<Snake>(SnakePrefab);
        snake.transform.SetParent(this.transform);

        snake.Body.Add(GameObject.Instantiate(bodyPrefab, snake.transform));

        var startPos = new Vector2Int( size.x/2, size.y/2);
        var startDir = Vector2Int.up;

        //SpawnCandy();

        candy.transform.localPosition = new Vector3(8,8,0);


        snake.Initialize(startPos, startDir);

        //body
        //candy
        //size

        //StartCoroutine("SlowUpdate");
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
        {
            StopAllCoroutines();
            control.SpawnNew(this.transform.localPosition, this);
            Destroy(this.gameObject);
            return;
        }

        ticks++;


        var input = new double [2];
        //candy
        input[0] = Mathf.FloorToInt(candy.transform.localPosition.x / size.x) + transform.localPosition.y;
        //input[1] = (candy.transform.localPosition.x * candy.transform.localPosition.y) / (size.x * size.y);
        //body
        input[1] = Mathf.FloorToInt(snake.Body[0].transform.localPosition.x / size.x) + snake.Body[0].transform.localPosition.y;

        //input[2] = (snake.Body[0].transform.position.x * snake.Body[0].transform.position.y) / (size.x * size.y);
        //input[3] = (snake.Body[0].transform.position.x * snake.Body[0].transform.position.y) / (size.x * size.y);
        //score
        //input[2] = TotalEaten / (TotalScore + 1);
        //dist
        //input[3] = ticks;

        var output = network.Query(input);

        //Debug.Log(output);


        int highest = 0;
        for (int i = 0; i < output.Length; i++)
        {
            //Debug.Log(i + " - " + output[i]);

            if (output[highest] < output[i])
            {
                highest = i;
            }

        }
        //Debug.Log("Highest " + highest);


        switch (highest)
        {
            case 0:
                snake.dir = Vector3.up;
                break;
            case 1:
                snake.dir = Vector3.left;
                break;
            case 2:
                snake.dir = Vector3.down;
                break;
            case 3:
                snake.dir = Vector3.right;
                break;
            default:
                break;
        }



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
            //Debug.Log("Dead");
            //StartCoroutine(snake.Dead());
            snake.isDead = true;
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
        Tick();
    }
}
