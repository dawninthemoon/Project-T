﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    public delegate T ObjectCreateDelegate();

    private ObjectCreateDelegate _createObjectCallback;

    public int _size;
 
    private List<T> _freeList;
    private List<T> _usedList;

    public ObjectPool(int initSize, ObjectCreateDelegate createObjCallback) {
        _size = initSize;
        _createObjectCallback = createObjCallback;
        Initalize();
    }

    private T CreateObject() {
        var pooledObject = _createObjectCallback.Invoke();
        pooledObject.gameObject.SetActive(false);
        return pooledObject;
    }

    private void Initalize() {
        _freeList = new List<T>(_size);
        _usedList = new List<T>(_size);
 
        // Instantiate the pooled objects and disable them.
        for (var i = 0; i < _size; i++)
        {
            _freeList.Add(CreateObject());
        }
    }

    public T GetObject() {
        if (_freeList.Count == 0)
            _freeList.Add(CreateObject());
        
        var pooledObject = _freeList[_freeList.Count - 1];
        _freeList.RemoveAt(_freeList.Count - 1);
        _usedList.Add(pooledObject);

        pooledObject.gameObject.SetActive(true);
        return pooledObject;
    }
 
    public void ReturnObject(T pooledObject) {
        Debug.Assert(_usedList.Contains(pooledObject));
    
        _usedList.Remove(pooledObject);
        _freeList.Add(pooledObject);
    
        var pooledObjectTransform = pooledObject.transform;
        pooledObjectTransform.localPosition = Vector3.zero;
        pooledObject.gameObject.SetActive(false);
    }
}
