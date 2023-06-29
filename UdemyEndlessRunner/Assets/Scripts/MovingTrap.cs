using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTrap : Trap
{
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Transform[] movePoint;

    private int pointIndex;

    void Start()
    {
        transform.position = movePoint[0].position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint[pointIndex].position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, movePoint[pointIndex].position) < .25f)
        {
            pointIndex++;
            if (pointIndex >= movePoint.Length)
            {
                pointIndex = 0;
            }
        }

        if (transform.position.x > movePoint[pointIndex].position.x)
        {
            transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));
        }
        else
        {
            transform.Rotate(new Vector3(0, 0, -rotationSpeed * Time.deltaTime));
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
}
