using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

// https://www.youtube.com/watch?app=desktop&v=JVbr7osMYTo&t=1s
// https://github.com/mixandjam/StarFox-RailMovement/blob/master/Assets/Scripts/PlayerMovement.cs


public class PlayerMovement : MonoBehaviour
{
    private Transform _playerModel;

    [Header("Settings")]
    public bool _joystick = true;

    [Space(1)]
    [Header("Parameters")]
    public float _moveSpeed = 18f;
    public float _lookSpeed = 340;
    public float _forwardSpeed = 6;

    [Space(1)]
    [Header("Public References")]
    public Transform _aimTarget;
    public CinemachineSplineCart _dolly;
    public Transform _cameraParent;

    [Space(1)]
    [Header("Particle FX")]
    public ParticleSystem trail;
    public ParticleSystem circle;
    public ParticleSystem barrel;
    public ParticleSystem stars;

    [Space(1)]
    [Header("State")]
    private bool IsSpinning = false;

    private void Start()
    {
        _playerModel = transform.GetChild(0);
    }

    private void Update()
    {
        float h = _joystick ? Input.GetAxis("Horizontal") : Input.GetAxis("Mouse X");
        float v = _joystick ? Input.GetAxis("Vertical") : Input.GetAxis("Mouse Y");

        LocalMove(h, v, _moveSpeed);
        RotationLook(h, v, _lookSpeed);
        HorizontalLean(_playerModel, h, 80, .1f);
        SetSpeed(_forwardSpeed);

        /*
        if (Input.GetButtonDown("Action"))
            Boost(true);

        if (Input.GetButtonUp("Action"))
            Boost(false);

        if (Input.GetButtonDown("Fire3"))
            Break(true);

        if (Input.GetButtonUp("Fire3"))
            Break(false);
        */

        if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2"))
        {
            ScreenLogManager.Instance.LogMessage($"Player did a barrel roll");
            int dir = Input.GetButtonDown("Fire1") ? -1 : 1;
            QuickSpin(dir);
        }
    }

    private void LocalMove(float x, float y, float speed)
    {
        transform.localPosition += new Vector3(x, y, 0) * speed * Time.deltaTime;
        ClampPosition();
    }

    private void ClampPosition()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }

    private void RotationLook(float h, float v, float speed)
    {
        _aimTarget.localPosition = new Vector3(h, v, 1);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(_aimTarget.position), Mathf.Deg2Rad * speed);
    }

    private void HorizontalLean(Transform target, float axis, float leanLimit, float lerpTime)
    {
        Vector3 targetEulerAngles = target.localEulerAngles;
        target.localEulerAngles = new Vector3(targetEulerAngles.x, targetEulerAngles.y, Mathf.LerpAngle(targetEulerAngles.z, -axis * leanLimit, lerpTime));
    }

    private void SetSpeed(float x)
    {
        _dolly.SplineSettings.Position += Time.deltaTime * x;
    }

    public void QuickSpin(int dir)
    {
        if (!IsSpinning)
        {
            StartCoroutine(QuickSpinCoroutine(dir));
            barrel.Play();
        }
    }

    private IEnumerator QuickSpinCoroutine(int dir)
    {
        IsSpinning = true;
        float duration = 0.4f;
        float elapsedTime = 0f;
        float startRotation = _playerModel.localEulerAngles.z;
        float endRotation = startRotation + 360 * -dir;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float currentRotation = Mathf.Lerp(startRotation, endRotation, Mathf.SmoothStep(0, 1, t));
            _playerModel.localEulerAngles = new Vector3(_playerModel.localEulerAngles.x, _playerModel.localEulerAngles.y, currentRotation);
            yield return null;
        }

        _playerModel.localEulerAngles = new Vector3(_playerModel.localEulerAngles.x, _playerModel.localEulerAngles.y, endRotation);
        IsSpinning = false;
    }


    /*
    void SetCameraZoom(float zoom, float duration)
    {
        _cameraParent.DOLocalMove(new Vector3(0, 0, zoom), duration);
    }

    void DistortionAmount(float x)
    {
        Camera.main.GetComponent<PostProcessVolume>().profile.GetSetting<LensDistortion>().intensity.value = x;
    }

    void FieldOfView(float fov)
    {
        cameraParent.GetComponentInChildren<CinemachineVirtualCamera>().m_Lens.FieldOfView = fov;
    }

    void Chromatic(float x)
    {
        Camera.main.GetComponent<PostProcessVolume>().profile.GetSetting<ChromaticAberration>().intensity.value = x;
    }

    void Boost(bool state)
    {

        if (state)
        {
            cameraParent.GetComponentInChildren<CinemachineImpulseSource>().GenerateImpulse();
            trail.Play();
            circle.Play();
        }
        else
        {
            trail.Stop();
            circle.Stop();
        }
        trail.GetComponent<TrailRenderer>().emitting = state;

        float origFov = state ? 40 : 55;
        float endFov = state ? 55 : 40;
        float origChrom = state ? 0 : 1;
        float endChrom = state ? 1 : 0;
        float origDistortion = state ? 0 : -30;
        float endDistorton = state ? -30 : 0;
        float starsVel = state ? -20 : -1;
        float speed = state ? forwardSpeed * 2 : forwardSpeed;
        float zoom = state ? -7 : 0;

        DOVirtual.Float(origChrom, endChrom, .5f, Chromatic);
        DOVirtual.Float(origFov, endFov, .5f, FieldOfView);
        DOVirtual.Float(origDistortion, endDistorton, .5f, DistortionAmount);
        var pvel = stars.velocityOverLifetime;
        pvel.z = starsVel;

        DOVirtual.Float(dolly.m_Speed, speed, .15f, SetSpeed);
        SetCameraZoom(zoom, .4f);
    }

    void Break(bool state)
    {
        float speed = state ? forwardSpeed / 3 : forwardSpeed;
        float zoom = state ? 3 : 0;

        DOVirtual.Float(dolly.m_Speed, speed, .15f, SetSpeed);
        SetCameraZoom(zoom, .4f);
    }
    */

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_aimTarget.position, .5f);
        Gizmos.DrawSphere(_aimTarget.position, .15f);
    }
}
