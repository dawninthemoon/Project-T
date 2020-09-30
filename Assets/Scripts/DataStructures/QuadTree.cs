using System.Collections.Generic;
using UnityEngine;

//Any object that you insert into the tree must implement this interface
public interface IQuadTreeObject {
	Rect GetBounds();
	void Progress();
	void FixedProgress();
}
public class QuadTree<T> where T : IQuadTreeObject {
	private int MAX_OBJECTS = 10;
	private int MAX_LEVELS = 5;

	private int _level;
	private List<T> _objects;
	private Rect _bounds;
	private QuadTree<T>[] _nodes;

	public QuadTree(int maxSize, Rect bounds) {
		_level = 0;
		_objects = new List<T>();
		_bounds = bounds;
		_nodes = new QuadTree<T>[4];
	}

	public void Clear() {
		_objects.Clear();

		for (int i = 0; i < _nodes.Length; i++) {
			if (_nodes[i] != null) {
				_nodes[i].Clear();
				_nodes[i] = null;
			}
		}
	}

	private void Split() {
		int subWidth = (int)(_bounds.width / 2);
		int subHeight = (int)(_bounds.height / 2);
		int x = (int)_bounds.x;
		int y = (int)_bounds.y;

		_nodes[0] = new QuadTree<T>(_level + 1, new Rect(x + subWidth, y, subWidth, subHeight));
		_nodes[1] = new QuadTree<T>(_level + 1, new Rect(x, y, subWidth, subHeight));
		_nodes[2] = new QuadTree<T>(_level + 1, new Rect(x, y + subHeight, subWidth, subHeight));
		_nodes[3] = new QuadTree<T>(_level + 1, new Rect(x + subWidth, y + subHeight, subWidth, subHeight));
	}

	private int GetIndex(Rect rect) {
		int index = -1;
		float verticalMidpoint = _bounds.x + (_bounds.width / 2f);
		float horizontalMidpoint = _bounds.y + (_bounds.height / 2f);

		bool topQuadrant = (rect.y < horizontalMidpoint && rect.y + rect.height < horizontalMidpoint);
		bool bottomQuadrant = (rect.y > horizontalMidpoint);

		if (rect.x < verticalMidpoint && rect.x + rect.width < verticalMidpoint)
		{
			if (topQuadrant)
				index = 1;
			else if (bottomQuadrant)
				index = 2;
		}
		else if (rect.x > verticalMidpoint)
		{
			if (topQuadrant)
				index = 0;
			else if (bottomQuadrant)
				index = 3;
		}

		return index;
	}

	public void Insert(T obj) {
		Rect bounds = obj.GetBounds();
		if (_nodes[0] != null) {
			int index = GetIndex(bounds);

			if (index != -1) {
				_nodes[index].Insert(obj);
				return;
			}
		}

		_objects.Add(obj);

		if (_objects.Count > MAX_OBJECTS && _level < MAX_LEVELS) {
			if (_nodes[0] == null) {
				Split();
			}

			int i = 0;
			while (i < _objects.Count) {
				int index = GetIndex(_objects[i].GetBounds());
				if (index != -1) {
					var temp = _objects[i];
					_objects.RemoveAt(i);
					_nodes[index].Insert(temp);
				}
				else {
					++i;
				}
			}
		}
	}

	public void Retrieve(Rect rect, List<T> returnObjects) {
		int index = GetIndex(rect);
		if (index != -1 && _nodes[0] != null) {
			_nodes[index].Retrieve(rect, returnObjects);
		}

		returnObjects.AddRange(_objects);
	}
}