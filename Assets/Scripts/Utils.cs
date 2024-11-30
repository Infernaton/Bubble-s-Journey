using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System;
using System.Security.Cryptography;

namespace Utils
{
    public class Anim
    {
        private static IEnumerator AnimationOnCurve(float time, Action<float> animation, AnimationCurve curve)
        {
            float currentTime = 0f;

            while (currentTime < time)
            {
                animation(curve.Evaluate(currentTime / time));
                currentTime += Time.deltaTime;
                yield return null;
            }

        }
        #region FadeIn/FadeOut
        #region TMP_Text
        public static IEnumerator FadeIn(float t, TMP_Text txt)
        {
            txt.enabled = true;
            txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, 0);
            while (txt.color.a < 1.0f)
            {
                txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, txt.color.a + (Time.deltaTime / t));
                yield return null;
            }
        }

        public static IEnumerator FadeOut(float t, TMP_Text txt)
        {
            txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, 1);
            while (txt.color.a > 0.0f)
            {
                txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, txt.color.a - (Time.deltaTime / t));
                yield return null;
            }
            txt.enabled = false;
        }
        #endregion

        #region CanvaGroup
        public static IEnumerator FadeIn(float t, CanvasGroup c)
        {
            c.gameObject.SetActive(true);
            c.alpha = 0f;
            while (c.alpha < 1.0f)
            {
                c.alpha += Time.deltaTime / t;
                yield return null;
            }
        }

        public static IEnumerator FadeOut(float t, CanvasGroup c)
        {
            c.alpha = 1f;
            while (c.alpha > 0.0f)
            {
                c.alpha -= Time.deltaTime / t;
                yield return null;
            }
            c.gameObject.SetActive(false);
        }
        #endregion

        #region RawImage
        public static IEnumerator FadeIn(float t, RawImage i)
        {
            i.gameObject.SetActive(true);
            i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
            while (i.color.a < 1.0f)
            {
                i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
                yield return null;
            }
        }

        public static IEnumerator FadeOut(float t, RawImage i)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
            while (i.color.a > 0.0f)
            {
                i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
                yield return null;
            }
            i.gameObject.SetActive(false);
        }
        #endregion

        #region Image
        public static IEnumerator FadeIn(float t, Image i)
        {
            i.gameObject.SetActive(true);
            i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
            while (i.color.a < 1.0f)
            {
                i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
                yield return null;
            }
        }

        public static IEnumerator FadeOut(float t, Image i)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
            while (i.color.a > 0.0f)
            {
                i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
                yield return null;
            }
            i.gameObject.SetActive(false);
        }
        #endregion
        #endregion

        public static IEnumerator Blink(GameObject obj, float time)
        {
            Renderer[] objectRenderers = obj.GetComponentsInChildren<Renderer>();
            bool switchAnim = true;
            float endTime = Time.time + time;
            while (endTime > Time.time)
            {
                switchAnim = !switchAnim;
                BlinkAnim(objectRenderers, switchAnim);
                yield return new WaitForSeconds(0.05f);
            }
            //To make sure the gameobject stay visible at the end of the animation
            BlinkAnim(objectRenderers, true);
        }

        private static void BlinkAnim(Renderer[] objectRenderers, bool hasToRender)
        {
            foreach (Renderer r in objectRenderers)
                r.enabled = hasToRender;
        }

        #region SlideIn/SlideOut
        public static IEnumerator SlideIn(float t, CanvasGroup i)
        {
            i.gameObject.SetActive(true);
            yield return Slide(t, i.gameObject, 1f);
        }
        public static IEnumerator SlideOut(float t, CanvasGroup i)
        {
            yield return Slide(t, i.gameObject, 2f);
            i.gameObject.SetActive(false);
        }
        private static IEnumerator Slide(float t, GameObject o, float targetScale)
        {
            Vector3 from = o.transform.localScale;
            Vector3 to = Vector3.one * targetScale;
            yield return AnimationOnCurve(t, t => o.transform.localScale = Vector2.Lerp(from, to, t), AnimationCurve.Linear(0,0,1,1));
        }
        #endregion

        #region MovesTo
        public static IEnumerator MoveUI(RectTransform o, Vector2 to, float time, AnimationCurve curve)
        {
            Vector2 from = o.anchoredPosition;
            yield return AnimationOnCurve(time, t => o.anchoredPosition = Vector2.Lerp(from, to, t), curve);
        }

        public static IEnumerator MoveObject(Transform o, Vector3 to, float time, AnimationCurve curve)
        {
            Vector3 from = o.position;
            yield return AnimationOnCurve(time, t => o.position = Vector3.Lerp(from, to, t), curve);
        }
        #endregion
    }
}