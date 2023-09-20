using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Should use ICollection, but uses internal list for now
/// </summary>
public class NavCollection<T>
{
    protected List<List<T>> _internalList;

    protected int _height;
    public int height
    {
        get => _height;
        set {

            for (int i = _height; i < value; i++)
                _internalList.Add(Enumerable.Repeat(defaultValue, width).ToList());

            _height = value;
        }
    }

    protected int _width;
    public int width
    {
        get => _width;
        set { 
            _width = value;

            for (int i = 0; i < _height; i++)
                _internalList[i].Resize(_width, defaultValue);
        }
    }

    public T defaultValue;

    public T this[Vector2 input]
    {
        get => this[input.x, input.y];
        set => this[input.x, input.y] = value;
    }

    public T this [float x, float y]
    {
        get => this[Mathf.RoundToInt(x), Mathf.RoundToInt(y)];
        set => this[Mathf.RoundToInt(x), Mathf.RoundToInt(y)] = value;
    }

    public T this [int x, int y]
    {
        get 
        {
            if (x < 0 || y < 0)
                return defaultValue;
            if (y >= _internalList.Count || x >= _internalList[y].Count)
                return defaultValue;

            return _internalList[y][x]; 
        
        }
        set
        {
            if(x >= _width || y >= _height)
            {
                if(x >= _width)
                    width = x + 1;
                if(y >= _height)
                    height = y + 1;
            }
            _internalList[y][x] = value;
        }
    }

    public NavCollection()
    {
        _internalList = new List<List<T>>();
    }

    public NavCollection (int width, int height, T defaultValue)
    {
        Constructor(width, height, defaultValue);
    }

    protected void Constructor(int width, int height, T defaultValue)
    {
        _internalList = new List<List<T>>();

        for(int i = 0; i < height; i++)
            _internalList.Add(Enumerable.Repeat(defaultValue, width).ToList());
    }

    public virtual int distanceValue(Vector2 pos)
    {
        return 1;
    }

    public virtual int distanceValue(int x, int y)
    {
        return 1;
    }
}
