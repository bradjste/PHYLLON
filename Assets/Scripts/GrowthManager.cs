using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*using UnityEditor;*/

public class GrowthManager : MonoBehaviour
{
    public bool autoUpdateTestPlant;

    // Frequencies
    [Range(0.0f, 1.0f)]
    public float alpha = 0f;
    [Range(0.0f, 1.0f)]
    public float beta = 0f;
    [Range(0.0f, 1.0f)]
    public float gamma = 0f;
    [Range(0.0f, 1.0f)]
    public float delta = 0f;
    [Range(0.0f, 1.0f)]
    public float epsilon = 0f;
    [Range(0.0f, 1.0f)]
    public float zeta = 0f;

    public GameObject[] finalSeeds = new GameObject[3];
    public GameObject[] miniSeeds = new GameObject[6];
    public GameObject[] orderSeeds = new GameObject[3];
    public GameObject testSeed;

    public float[][] finalFrequencies = new float[3][];
    public float[][] miniFrequencies = new float[6][];
    public float[][] orderFrequencies = new float[3][];
    public float[] testFrequencies = new float[6];

    // Level 1
    public GameObject[] seeseesPrefabs = new GameObject[3];
    public int seeseesBranchCount = 30;
    public float seeseesMaxBranchLength = 30f;
    public Color seeseesFlowerColorStart;
    public Color seeseesFlowerColorEnd;

    // Level 2
    public GameObject[] juhmbahPrefabs = new GameObject[3];
    public int juhmbahMaxNeedleCount = 300;
    public float juhmbahMaxNeedleLength = 30f;
    public float juhmbahMaxBaseHeight = 30f;
    public Color juhmbahBaseColorStart;
    public Color juhmbahBaseColorEnd;

    // Level 3
    public GameObject[] cusperiusPrefabs = new GameObject[3];
    public int cusperiusMaxShootCount = 10;
    public int cusperiusMaxChainLength = 30;
    public int cusperiusMaxPetalCount = 60;
    public float cusperiusMaxPetalSize = 2f;

    GameManager gameManager;

