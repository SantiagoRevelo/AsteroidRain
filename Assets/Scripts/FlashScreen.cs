using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashScreen : MonoBehaviour
{

    List<GameObject> currentLivesList = new List<GameObject>();
    public GameObject livePrefab;
    public Transform livesContainer;
    public Animator flashAnimator;
    int currentLives;

    void Start()
    {
        GameManager.instance.OnUpdatePlayerLives += HandleOnChangeLivesCount;
    }

    void HandleOnChangeLivesCount(int count)
    {
        // If for some reason i don't have the needed lives to show then create the necessary
        if (count > currentLivesList.Count)
        {
            int hearts = count - currentLivesList.Count;
            for (int i = 0; i < hearts; i++)
            {
                GameObject newLive = Instantiate(livePrefab, livesContainer);
                currentLivesList.Add(newLive);
            }
        }

        // Only show the needed ones
        for (int i = 0; i < currentLivesList.Count; i++)
        {
            currentLivesList[i].SetActive(i < count);
        }

        if (count < currentLives)
        {
            flashAnimator.SetTrigger("hit");
        }

        currentLives = count;
    }
}
