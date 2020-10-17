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
        transform.DOLocalMove(endLocalPos, 0.5f).SetRelative();

        StartCoroutine(MoveItemToPlayer(_playerTransform));
    }

    private IEnumerator MoveItemToPlayer(Transform target)
    {
        yield return _waitTime;

        float sign = Random.Range(0, 2) == 0 ? 1 : -1;
        Vector3 p1 = target.position;
        p1.x += (transform.position.x - target.position.x);
        p1.y += sign * Mathf.Abs(transform.position.y - target.position.y) * 1.5f;

        Vector3 start = transform.position;
        
        float timeAgo = 0f;
        while (timeAgo < 1f) {
            Vector3 next = Aroma.CustomMath.Bezier.GetPoint(start, p1, target.position, timeAgo);
            transform.position = next;
            timeAgo += Time.deltaTime * 2f;
            yield return null;
        }

        Destroy(gameObject);
        gameObject.SetActive(false);
    }
}
