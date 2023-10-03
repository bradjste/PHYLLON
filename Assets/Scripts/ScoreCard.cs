using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreCard : MonoBehaviour
{
    float floatSpeed = 0.4f;
    float floatAmplitude = 0.4f;
    float startY;
    float phaseOffset;
    public TMP_Text text;

    void Start()
    {
        startY = transform.position.y;
        phaseOffset = Random.Range(0.0f, 2 * Mathf.PI);
    }

    void Update()
    {
        transform.position = new Vector3(
            transform.position.x, 
            startY + floatAmplitude * Mathf.Sin((phaseOffset + Time.fixedTime) / floatSpeed), 
            transform.position.z
        );
    }

    public void SetScore(string score)
    {
        text.text = score;
        text.color = GetColorFromScore(score);
    }

    Color GetColorFromScore(string score)
    {
        switch(score)
        {
            case "A-": case "A+": case "A": return Color.green;
            case "B-": case "B+": case "B": return Color.magenta;
            case "C-": case "C+": case "C": return Color.blue;
            case "D-": case "D+": case "D": return Color.gray;
            case "F-": case "F+": case "F": return Color.red;
        }

        return Color.black;
    }
}
