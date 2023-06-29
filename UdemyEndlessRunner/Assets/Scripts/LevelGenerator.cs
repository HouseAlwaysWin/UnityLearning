using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Transform[] levelPart;
    [SerializeField] private Vector3 nextPartPosition;
    [SerializeField] private float distanceToSpawn;
    [SerializeField] private float distanceToDelete;
    [SerializeField] private Transform Player;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DeletePlatform();
        GeneratePlatform();
    }

    private void DeletePlatform()
    {
        if (transform.childCount > 0)
        {
            Transform partToDelete = transform.GetChild(0);
            if (Vector2.Distance(Player.transform.position, partToDelete.transform.position) > distanceToDelete)
            {
                Destroy(partToDelete.gameObject);
            }
        }
    }

    private void GeneratePlatform()
    {
        while (Vector2.Distance(Player.transform.position, nextPartPosition) < distanceToSpawn)
        {
            Transform part = levelPart[Random.Range(0, levelPart.Length)];

            Vector2 newPosition = new Vector2(nextPartPosition.x - part.Find("StartPoint").position.x, 0);

            Transform newPart = Instantiate(part, newPosition, transform.rotation, transform);
            nextPartPosition = newPart.Find("EndPoint").position;
        }
    }
}
