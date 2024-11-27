using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropPositionLock : MonoBehaviour
{
    Rigidbody rb;

    public GameObject[] propsToUnlock;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && rb.constraints == RigidbodyConstraints.FreezeAll)
        {
            rb.constraints = RigidbodyConstraints.None;
            for(int i = 0; i < propsToUnlock.Length; i++)
            {
                propsToUnlock[i].GetComponent<PropPositionLock>().LockRigidbodyConstraints(false);
            }
        }
    }

    public void LockRigidbodyConstraints(bool isLocked)
    {
        if(isLocked) rb.constraints = RigidbodyConstraints.FreezeAll;
        else rb.constraints = RigidbodyConstraints.None;
    }
}
