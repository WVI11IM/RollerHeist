using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    private RectTransform rectTransform;
    public float scaleMultiplier = 1.1f;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Aumentar a escala ao passar o mouse por cima
        //rectTransform.localScale = originalScale * scaleMultiplier;
        animator.SetBool("taDentro", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Reverter para o tamanho original ao retirar o mouse
        //rectTransform.localScale = originalScale;
        animator.SetBool("taDentro", false);

    }
}
