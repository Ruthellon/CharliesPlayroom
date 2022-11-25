using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BalloonPopGame : MonoBehaviour
{
    public GameObject Balloon;
    public Color[] BalloonColors;
    public GameObject SpeechBubble;

    private int maxBalloons = 20;
    private float startingX = -2.75f;
    private float maxX = 5.5f;
    private float startingY = -6.5f;
    private float maxY = 9f;

    private int targetColor = -1;
    private int targetsHit = 0;
    // Start is called before the first frame update
    void Start()
    {
        maxX = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
    }

    List<GameObject> balloons = new List<GameObject>();
    GameObject poppedBalloon;
    List<GameObject> poppedBalloons = new List<GameObject>();
    private double timer = 1f;
    private float zCount = -9f;
    private int runningCount = 0;
    // Update is called once per frame
    void Update()
    {
        if (targetsHit >= 5 || targetColor == -1)
        {
            targetsHit = 0;
            targetColor = (((int)(Random.value * 10)) % BalloonColors.Length);
            SpeechBubble.GetComponent<Image>().color = BalloonColors[targetColor];
        }

        timer -= Time.deltaTime;
        if (timer <= 0 && balloons.Count < maxBalloons)
        {
            runningCount++;
            GameObject newBalloon = Instantiate(Balloon);
            zCount += .01f;

            if (zCount >= -1f)
                zCount = -10f;

            newBalloon.transform.position = new Vector3(((((Random.value * 100) + balloons.Count) % maxX) + startingX), startingY, zCount);
            int color = ((int)(Random.value * 10)) % BalloonColors.Length;

            if (runningCount % 4 == 0)
                color = targetColor;

            newBalloon.GetComponent<SpriteRenderer>().color = BalloonColors[color];

            balloons.Add(newBalloon);
            timer = ((Random.value * 10) % 1f) + .5f;
        }

        if (poppedBalloon != null && !poppedBalloon.GetComponent<ParticleSystem>().isPlaying)
        {
            Destroy(poppedBalloon);
            poppedBalloon = null;
        }

        for (int i = 0; i < balloons.Count; i++)
        {
            bool popped = CheckBalloon(balloons[i]);
            if (popped && poppedBalloon == null)
            {
                poppedBalloon = balloons[i];

                if (poppedBalloon.GetComponent<SpriteRenderer>().color == BalloonColors[targetColor])
                {
                    targetsHit++;
                    poppedBalloon.GetComponent<ParticleSystem>().Play();
                    ParticleSystem.MainModule psmain = poppedBalloon.GetComponent<ParticleSystem>().main;
                    psmain.startColor = balloons[i].GetComponent<SpriteRenderer>().color;
                }

                poppedBalloon.GetComponent<SpriteRenderer>().enabled = false;
                balloons.Remove(balloons[i]);
                i--;
                continue;
            }

            if (balloons[i].transform.position.y >= maxY)
            {
                GameObject deleteballoon = balloons[i];
                balloons.Remove(balloons[i]);
                i--;
                Destroy(deleteballoon);
                continue;
            }

            balloons[i].transform.position = new Vector3(balloons[i].transform.position.x, balloons[i].transform.position.y + Time.deltaTime, balloons[i].transform.position.z);
        }
    }

    bool CheckBalloon(GameObject balloon)
    {
        if (Input.touchCount > 0)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

            if (balloon.GetComponent<CapsuleCollider2D>().OverlapPoint(mousePosition))
            {
                return true;
            }
        }
        return false;
    }
}
