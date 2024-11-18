using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private TMP_Text m_BubbleCounter;
    [SerializeField] private TMP_Text m_HighScoreCounter;
    [SerializeField] private TMP_Text m_TimeCounter;
    [SerializeField] private CanvasGroup m_CinematicViewUI;

    [Header("ClickAnimation")]
    [SerializeField] private Texture m_CursorIcon;
    [SerializeField] private Texture m_CursorClickIcon;
    [SerializeField] private RawImage m_RawImageObject;
    [SerializeField] private Vector2 m_StartPosAnimation;
    [SerializeField] private Vector2 m_EndPosAnimation;
    [SerializeField] private float m_AnimationTime;
    [SerializeField] private AnimationCurve m_MovementCurve;

    public CanvasGroup CinematicView => m_CinematicViewUI;

    public static UIManager Instance = null;
    private void Awake()
    {
        if (Instance == null) // If there is no instance already
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    public void UpdateHighScoreCounter(float nb)
    {
        m_HighScoreCounter.text = "Hauteur Max : " + nb + "m";
    }
    public void UpdateBubbleCounter(float nb)
    {
        m_BubbleCounter.text = "Essais : " + nb;
    }

    public void UpdateTimeCounter(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        m_TimeCounter.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    [ContextMenu("StopAnimation")]
    public void StopAnimation()
    {
        StopAllCoroutines();
    }

    [ContextMenu("PlayClickAnimation")]
    public void PlayClickAnimation()
    {
        StartCoroutine(ClickAnimation());
    }

    public IEnumerator ClickAnimation()
    {
        float fadeTime = 0.1f;
        float idleTime = 0.3f;
        //Init
        m_RawImageObject.rectTransform.anchoredPosition = m_StartPosAnimation;
        m_RawImageObject.texture = m_CursorIcon;
        
        //Start
        yield return Utils.Anim.FadeIn(fadeTime, m_RawImageObject);
        yield return new WaitForSeconds(idleTime);

        //Hold Click
        m_RawImageObject.texture = m_CursorClickIcon;

        //Moves to destination
        yield return Utils.Anim.MoveUI(m_RawImageObject.rectTransform, m_EndPosAnimation, m_AnimationTime - 2*idleTime - 2*fadeTime, m_MovementCurve);

        //Release Click
        m_RawImageObject.texture = m_CursorIcon;

        //End
        yield return new WaitForSeconds(idleTime);
        yield return Utils.Anim.FadeOut(fadeTime, m_RawImageObject);
    }
}
