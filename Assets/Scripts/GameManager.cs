using UnityEngine.InputSystem;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Camera m_Camera;
    [SerializeField] GameObject m_Background;
    [SerializeField] Wind m_WindPrefab;

    [SerializeField] float m_ScrollingSpeed;

    private Vector3 _startWind;
    private Vector3 _endWind;

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
            _startWind = GetMousePosition();
        if (Mouse.current.leftButton.wasReleasedThisFrame){
            _endWind = GetMousePosition();
            Wind w = Instantiate(m_WindPrefab);
            w.Init(_startWind, _endWind);
        }

        m_Camera.transform.position += m_ScrollingSpeed * Time.deltaTime * Vector3.up;
    }

    Vector3 GetMousePosition()
    {
        Vector2 mousePos = Mouse.current.position.value;
        Vector3 startPos = new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane);

        //Get ray from mouse postion
        Ray rayCast = Camera.main.ScreenPointToRay(startPos);

        //Raycast and check if any object is hit
        Physics.Raycast(rayCast, out RaycastHit hit, Camera.main.farClipPlane);
        Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);
        return new Vector3(hit.point.x, hit.point.y, 0);
    }
}