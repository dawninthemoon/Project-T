using System.Collections;
using UnityEngine;
using Cinemachine;

[SaveDuringPlay]
public class CameraShake : MonoBehaviour
{
    [SerializeField] private AnimationCurve _shakeCurve = null;

    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    private CinemachineBasicMultiChannelPerlin _virtualCameraNoise;

    void Start()
    {
        _virtualCameraNoise = _virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }

    public void StartShakeCamera(float duration) {
        StartCoroutine(ShakeCamera(duration));
    }

    private IEnumerator ShakeCamera(float duration) {
        float timeAgo = 0f;

        while (timeAgo <= 1f) {
            float amount = _shakeCurve.Evaluate(timeAgo);
            _virtualCameraNoise.m_AmplitudeGain = amount * 3f;

            timeAgo += Time.deltaTime / duration;

            yield return null;
        }

        _virtualCameraNoise.m_AmplitudeGain = 0f;
    }
}