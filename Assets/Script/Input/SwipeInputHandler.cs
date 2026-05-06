using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeInputHandler : MonoBehaviour
{
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    private const float MinSwipeDistance = 50f;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            endTouchPosition = Input.mousePosition;
            HandleSwipe();
        }
    }
    private void HandleSwipe()
    {
        Vector2 swipeVector = endTouchPosition - startTouchPosition;
        if (swipeVector.magnitude < MinSwipeDistance)
            return;
        {
            MoveDirection dir = MoveDirection.None;

            if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
                dir = swipeVector.x > 0 ? MoveDirection.Right : MoveDirection.Left;
            else
                dir = swipeVector.y > 0 ? MoveDirection.Up : MoveDirection.Down;

            EventManager.Raise(new SwipeMoveEvent { Direction = dir });
        }
    }


}
