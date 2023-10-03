using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShouldShowSeedName : MonoBehaviour
{
    public int seedIndex;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(seedIndex == PlayerStats.CurrentLevel - 1);
    }
}
