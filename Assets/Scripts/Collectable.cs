using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private bool canCollect;

    private void Update()
    {
        if (canCollect)
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
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            canCollect = true;

        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            canCollect = false;
        }
    }
}