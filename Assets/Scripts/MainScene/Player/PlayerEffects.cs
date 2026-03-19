using System.Collections;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    public static PlayerEffects Instance;
    private void Awake() => Instance = this;

    // 히트 스탑 (슬로우 모션)
    public IEnumerator DefenseImpactRoutine(float _fDelay, System.Action onComplete)
    {
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(_fDelay);

        Time.timeScale = 1.0f;

        onComplete?.Invoke(); // 멈춘 뒤 밀어내기 실행
    }

    // 잔상 생성
    public void CreateGhost(Animator _animator)
    {
        GameObject ghostParent = new GameObject("DashGhost");
        ghostParent.transform.position = _animator.transform.position;
        ghostParent.transform.localScale = _animator.transform.localScale;

        SpriteRenderer[] arrSpriteRenderer = _animator.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sr in arrSpriteRenderer)
        {
            GameObject objGhostPart = new GameObject(sr.name + "_Ghost");
            objGhostPart.transform.SetParent(ghostParent.transform);
            objGhostPart.transform.localPosition = sr.transform.localPosition;
            objGhostPart.transform.localRotation = sr.transform.localRotation;
            objGhostPart.transform.localScale = sr.transform.localScale;

            SpriteRenderer srGhost = objGhostPart.AddComponent<SpriteRenderer>();
            srGhost.sprite = sr.sprite;
            srGhost.sortingOrder = sr.sortingOrder - 1;
            srGhost.material = sr.material;
            srGhost.color = new Color(1, 1, 1, 0.5f);
        }
        ghostParent.AddComponent<DashEffect>();
    }
}