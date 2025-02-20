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
        SFXManager.Instance.PlaySFX("coletaItem");
        if (gameObject.tag == "BigItem")
        {
            SFXManager.Instance.PlaySFX("coletaPrincipalWalkieTalkie");
            ObjectiveManager.Instance.bigItemCollected++;
            ObjectiveManager.Instance.QuestPanelMainItemNumber();
        }
        else if (gameObject.tag == "SmallItem")
        {
            ObjectiveManager.Instance.smallItensCollected++;
            ObjectiveManager.Instance.QuestPanelSecondaryItemNumber();
        }
        capsuleCollider.enabled = false;
        canCollect = false;
    }
}