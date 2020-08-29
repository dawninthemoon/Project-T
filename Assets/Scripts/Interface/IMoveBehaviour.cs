using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveBehaviour
{
    void ExecuteMove(Transform transform, params string[] parameters);
}
