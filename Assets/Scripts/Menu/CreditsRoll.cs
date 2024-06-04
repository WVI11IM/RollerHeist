using UnityEngine;
using TMPro;

public class CreditsRoll : MonoBehaviour
{
    public TMP_Text creditsText;
    public float scrollSpeed = 20f;

    private RectTransform creditsRectTransform;
    private RectTransform panelRectTransform;

    private void Start()
    {
        creditsRectTransform = creditsText.GetComponent<RectTransform>();
        panelRectTransform = creditsText.transform.parent.GetComponent<RectTransform>();

        // Posicionar o texto fora da área visível no início
        creditsRectTransform.anchoredPosition = new Vector2(0, panelRectTransform.rect.height / 2);
    }

    private void Update()
    {
        // Move o texto para cima ao longo do tempo
        creditsRectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        // Reiniciar a posição se tiver rolado completamente fora da tela
       if (creditsRectTransform.anchoredPosition.y > creditsRectTransform.rect.height)
       {
       creditsRectTransform.anchoredPosition = new Vector2(0, panelRectTransform.rect.height / 2);
       }

    }
}

