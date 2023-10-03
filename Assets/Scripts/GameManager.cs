using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject tutorialScreen;
    public GameObject mainScreen;
    public GameObject minisCamera;
    public GameObject computerCamera;
    public GameObject orderCamera;
    public GameObject finalCamera;
    bool isViewingMinis = false;
    bool playerCanViewMinis = false;
    public GameObject finishTutorialButton;
    GrowthManager growthManager;
    SliderManager sliderManager;
    public GameObject firstTutorialMini;
    public GameObject secondTutorialMini;
    public bool creatingFirstTutorialMini = false;
    public bool creatingSecondTutorialMini = false;

    public bool callibrating = false;
    public bool germinating = false;
    public bool generating = false;
    public bool skipTutorial = false;

    public GameObject[] scoreCards;
    public GameObject[] finalSpotlights;
    public GameObject beltGroup;

    public GameObject winLight;
    public GameObject loseLight;
    public Material winMaterial;
    public Material loseMaterial;

    public GameObject switchMinisText;
    public GameObject switchTextBack;
    public GameObject switchPhyllonText;

    public GameObject orderComplete;
    public GameObject orderFailed;

    public AudioClip anticipateClip;
    public AudioClip successClip;
    public AudioClip loseClip;
    public AudioClip spotlightClip;
    public AudioClip beltClip;
    public AudioClip selectClip;

    void Start()
    {
        if (PlayerStats.CurrentLevel == 0)
        {
            if (skipTutorial)
            {
                PlayerStats.CurrentLevel = 1;
                SceneManager.LoadSceneAsync(1);
            }
            else
            {
                tutorialScreen.SetActive(true);
                switchMinisText.SetActive(false);
                switchTextBack.SetActive(false);
            }
        }
        else
        {
            generating = true;
            mainScreen.SetActive(true);
            playerCanViewMinis = true;
        }

        growthManager = GameObject.FindWithTag("GrowthManager").GetComponent<GrowthManager>();
        sliderManager = GameObject.FindWithTag("SliderManager").GetComponent<SliderManager>();


        winMaterial.DisableKeyword("_EMISSION");
        winMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;

        loseMaterial.DisableKeyword("_EMISSION");
        loseMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
    }

    float CalculateRoundScore()
    {
        float score = 0f;
        for (int i = 0; i < 3; i++) { 
            score += CalcualteOrderScore(i);
        }

        return score;
    }

    float CalcualteOrderScore(int plantNumber)
    {
        if (PlayerStats.CurrentLevel == 0)
        {
            return -1;
        }

        float error = 0f;
        for (int n = 0; n < 2 * PlayerStats.CurrentLevel; n++)
        {
            error += Mathf.Pow(growthManager.orderFrequencies[plantNumber][n] - growthManager.finalFrequencies[plantNumber][n], 2);
        }

        float score = 100f * (1f - Mathf.Sqrt(error) / (2 * PlayerStats.CurrentLevel));
        scoreCards[plantNumber].GetComponent<ScoreCard>().SetScore(GetLetterFromScore(score));

        return score;
    }

    string GetLetterFromScore(float score)
    {
        if (score < 60)
        {
            return "F";
        } 
        else if(score < 63)
        {
            return "D-";
        }
        else if(score < 67)
        {
            return "D";
        }
        else if(score < 70)
        {
            return "D+";
        }
        else if(score < 73)
        {
            return "C-";
        }
        else if(score < 77)
        {
            return "C";
        }
        else if(score < 80)
        {
            return "C+";
        }
        else if (score < 83)
        {
            return "B-";
        }
        else if (score < 87)
        {
            return "B";
        }
        else if (score < 90)
        {
            return "B+";
        }
        else if (score < 93)
        {
            return "A-";
        }
        else if (score < 97)
        {
            return "A";
        }
        else if (score < 100)
        {
            return "A+";
        }

        return "-1";
    }

    public void FinishTutorial()
    {
        PlayerStats.CurrentLevel = 1;
        SceneManager.LoadSceneAsync(1);
    }

    public void UnlockMiniView()
    {
        playerCanViewMinis = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && playerCanViewMinis && !germinating) {
            if (isViewingMinis)
            { 
                if (callibrating)
                {
                    orderCamera.SetActive(true);
                }
                minisCamera.SetActive(false);
                computerCamera.SetActive(true);

                switchMinisText.SetActive(true);
                switchPhyllonText.SetActive(false);

                isViewingMinis = false;
            } else
            {
                orderCamera.SetActive(false);
                minisCamera.SetActive(true);
                computerCamera.SetActive(false);

                switchMinisText.SetActive(false);
                switchPhyllonText.SetActive(true);

                isViewingMinis = true;
            }

            if (PlayerStats.CurrentLevel == 0) {
                finishTutorialButton.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void ActivateFirstMiniTutorialCheck()
    {
        creatingFirstTutorialMini = true;
    }

    public void ActivateSecondMiniTutorialCheck()
    {
        creatingSecondTutorialMini = true;
        creatingFirstTutorialMini = false;
        sliderManager.updateIndex = 1;
    }

    public void SelectMini(int miniIndex)
    {
        ClearSelectedMarks();
        sliderManager.updateIndex = miniIndex;
        sliderManager.updateType = "mini";
        if (callibrating)
        {
            sliderManager.DisableSliders();
        }
        sliderManager.SetSliders(growthManager.miniFrequencies[miniIndex]);
    }

    public void SelectFinal(int finalIndex)
    {
        ClearSelectedMarks();
        sliderManager.updateIndex = finalIndex;
        sliderManager.updateType = "final";
        if (callibrating)
        {
            sliderManager.EnableSliders();
        }
        sliderManager.SetSliders(growthManager.finalFrequencies[finalIndex]);
    }

    void ClearSelectedMarks()
    {
        GameObject[] marks = GameObject.FindGameObjectsWithTag("SelectedMark");

        for (int i = 0; i < marks.Length; i++)
        {
            marks[i].SetActive(false);
        }
    }

    public void CalibrateFinals()
    {
        callibrating = true;
        generating = false;
        growthManager.GenerateOrderPlants();

        orderCamera.SetActive(true);

        ClearSelectedMarks();
        SelectFinal(0);

        PlaySelectClip();
    }

    public void PlaySelectClip()
    {
        GetComponent<AudioSource>().PlayOneShot(selectClip);
    }

    public void GerminateFinals()
    {
        germinating = true;
        callibrating = false;
        growthManager.GenerateFinalPlants();
        switchMinisText.SetActive(false);
        switchTextBack.SetActive(false);

        orderCamera.SetActive(false);
        finalCamera.SetActive(true);
        computerCamera.SetActive(false);

        sliderManager.MuteAllSliders();

        PlaySelectClip();

        StartCoroutine(RevealScoresCoroutine(CalculateRoundScore()));
    }

    public void GameOver()
    {
        SceneManager.LoadSceneAsync(1);
    }

    IEnumerator ConveyorBeltCoroutine(bool reverse)
    {
        float duration = 5f;
        Vector3 targetPosition = beltGroup.transform.position + (reverse ? -1 : 1) * beltGroup.transform.right * 30f;
        float timeElapsed = 0f;
        Vector3 startPosition = beltGroup.transform.position;

        while (timeElapsed < duration)
        {
            beltGroup.transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        beltGroup.transform.position = targetPosition;
    }

    IEnumerator RevealScoresCoroutine(float score)
    {
        GetComponent<AudioSource>().PlayOneShot(anticipateClip);
        GetComponent<AudioSource>().PlayOneShot(beltClip);
        StartCoroutine(ConveyorBeltCoroutine(false));
        float duration = 5f;
        Vector3 targetPosition = finalCamera.transform.position + finalCamera.transform.forward * 3f;
        float timeElapsed = 0f;
        Vector3 startPosition = finalCamera.transform.position;

        while (timeElapsed < duration)
        {
            finalCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        finalCamera.transform.position = targetPosition;

        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i< 3; i++)
        {
            scoreCards[i].SetActive(true);
            finalSpotlights[i].SetActive(true);
        }
        GetComponent<AudioSource>().PlayOneShot(spotlightClip);

        yield return new WaitForSeconds(1.5f);

        // Win with B average
        if (score > 300 * .75f)
        {
            WinLight winComponent = winLight.GetComponent<WinLight>();
            winComponent.spotLight.SetActive(true);
            winComponent.pointLight.SetActive(true);
            winMaterial.EnableKeyword("_EMISSION");
            winMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

            StartCoroutine(ConveyorBeltCoroutine(false));

            orderComplete.SetActive(true);

            GetComponent<AudioSource>().PlayOneShot(successClip);

            yield return new WaitForSeconds(4f);

            PlayerStats.CurrentLevel++;
            SceneManager.LoadSceneAsync(PlayerStats.CurrentLevel == 4 ? 0 : 1);
        }
        else
        {
            WinLight loseComponent = loseLight.GetComponent<WinLight>();
            loseComponent.spotLight.SetActive(true);
            loseComponent.pointLight.SetActive(true);
            loseMaterial.EnableKeyword("_EMISSION");
            loseMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

            StartCoroutine(ConveyorBeltCoroutine(true));

            orderFailed.SetActive(true);

            GetComponent<AudioSource>().PlayOneShot(loseClip);

            yield return new WaitForSeconds(4f);

            GameOver();
        }
    }
}

public static class PlayerStats
{
    public static int CurrentLevel { get; set; }
    public static bool IsWinner { get; set; }
}
