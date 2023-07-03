using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        SceneManager.LoadScene(0);
    }

}
