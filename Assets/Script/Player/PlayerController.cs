using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 15f;

    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float offset;

    public Transform collectionBricks;
    public GameObject footBrickPrefab;
    public Transform visualGroup;

    private bool isMoving = false;
    private bool isFinished = false;
    private Vector3 targetPos;
    private Rigidbody rb;
    private MoveDirection currentDir = MoveDirection.None;
    private float? pendingRotationY = null;

    private readonly float brickHeight = 0.5f;

    private Animator childAnimator;
    private int currentBrickCount;

    private void Awake()
    {
        childAnimator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        EventManager.SubscribeTo<SwipeMoveEvent>(OnSwipe);
        EventManager.SubscribeTo<QiaoPlayerCollisionEvent>(OnQiaoPlayerCollision);
        EventManager.SubscribeTo<Zhuanjiao1ColisionEvent>(OnZhuanjiao1Collision);
        EventManager.SubscribeTo<FinishEvent>(OnFinish);
    }

    private void OnDisable()
    {
        EventManager.UnsubscribeFrom<SwipeMoveEvent>(OnSwipe);
        EventManager.UnsubscribeFrom<QiaoPlayerCollisionEvent>(OnQiaoPlayerCollision);
        EventManager.UnsubscribeFrom<Zhuanjiao1ColisionEvent>(OnZhuanjiao1Collision);
        EventManager.UnsubscribeFrom<FinishEvent>(OnFinish);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!isMoving)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.fixedDeltaTime);
 
        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            transform.position = targetPos;
            isMoving = false;

            if (pendingRotationY.HasValue)
            {
                currentDir = GetNewDirection(currentDir, pendingRotationY.Value);
                targetPos = CalculateTarget(GetVectorFromEnum(currentDir));
                targetPos.y = transform.position.y;
                isMoving = true;
                pendingRotationY = null;
            }
            else
            {
                currentDir = MoveDirection.None;
            }
        }
    }

    private void OnFinish(ref FinishEvent data)
    {
        isFinished = true;
        isMoving = false;
        currentDir = MoveDirection.None;
        transform.position = data.FinishPosition;

        childAnimator.SetTrigger("Win");
        Transform body=transform.GetChild(1);
        body.localRotation = Quaternion.Euler(0, 0, 0);
        RemoveAllBricks();

        StartCoroutine(DelayedWinEvent(currentBrickCount));
    }

    private IEnumerator DelayedWinEvent(int count)
    {
        yield return new WaitForSeconds(2f);
        EventManager.Raise(new LevelWinEvent { StackCount = count });
    }

    private void OnZhuanjiao1Collision(ref Zhuanjiao1ColisionEvent data)
    {
        if (data.RotationY != null)
        {
            pendingRotationY = data.RotationY;
        }
    }

    private void OnQiaoPlayerCollision(ref QiaoPlayerCollisionEvent data)
    {
        if (data.Qiao != null)
        {
            if (currentBrickCount > 0)
            {
                data.Qiao.OnBrickExchanged();
                RemoveFoorBrick();
            }
            else
            { 
                StopMovementAndLose();
            }
        }
    }

    private void OnSwipe(ref SwipeMoveEvent data)
    {
        if (isMoving || currentDir != MoveDirection.None)
        {
            return;
        }

        currentDir = data.Direction;

        targetPos = CalculateTarget(GetVectorFromEnum(currentDir));
        targetPos.y = transform.position.y;
        isMoving = true;
    }

    private Vector3 GetVectorFromEnum(MoveDirection dir)
    {
        return dir switch
        {
            MoveDirection.Up => Vector3.forward,
            MoveDirection.Down => Vector3.back,
            MoveDirection.Left => Vector3.left,
            MoveDirection.Right => Vector3.right,
            _ => Vector3.zero
        };
    }

    private Vector3 CalculateTarget(Vector3 direction)
    {
        Vector3 rayOrigin = transform.position + Vector3.up * offset;

        float maxDistance = 100f;
        RaycastHit hit;

        Debug.DrawRay(rayOrigin, direction * maxDistance, Color.red, 2f);

        if (Physics.Raycast(rayOrigin, direction, out hit, maxDistance, obstacleLayer))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                Vector3 targetPos = hit.point - direction * 0.5f;
                Vector2Int cellPos = GridSystem.Instance.WorldToCell(targetPos);
                return GridSystem.Instance.CellToWorld(cellPos);
            }

            Vector3 noWallTarget = transform.position + (direction * maxDistance);
            Vector2Int finalCellPos = GridSystem.Instance.WorldToCell(noWallTarget);

            Vector3 result = GridSystem.Instance.CellToWorld(finalCellPos);
            return new Vector3(result.x, transform.position.y, result.z);
        }

        Vector3 fallbackTarget = transform.position + (direction * maxDistance);
        Vector2Int fallbackCell = GridSystem.Instance.WorldToCell(fallbackTarget);

        Vector3 fallbackResult = GridSystem.Instance.CellToWorld(fallbackCell);
        return new Vector3(fallbackResult.x, transform.position.y, fallbackResult.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Brick") && other.gameObject.activeSelf)
        {
            other.gameObject.SetActive(false);

            if (childAnimator != null)
            {
                childAnimator.SetTrigger("PlayTake");
            }

            SpawnFootBrick();
        }
    }

    private void SpawnFootBrick()
    {
        currentBrickCount++;

        GameObject newFootBrick = Instantiate(footBrickPrefab, collectionBricks);
        float brickY = (currentBrickCount - 1) * brickHeight;
        newFootBrick.transform.localPosition += new Vector3(0, brickY, 0);
        newFootBrick.transform.localRotation = Quaternion.Euler(-90f, 0f, -180f);

        float playerY = currentBrickCount * brickHeight;
        visualGroup.localPosition = new Vector3(0, playerY, 0);
    }

    public void RemoveFoorBrick()
    {
        if (collectionBricks.childCount > 0)
        {
            currentBrickCount--;

            Transform lastBrick = collectionBricks.GetChild(collectionBricks.childCount - 1);
            Destroy(lastBrick.gameObject);

            float playerY = currentBrickCount * brickHeight;
            visualGroup.localPosition = new Vector3(0, playerY, 0);

           
        }
    }
    public void StopMovementAndLose()
    {
        if (!isFinished && isMoving)
        {
            isMoving = false; 
            Vector3 moveDir = GetVectorFromEnum(currentDir);
            Vector2Int gridPos = GridSystem.Instance.WorldToCell(transform.position);
            targetPos = GridSystem.Instance.CellToWorld(gridPos);
            targetPos.y = transform.position.y;
            transform.position = targetPos;
            StartCoroutine(DelayedLoseEvent());
        }
    }
    //public void RemoveFoorBrick()
    //{
    //    if (currentBrickCount > 0)
    //    {
    //        currentBrickCount--;

    //        if (collectionBricks.childCount > 0)
    //        {
    //            Transform lastBrick = collectionBricks.GetChild(collectionBricks.childCount - 1);
    //            Destroy(lastBrick.gameObject);
    //        }

    //        float playerY = currentBrickCount * brickHeight;
    //        visualGroup.localPosition = new Vector3(0, playerY, 0);
    //    }
    //    //else
    //    //{
    //    //    if (!isFinished && isMoving)
    //    //    {
    //    //        isMoving = false; 
    //    //        Vector3 moveDir = GetVectorFromEnum(currentDir);
    //    //        Vector2Int gridPos = GridSystem.Instance.WorldToCell(transform.position);
    //    //        targetPos = GridSystem.Instance.CellToWorld(gridPos);
    //    //        targetPos.y = transform.position.y;
    //    //        transform.position = targetPos;

    //    //        StartCoroutine(DelayedLoseEvent());
    //    //    }
    //    //}
    //}
    private IEnumerator DelayedLoseEvent()
    {
        yield return new WaitForSeconds(1.5f);
        EventManager.Raise(new LevelLoseEvent());
    }

    public void RemoveAllBricks()
    {
        int countRemoved = collectionBricks.childCount;

        foreach (Transform child in collectionBricks)
        {
            Destroy(child.gameObject);
        }

        visualGroup.localPosition = Vector3.zero;
    }

    private MoveDirection GetNewDirection(MoveDirection currentDir, float objectYRotation)
    {
        int rotation = Mathf.RoundToInt(objectYRotation);
        rotation = (rotation % 360 + 360) % 360;

        switch (rotation)
        {
            case 90:
                if (currentDir == MoveDirection.Right) return MoveDirection.Up;
                if (currentDir == MoveDirection.Down) return MoveDirection.Left;
                break;

            case 0:
                if (currentDir == MoveDirection.Right) return MoveDirection.Down;
                if (currentDir == MoveDirection.Up) return MoveDirection.Left;
                break;

            case 270:
                if (currentDir == MoveDirection.Left) return MoveDirection.Down;
                if (currentDir == MoveDirection.Up) return MoveDirection.Right;
                break;

            case 180:
                if (currentDir == MoveDirection.Left) return MoveDirection.Up;
                if (currentDir == MoveDirection.Down) return MoveDirection.Right;
                break;
        }

        return currentDir;
    }
}