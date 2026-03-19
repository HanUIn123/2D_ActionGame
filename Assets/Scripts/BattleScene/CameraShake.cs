using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Vector3 m_v3OriginPos;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // 카메라의 원래 위치 기억 (흔들고 돌아와야 하니까)
        m_v3OriginPos = transform.localPosition;
    }

    // 외부에서 호출할 함수 (강도와 지속시간을 인자로 )
    public void Shake(float _fDuration, float _fMagnitude)
    {
        StopAllCoroutines(); // 겹치지 않게 이전 쉐이크 중지
        StartCoroutine(ShakeRoutine(_fDuration, _fMagnitude));
    }

    private IEnumerator ShakeRoutine(float _fDuration, float _fMagnitude)
    {
        float fElapsed = 0.0f;

        while (fElapsed < _fDuration)
        {
            // 랜덤한 위치로 카메라 이동
            float fX = Random.Range(-1f, 1f) * _fMagnitude;
            float fY = Random.Range(-1f, 1f) * _fMagnitude;

            transform.localPosition = new Vector3(fX, fY, m_v3OriginPos.z);

            fElapsed += Time.deltaTime;
            yield return null;
        }

        // 쉐이크 끝나면 원래 위치로 복구
        transform.localPosition = m_v3OriginPos;
    }
}
