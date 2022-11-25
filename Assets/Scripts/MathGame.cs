using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathGame : MonoBehaviour
{
    public GameObject Train;
    public GameObject Choice;
    public Sprite Circle;
    public Sprite Square;
    public Sprite Triangle;
    // Start is called before the first frame update
    void Start()
    {
        NewTrain();
    }

    bool trainArriving = false;
    bool trainLeaving = false;
    bool departed = false;
    bool arrived = false;
    // Update is called once per frame
    void Update()
    {
        if (train != null && trainArriving)
        {
            train.transform.position = new Vector3(train.transform.position.x - (Time.deltaTime * 7), train.transform.position.y, 0);

            if (train.transform.position.x <= -3f)
            {
                trainArriving = false;
                arrived = true;
            }
        }

        if (train != null && trainLeaving)
        {
            train.transform.position = new Vector3(train.transform.position.x - (Time.deltaTime * 7), train.transform.position.y, 0);

            if (train.transform.position.x <= -20f)
            {
                trainLeaving = false;
                departed = true;
            }
        }

        if (train != null && departed)
        {
            departed = false;
            Destroy(train);
        }

        if (train == null)
        {
            NewTrain();
        }

        if (train != null && arrived)
        {
            DragAndDrop();
        }
    }

    private GameObject train;
    private GameObject answerBox;
    private GameObject choice1;
    private GameObject choice2;
    private GameObject choice3;
    private GameObject correctChoice;
    void NewTrain()
    {
        int first = ((int)(Random.value * 10) % 6) + 1;
        int second = ((int)(Random.value * 10) % 6);
        int answer = first + second;

        train = Instantiate(Train);
        train.transform.position = new Vector3(16.5f, -2.5f, 0);
        answerBox = GameObject.FindGameObjectWithTag("AnswerBox");
        choice1 = Instantiate(Choice);
        choice1.transform.position = new Vector3(choice1.transform.position.x - 4, choice1.transform.position.y);
        choice1.GetComponent<SpriteRenderer>().sprite = Circle;
        choice2 = Instantiate(Choice);
        choice2.GetComponent<SpriteRenderer>().sprite = Triangle;
        choice3 = Instantiate(Choice);
        choice3.transform.position = new Vector3(choice3.transform.position.x + 4, choice3.transform.position.y);
        choice3.GetComponent<SpriteRenderer>().sprite = Square;


        GameObject.FindGameObjectWithTag("Number1").GetComponent<TMPro.TextMeshPro>().text = first.ToString();
        GameObject.FindGameObjectWithTag("Number2").GetComponent<TMPro.TextMeshPro>().text = second.ToString();

        choice1.GetComponentInChildren<TMPro.TextMeshPro>().text = (answer + 2).ToString();
        choice2.GetComponentInChildren<TMPro.TextMeshPro>().text = (answer <= 1 ? 0 : answer - 2).ToString();
        choice3.GetComponentInChildren<TMPro.TextMeshPro>().text = (answer <= 3 ? 0 : answer - 4).ToString();

        int temp = (((int)(Random.value * 10)) % 3);

        if (temp == 0)
        {
            choice1.GetComponentInChildren<TMPro.TextMeshPro>().text = answer.ToString();
            correctChoice = choice1;
            answerBox.GetComponent<SpriteRenderer>().sprite = Circle;
        }
        else if (temp == 1)
        {
            choice2.GetComponentInChildren<TMPro.TextMeshPro>().text = answer.ToString();
            correctChoice = choice2;
            answerBox.GetComponent<SpriteRenderer>().sprite = Triangle;
        }
        else
        {
            choice3.GetComponentInChildren<TMPro.TextMeshPro>().text = answer.ToString();
            correctChoice = choice3;
            answerBox.GetComponent<SpriteRenderer>().sprite = Square;
        }


        trainArriving = true;
    }

    Vector2 originalPosition;
    GameObject draggedObject;
    void DragAndDrop()
    {
        if (Input.touchCount > 0)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            if (draggedObject == null)
            {
                if (choice1.GetComponent<BoxCollider2D>().OverlapPoint(mousePosition))
                {
                    originalPosition = choice1.transform.position;
                    draggedObject = choice1;
                }

                if (choice2.GetComponent<BoxCollider2D>().OverlapPoint(mousePosition))
                {
                    originalPosition = choice2.transform.position;
                    draggedObject = choice2;
                }

                if (choice3.GetComponent<BoxCollider2D>().OverlapPoint(mousePosition))
                {
                    originalPosition = choice3.transform.position;
                    draggedObject = choice3;
                }
            }

            if (draggedObject != null)
            {
                draggedObject.transform.position = new Vector3(mousePosition.x, mousePosition.y, -5);
            }
        }
        else
        {
            if (draggedObject != null)
            {
                Collider2D first = draggedObject.GetComponent<BoxCollider2D>();
                Collider2D second = train.GetComponentInChildren<BoxCollider2D>();
                if (Physics2D.IsTouching(first, second) && correctChoice == draggedObject)
                {
                    train.GetComponentInChildren<ParticleSystem>().Play();
                    answerBox.GetComponentInChildren<TMPro.TextMeshPro>().text = draggedObject.GetComponentInChildren<TMPro.TextMeshPro>().text;
                    trainLeaving = true;

                    Destroy(choice1);
                    Destroy(choice2);
                    Destroy(choice3);

                    draggedObject = null;
                }
                else
                {
                    draggedObject.transform.position = originalPosition;
                    draggedObject = null;
                }
            }
        }
    }
}
