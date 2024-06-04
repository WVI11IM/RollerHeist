using UnityEngine;
using UnityEngine.UI;

public class CreditsRoll : MonoBehaviour
{
    public Image creditsImage; // Substitui TMP_Text por Image
    public float scrollSpeed = 10f; // Ajusta a velocidade para um valor mais baixo

    private RectTransform creditsRectTransform;
    private RectTransform panelRectTransform;

    private void Start()
    {
        creditsRectTransform = creditsImage.GetComponent<RectTransform>();
        panelRectTransform = creditsImage.transform.parent.GetComponent<RectTransform>();

        // Posicionar a imagem fora da área visível no início
        creditsRectTransform.anchoredPosition = new Vector2(0, -panelRectTransform.rect.height / 2 - creditsRectTransform.rect.height / 2);
    }

    private void Update()
    {
        // Move a imagem para cima ao longo do tempo
        creditsRectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        // Reiniciar a posição se tiver rolado completamente fora da tela
        if (creditsRectTransform.anchoredPosition.y > panelRectTransform.rect.height / 2 + creditsRectTransform.rect.height / 2)
        {
            creditsRectTransform.anchoredPosition = new Vector2(0, -panelRectTransform.rect.height / 2 - creditsRectTransform.rect.height / 2);
        }
    }
}
