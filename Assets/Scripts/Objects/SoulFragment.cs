using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SoulFragment : MonoBehaviour
{
    private Transform _playerTransform;
    private WaitForSeconds _waitTime = new WaitForSeconds(0.5f);

    public void Spawn(Vector3 startPos, Vector3 endLocalPos, Transform playerTransform)
    {
        _playerTransform = playerTransform;
        transform.position = startPos;
        transform.DOLocalMove(endLocalPos, 0.5f);

        StartCoroutine(MoveItemToPlayer(_playerTransform));
    }

    private IEnumerator MoveItemToPlayer(Transform target)
    {
        yield return _waitTime;

        float accelFactor = 1f;
        float speed = 3f;

        while (!IsMoveEnd())
        {
            Vector3 dir = (target.position - transform.position).normalized;
            transform.position += dir * speed * Time.deltaTime;
            speed += accelFactor;

            yield return null;
        }

        Destroy(gameObject);
        gameObject.SetActive(false);
    }

    private bool IsMoveEnd() {
        Vector3 size = transform.localScale;
        Vector3 offset = transform.localScale;
        offset.y *= 0.5f;

        var collider = Physics2D.OverlapBox(transform.position + offset, size, 0f, 1 << LayerMask.NameToLayer("Player"));
        return collider != null;
    }
}
