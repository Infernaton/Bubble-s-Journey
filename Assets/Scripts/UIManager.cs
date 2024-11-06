using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private TMP_Text m_BubbleCounter;
    [SerializeField] private CanvasGroup m_CinematicViewUI;

    [Header("ClickAnimation")]
    [SerializeField] private Texture m_CursorIcon;
    [SerializeField] private Texture m_CursorClickIcon;
    [SerializeField] private RawImage m_RawImageObject;
    [SerializeField] private Vector2 m_StartPosAnimation;
    [SerializeField] private Vector2 m_EndPosAnimation;
    [SerializeField] private float m_SpeedAnimation;

    public CanvasGroup CinematicView => m_CinematicViewUI;

    public static UIManager Instance = null;
    private void Awake()
    {
        if (Instance == null) // If there is no instance already
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    public void UpdateBubbleCounter(float nb)
    {
        m_BubbleCounter.text = "Bulles : " + nb;
    }

    [ContextMenu("PlayClickAnimation")]
    public void PlayClickAnimation()
    {
        m_RawImageObject.gameObject.SetActive(true);
        StartCoroutine(ClickAnimation());
    }
    public IEnumerator ClickAnimation()
    {
        m_RawImageObject.texture = m_CursorIcon;
        yield return new WaitForSeconds(1f);
        m_RawImageObject.texture = m_CursorClickIcon;
        yield return new WaitForSeconds(0.1f);
        m_RawImageObject.texture = m_CursorIcon;
        yield return new WaitForSeconds(1f);
        m_RawImageObject.gameObject.SetActive(false);
    }
}
