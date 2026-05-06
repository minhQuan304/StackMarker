using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveDirection { None, Up, Down, Left, Right }
public struct SwipeMoveEvent : IEvent
{
    public MoveDirection Direction;
}

public struct QiaoPlayerCollisionEvent : IEvent
{
    public IObject Qiao;
    public Collision Collision;
}

public struct Zhuanjiao1ColisionEvent : IEvent
{
    public float RotationY; 
}

public struct FinishEvent : IEvent
{
    public Vector3 FinishPosition;
}

public struct LevelWinEvent : IEvent
{
    public int StackCount;
}

public struct LevelLoseEvent : IEvent { }

public struct LevelStartEvent : IEvent { }


public struct NextLevelEvent : IEvent { }

public struct RestartLevelEvent : IEvent { }







