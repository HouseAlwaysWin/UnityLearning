using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeDetection : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float radius;
    [SerializeField] private Player player;

    private bool canDetacted;

    private BoxCollider2D boxCd => GetComponent<BoxCollider2D>();

    private void Update()
    {
        if (canDetacted)
        {
            player.ledgeDetected = Physics2D.OverlapCircle(transform.position, radius, whatIsGround);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            canDetacted = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(boxCd.bounds.center, boxCd.size, 0);

        foreach (var hit in colliders)
        {
            if (hit.gameObject.GetComponent<PlatformController>() != null)
            {
                return;
            }
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            canDetacted = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
