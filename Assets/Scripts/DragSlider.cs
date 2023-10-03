using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragSlider : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;
    private float maxHeight = 3.25f;
    private float minHeight = 0.9f;
    public int frequencyIndex;
    public bool disabled;
    public GameObject lightGO;
    Light lightComponent;
    SliderManager sliderManager;
    GameManager gameManager;

    void Start()
    {
        sliderManager = GameObject.FindWithTag("SliderManager").GetComponent<SliderManager>();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        lightComponent = lightGO.GetComponent<Light>();

        if (frequencyIndex < 2 * PlayerStats.CurrentLevel)
        {
            lightGO.SetActive(true);
            GetComponent<AudioSource>().volume = 0.2f;
            GetComponent<AudioSource>().mute = false;
            disabled = false;
        }
    }

    void OnMouseDown()
    {
        if (disabled)
        {
            return;
        }

        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

    }

    void OnMouseDrag()
    {
        if (disabled)
        {
            return;
        }

        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = new Vector3(transform.position.x,  curPosition.y, transform.position.z);

        UpdateFrequency(Mathf.Clamp((curPosition.y - minHeight) / (maxHeight - minHeight), 0f, 1f));
    }

    void Update()
    {
        if (transform.position.y > maxHeight) 
        {
            transform.position = new Vector3(transform.position.x, maxHeight, transform.position.z);
        }

        if (transform.position.y < minHeight)
        {
            transform.position = new Vector3(transform.position.x, minHeight, transform.position.z);
        }
    }

    void UpdateFrequency(float amplitude)
    {
        if (PlayerStats.CurrentLevel == 0 && frequencyIndex == 0)
        {
            if (gameManager.creatingFirstTutorialMini)
            {
                gameManager.firstTutorialMini.SetActive(amplitude > 0.8);
            }
            else if (gameManager.creatingSecondTutorialMini)
            {
                gameManager.secondTutorialMini.SetActive(amplitude < 0.2);
            }
        }

        lightComponent.intensity = 0.5f + amplitude;
        sliderManager.SetFrequencyAmplitude(frequencyIndex, amplitude);
        GetComponent<AudioSource>().volume = 0.2f + (amplitude * 0.8f);
    }

    public void SetDisabled(bool value)
    {
        disabled = value;
    }

    public void MoveToAmplitude(float amplitude)
    {
        Vector3 newPosition = new Vector3(
            transform.position.x, 
            minHeight + (amplitude * (maxHeight - minHeight)), 
            transform.position.z
        );
        bool wasDisabled = disabled;
        SetDisabled(true);
        StartCoroutine(MoveSliderCoroutine(newPosition, sliderManager.slideDuration, wasDisabled, amplitude, GetComponent<AudioSource>().volume));
        lightComponent.intensity = 0.5f + amplitude;
    }

    IEnumerator MoveSliderCoroutine(Vector3 targetPosition, float duration, bool wasDisabled, float amplitude, float prevAmplitude)
    {
        float timeElapsed = 0f;
        Vector3 startPosition = transform.position;
        while (timeElapsed < duration)
        {
            float lerpPosition = Mathf.Clamp(1f - ((Mathf.Log(1f / (timeElapsed / duration))) / 4f), 0f, 1f);
            transform.position = Vector3.Lerp(startPosition, targetPosition, lerpPosition);
            timeElapsed += Time.deltaTime;
            GetComponent<AudioSource>().volume = 0.2f + Mathf.Lerp(prevAmplitude, amplitude, lerpPosition) * 0.8f;
            yield return null;
        }
        transform.position = targetPosition;
        SetDisabled(wasDisabled);
    }
}
