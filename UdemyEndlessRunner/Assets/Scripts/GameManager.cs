using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool colorEntierPlatform;
    public Color platformColor;
    public int coins;

    private void Awake()
    {
        instance = this;
    }

}
