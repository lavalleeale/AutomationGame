using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public WorldGenerationController worldGenController;
    // Start is called before the first frame update
    void Start()
    {
        worldGenController.Generate();
    }
}