    private void Start()
    {
        ResetFrequencies();
        GenerateOrders();

        if (PlayerStats.CurrentLevel != 0)
        {
            GenerateOrderPlants();
        }

        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public void ResetFrequencies()
    {
        for (int i = 0; i < 3; i++)
        {
            finalFrequencies[i] = new float[6];
            orderFrequencies[i] = new float[6];
            miniFrequencies[i] = new float[6];
            miniFrequencies[i + 3] = new float[6];
        }
    }

    public void GenerateOrders()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 2 * PlayerStats.CurrentLevel; j++)
            {
                orderFrequencies[i][j] = UnityEngine.Random.Range(0f, 1f);
            }
        }
    }

    public void GenerateOrderPlants()
    {
        for (int i = 0; i < 3; i++)
        {
            GeneratePlant(orderSeeds[i], orderFrequencies[i]);
        }
    }

    public void GenerateTestPlant()
    {
        GeneratePlant(testSeed, testFrequencies);
    }

    public void GenerateFinalPlants()
    {
        for (int i = 0; i < finalSeeds.Length; i++)
        {
            GeneratePlant(finalSeeds[i], finalFrequencies[i]);
        }
    }

    public void GenerateMiniPlants()
    {
        DestroyPlants();
        for (int i = 0; i < miniSeeds.Length; i++)
        {
            GeneratePlant(miniSeeds[i], miniFrequencies[i]);
        }
    }


    public void GeneratePlant(GameObject seed, float[] frequencies)
    {
        GeneratePlant(seed, frequencies, 1f);
    }

    public void GeneratePlant(GameObject seed, float[] frequencies, float scale)
    {
        GameObject plant = new GameObject("PhyloPlant");
        plant.tag = "PhyloPlant";
        plant.transform.SetParent(seed.transform);
        plant.transform.SetPositionAndRotation(seed.transform.position, seed.transform.rotation);
        plant.transform.localScale = new Vector3(scale, scale, scale);

        switch (PlayerStats.CurrentLevel)
        {
            case 0:
            case 1:
                GenerateSeesees(plant, frequencies);
                break;
            case 2:
                GenerateJuhmbah(plant, frequencies);
                break;
            case 3:
                GenerateCusperius(plant, frequencies);
                break;
        }
    }

    public void DestroyPlants()
    {
        GameObject[] plants = GameObject.FindGameObjectsWithTag("PhyloPlant");
        for (var i = 0; i < plants.Length; i++)
        {
            DestroyImmediate(plants[i]);
        }
    }

    void GenerateSeesees(GameObject plant, float[] frequencies)
    {
        float flowerHue = frequencies[0];
        float branchLength = frequencies[1];
        GameObject plantBase = Instantiate(
            seeseesPrefabs[0],
            plant.transform.position,
            plant.transform.rotation,
            plant.transform
        );
        plantBase.name = "Base";

        for (int i = 0; i < seeseesBranchCount; i++)
        {
            GameObject baseEnd = plantBase.GetComponent<PlantPart>().end;

            GameObject branch = Instantiate(
                seeseesPrefabs[1],
                baseEnd.transform.position,
                baseEnd.transform.rotation,
                plant.transform
            );
            branch.name = "Branch";

            branch.transform.RotateAround(baseEnd.transform.position, Vector3.forward, UnityEngine.Random.Range(0f, 60f) + (i * 40));
            branch.transform.RotateAround(baseEnd.transform.position, Vector3.right, UnityEngine.Random.Range(0f, 80f) + (i * 20));
            branch.transform.localScale = new Vector3(1, 1 + (branchLength + UnityEngine.Random.Range(0f, .12f)) * seeseesMaxBranchLength * i / seeseesBranchCount, 1);

            GameObject branchEnd = branch.GetComponent<PlantPart>().end;

            GameObject flower = Instantiate(seeseesPrefabs[2],
                branchEnd.transform.position,
                branchEnd.transform.rotation,
                plant.transform
            );
            flower.name = "Flower";

            GameObject meshGO = flower.GetComponent<PlantPart>().mesh;
            Color flowerColor = Color.Lerp(seeseesFlowerColorStart, seeseesFlowerColorEnd, flowerHue);
            meshGO.GetComponent<Renderer>().material.SetColor("_Color", flowerColor);
        }
    }

    void GenerateJuhmbah(GameObject plant, float[] frequencies)
    {
        int needleCount = 20 + (int)Mathf.Round(frequencies[0] * juhmbahMaxNeedleCount);
        float baseHeight = 0.5f + frequencies[1] * juhmbahMaxBaseHeight;
        float needleSize = 0.1f + frequencies[2] * juhmbahMaxNeedleLength;
        Color baseColor = Color.Lerp(juhmbahBaseColorStart, juhmbahBaseColorEnd, frequencies[3]);

        GameObject plantBase = Instantiate(
            juhmbahPrefabs[0],
            plant.transform.position,
            plant.transform.rotation,
            plant.transform
        );
        plantBase.name = "Base";
        GameObject meshGO = plantBase.GetComponent<PlantPart>().mesh;
        meshGO.GetComponent<Renderer>().material.SetColor("_Color", baseColor);
        plantBase.transform.localScale = new Vector3(1, 1 + baseHeight * juhmbahMaxBaseHeight, 1);

        GameObject baseEnd = plantBase.GetComponent<PlantPart>().end;
        baseEnd.transform.Rotate(Vector3.up, UnityEngine.Random.Range(0f, 360f));
        GameObject flower = Instantiate(
                juhmbahPrefabs[2],
                baseEnd.transform.position,
                baseEnd.transform.rotation,
                plant.transform
            );
        flower.name = "Flower";

        for (int i = 0; i < needleCount; i++)
        {
            float randomPhase = UnityEngine.Random.Range(0f, 360f);
            float radius = meshGO.transform.localScale.x;
            float randomX = radius * Mathf.Sin((float)Math.PI * 2f * randomPhase / 360f);
            float randomZ = radius * Mathf.Cos((float)Math.PI * 2f * randomPhase / 360f);
            GameObject needle = Instantiate(
                juhmbahPrefabs[1],
                plant.transform.position + new Vector3(randomX, UnityEngine.Random.Range(0, baseHeight), randomZ),
                UnityEngine.Random.rotation,
                plant.transform
            );
            needle.name = "Needle";
            needle.transform.localScale = needle.transform.localScale + new Vector3(0, needleSize, 0);
        }
    }

    void GenerateCusperius(GameObject plant, float[] frequencies)
    {
        float lightIntensity = 0.3f + 0.7f * frequencies[0];
        int shootCount = 1 + (int)Mathf.Round(frequencies[1] * cusperiusMaxShootCount);
        float maxOffsetAngle = frequencies[2] * 180f;
        float petalSize = (0.3f + 0.7f * frequencies[3]) * cusperiusMaxPetalSize;
        int chainCount = 5 + (int)Mathf.Round(frequencies[4] * cusperiusMaxChainLength);
        int petalCount = 1 + (int)Mathf.Round(frequencies[5] * cusperiusMaxPetalCount);

        for (int p = 0; p < shootCount; p++)
        {

            GameObject plantBase = Instantiate(
                cusperiusPrefabs[0],
                plant.transform.position + new Vector3(UnityEngine.Random.Range(0f, .5f), 0, UnityEngine.Random.Range(0f, .5f)),
                plant.transform.rotation,
                plant.transform
            );
            plantBase.name = "Base";

            GameObject prevChain = plantBase;
            for (int c = 0; c < chainCount; c++)
            {
                GameObject prevChainEnd = prevChain.GetComponent<PlantPart>().end;
                GameObject chain = Instantiate(
                    cusperiusPrefabs[0],
                    prevChain.GetComponent<PlantPart>().end.transform.position,
                    prevChain.transform.rotation,
                    plant.transform
                );
                chain.name = "Chain";
                chain.transform.RotateAround(prevChainEnd.transform.position, Vector3.forward, UnityEngine.Random.Range(0f, maxOffsetAngle));
                chain.transform.RotateAround(prevChainEnd.transform.position, Vector3.right, UnityEngine.Random.Range(0f, maxOffsetAngle));
                prevChain = chain;
            }

            GameObject flowerBase = Instantiate(
                cusperiusPrefabs[1],
                prevChain.GetComponent<PlantPart>().end.transform.position,
                prevChain.transform.rotation,
                plant.transform
            );
            flowerBase.name = "Flower Base";
            flowerBase.GetComponent<PlantPart>().lightGO.GetComponent<Light>().intensity = lightIntensity;

            for (int i = 0; i < petalCount; i++)
            {
                GameObject flowerBaseEnd = flowerBase.GetComponent<PlantPart>().end;
                float radius = flowerBaseEnd.transform.localPosition.x;
                float nextX = radius * Mathf.Sin((float)Math.PI * 2f * (i * 10) / 360f) - radius;
                float nextZ = radius * Mathf.Cos((float)Math.PI * 2f * (i * 10) / 360f);

                GameObject petal = Instantiate(
                    cusperiusPrefabs[2],
                    flowerBaseEnd.transform.position + new Vector3(nextX, i * -0.04f, nextZ),
                    flowerBaseEnd.transform.rotation,
                    plant.transform
                );
                petal.name = "Petal";
                petal.transform.localScale = new Vector3(1, 1 + petalSize * cusperiusMaxPetalSize * i / petalCount, 1);
            }
        }
    }
}
/*
[CustomEditor(typeof(GrowthManager))]
public class GrowthManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GrowthManager growthManager = (GrowthManager)target;

        if (DrawDefaultInspector())
        {
            growthManager.testFrequencies[0] = growthManager.alpha;
            growthManager.testFrequencies[1] = growthManager.beta;
            growthManager.testFrequencies[2] = growthManager.gamma;
            growthManager.testFrequencies[3] = growthManager.delta;
            growthManager.testFrequencies[4] = growthManager.epsilon;
            growthManager.testFrequencies[5] = growthManager.zeta;

            if (growthManager.autoUpdateTestPlant)
            {
                growthManager.DestroyPlants();
                growthManager.GenerateTestPlant();
            }
        }

        if (GUILayout.Button("Generate Test Plant"))
        {
            growthManager.DestroyPlants();
            growthManager.GenerateTestPlant();
        }

        if (GUILayout.Button("Delete Plants"))
        {
            growthManager.DestroyPlants();
        }
    }
}*/