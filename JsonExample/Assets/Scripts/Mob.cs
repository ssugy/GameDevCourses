using System;
using System.Collections;
using UnityEngine;
using SimpleJSON;
using System.Collections.Generic;

[Serializable]
public class Mob
{
    [SerializeField] int index;
    [SerializeField] string name;

    public int INDEX
    {
        get { return index; }
        set { index = value; }
    }
    public string NAME
    {
        get { return name; }
        set { name = value; }
    }
    public Mob()
    {

    }
    public Mob(int _index, string _name)
    {
        index = _index;
        name = _name;
    }
}

public class Serialization<T>
{
    [SerializeField] List<T> _t;

    public List<T> ToList() { return _t; }  // get _t같은 용도의 함수
    public Serialization(List<T> _tmp)
    {
        _t = _tmp;
    }
}
