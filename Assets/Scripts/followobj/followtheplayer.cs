using UnityEngine;

public class SmoothObjectFollow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playerTransform;

    [Header("Flip Rotation Stats")]
    [SerializeField] private float _flipYRotationTime = 0.5f;
    private Coroutine _turnCoroutine;
    private playermovement _player;
    private bool _isFacingRight;

    private void Start()
{
    // Ensure the camera starts with the correct rotation
    _isFacingRight = _player.IsFacingRight;
    transform.rotation = Quaternion.Euler(0f, _isFacingRight ? 0f : 180f, 0f);
}
    private void Awake()
    {
        _player = _playerTransform.gameObject.GetComponent<playermovement>();
        _isFacingRight = _player.IsFacingRight;
    }

    private void Update()
    {
        // Make the cameraFollowObject follow the player's position
        transform.position = _playerTransform.position;
    }

    public void CallTurn()
    {
        if (_turnCoroutine != null)
        {
            StopCoroutine(_turnCoroutine); // Cancel ongoing transition
        }
        _turnCoroutine = StartCoroutine(FlipYLerp());
    }

    private System.Collections.IEnumerator FlipYLerp()
    {
        float startRotation = transform.localEulerAngles.y;
        float endRotationAmount = DetermineEndRotation();
        float elapsedTime = 0f;

        while (elapsedTime < _flipYRotationTime)
        {
            elapsedTime += Time.deltaTime;
            float YRotation = Mathf.Lerp(startRotation, endRotationAmount, elapsedTime / _flipYRotationTime);
            transform.rotation = Quaternion.Euler(0f, YRotation, 0f);
            yield return null;
        }
    }

    private float DetermineEndRotation()
    {
        _isFacingRight = !_isFacingRight;
        return _isFacingRight ? 0f : 180f;
    }
}