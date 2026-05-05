using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] VirtualJoystick joystick;

    float _speed = 5f;
    bool  _speedBoosted;
    float _boostEndTime;
    const float BoostMultiplier = 1.6f;

    Rigidbody2D _rb;

    void Awake() => _rb = GetComponent<Rigidbody2D>();

    void Start()
    {
        if (joystick == null)
            joystick = FindObjectOfType<VirtualJoystick>();
    }

    void Update()
    {
        if (!_speedBoosted && RemoteConfigManager.Instance != null && RemoteConfigManager.Instance.IsReady)
            _speed = RemoteConfigManager.Instance.PlayerSpeed;

        if (_speedBoosted && Time.time >= _boostEndTime)
        {
            _speedBoosted = false;
            _speed = RemoteConfigManager.Instance != null ? RemoteConfigManager.Instance.PlayerSpeed : 5f;
        }
    }

    void FixedUpdate()
    {
        Vector2 dir = joystick != null ? joystick.Direction : Vector2.zero;
        if (dir.sqrMagnitude < 0.01f)
        {
            dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            dir = Vector2.ClampMagnitude(dir, 1f);
        }

        _rb.velocity = dir * _speed;

        if (dir != Vector2.zero)
            transform.up = dir;
    }

    public void ApplySpeedBoost(float duration)
    {
        _speedBoosted = true;
        _boostEndTime = Time.time + duration;
        _speed = (RemoteConfigManager.Instance != null ? RemoteConfigManager.Instance.PlayerSpeed : 5f) * BoostMultiplier;
    }
}
