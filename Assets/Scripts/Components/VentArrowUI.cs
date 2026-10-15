using UnityEngine;
using UnityEngine.UI;

public class VentArrowUI : MonoBehaviour
{
    [Header("")]
    [SerializeField] private Button _button;
    [SerializeField] private Image _arrowImage;

    private Vent _sourceVent, _destinationVent;


    private void Awake()
    {
        if (_button == null)
            _button = GetComponent<Button>();

        if (_arrowImage == null)
            _arrowImage = GetComponent<Image>();
    }

    public void Setup(
        Vent sourceVent,
        Vent destinationVent,
        float spacingOffset = 0f)
    {
        _sourceVent = sourceVent;
        _destinationVent = destinationVent;

        Vector3 worldDirection = 
            _destinationVent.transform.position - 
            _sourceVent.transform.position;
        if (worldDirection.sqrMagnitude > .001f)
        {
            Vector3 localDirection = 
                _arrowImage.rectTransform
                .parent
                .InverseTransformDirection(
                    worldDirection.normalized
                );
            localDirection.z = 0f;
            localDirection.Normalize();

            Vector2 direction =
                (Vector2)localDirection.normalized;
            Vector2 perpendicular = 
                new(-direction.y, direction.x);
            Vector2 arrowPosition =
                direction * 55f + 
                perpendicular * spacingOffset;
            _arrowImage.rectTransform.anchoredPosition = 
                arrowPosition;
            float angle = 
                Mathf.Atan2(localDirection.y, localDirection.x) 
                * Mathf.Rad2Deg;
            _arrowImage.rectTransform.localRotation =
                Quaternion.Euler(0f, 0f, angle - 90f);
        }

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnArrowClicked);
    }

    private void OnArrowClicked()
    {
        if (_sourceVent == null || _destinationVent == null)
            return;

        _sourceVent.TravelToVent(_destinationVent);
    }
}
