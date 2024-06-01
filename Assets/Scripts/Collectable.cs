using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private bool canCollect;
    private CapsuleCollider capsuleCollider;

    private void Start()
    {
        canCollect = true;
        capsuleCollider = GetComponent<CapsuleCollider>();
        capsuleCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.CompareTag("Player") && canCollect)
        {
            Collect();
        }
    }

    private void Collect()
    {
        if (gameObject.tag == "BigItem")
        {
            Debug.Log("Collect Item");
            CollectableManager.instance.bigItemCollected++;
        }
        else if (gameObject.tag == "SmallItem")
        {
            CollectableManager.instance.smallItensCollected++;
        }
        capsuleCollider.enabled = false;
        canCollect = false;
    }
}