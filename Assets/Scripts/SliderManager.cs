using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderManager : MonoBehaviour
{
    public string updateType = "mini";
    public int updateIndex = 0;
    public GameObject[] sliders;
    public float slideDuration = 10f;

    GrowthManager growthManager;
    GameManager gameManager;

    void Start()
    {
        growthManager = GameObject.FindGameObjectWithTag("GrowthManager").GetComponent<GrowthManager>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public void SetFrequencyAmplitude(int frequencyIndex, float amplitude)
    {
        if (!gameManager)
        {
            return;
        }

        switch(updateType)
        {
            case "mini":
                growthManager.miniFrequencies[updateIndex][frequencyIndex] = amplitude;
                break;
            case "final":
                growthManager.finalFrequencies[updateIndex][frequencyIndex] = amplitude;
                break;
        }

        if (growthManager.autoUpdateTestPlant)
        {
            growthManager.DestroyPlants();
            growthManager.testFrequencies[frequencyIndex] = amplitude;
            growthManager.GenerateTestPlant();
        }
    }

    public void SetSliders(float[] amplitudes)
    {
        for (int i = 0; i < sliders.Length; i++)
        {
            DragSlider dragSlider = sliders[i].GetComponent<DragSlider>();
            dragSlider.MoveToAmplitude(amplitudes[i]);
        }
    }

    public void DisableSliders()
    {
        for (int i = 0; i < sliders.Length; i++)
        {
            sliders[i].GetComponent<DragSlider>().SetDisabled(true);
        }
    }
    public void EnableSliders()
    {
        for (int i = 0; i < 2 * PlayerStats.CurrentLevel; i++)
        {
            sliders[i].GetComponent<DragSlider>().SetDisabled(false);
        }
    }
    public void MuteAllSliders()
    {
        for (int i = 0; i < 2 * PlayerStats.CurrentLevel; i++)
        {
            sliders[i].GetComponent<AudioSource>().mute = true;
        }
    }
}
