using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShouldShowMini : MonoBehaviour
{
    public int frequencyIndex;

    void Start()
    {
        gameObject.SetActive(frequencyIndex < PlayerStats.CurrentLevel * 2);
    }
}
