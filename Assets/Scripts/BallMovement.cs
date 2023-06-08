using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GG.Infrastructure.Utils.Swipe;
using DG.Tweening;

public class BallMovement : MonoBehaviour
{
    [SerializeField] private SwipeListener swipeListener;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private float stepDuration = 0.1f;
    [SerializeField] private LayerMask wallsAndRoadsLayer;
    private Rigidbody rb;
    private const float MAX_RAY_DISTANCE = 10f;
    private Vector3 moveDirection;
    private bool canMove = true;
    Vector3 targetPosition;
    int steps;

    List<GameObject> hittedObj;
    private void Start()
    {
        transform.position = levelManager.defaultBallRoadTile.position;
        swipeListener.OnSwipe.AddListener(HandleSwipe);
        rb = GetComponent<Rigidbody>();
        targetPosition = transform.position;
        hittedObj = new List<GameObject>();
    }

    private void HandleSwipe(string swipeDirection)
    {
        switch (swipeDirection)
        {
            case "Right":
                moveDirection = Vector3.right;
                break;
            case "Left":
                moveDirection = Vector3.left;
                break;
            case "Up":
                moveDirection = Vector3.forward;
                break;
            case "Down":
                moveDirection = Vector3.back;
                break;
        }
        Debug.Log(swipeDirection);
        MoveBall();
    }

    private void MoveBall()
    {
        if (!canMove)
            return;

        canMove = false;
        HitControl(transform.position, moveDirection);
        Debug.Log("MoveDirection: " + moveDirection);

        Debug.Log("TargetPosition: " + targetPosition);

        float moveDuration = stepDuration * steps;
        transform.DOMove(targetPosition, moveDuration).SetEase(Ease.OutExpo).OnComplete(() => canMove = true);
    }

    void HitControl(Vector3 startPos, Vector3 direction)
    {
        Ray ray = new Ray(startPos, direction);
        Physics.Raycast(ray, out RaycastHit hit, MAX_RAY_DISTANCE);

        if (hit.collider.isTrigger)
        {
            hittedObj.Add(hit.transform.gameObject);
            HitControl(hit.transform.position, moveDirection);
            steps++;
        }
        else
        {
            targetPosition = hittedObj[hittedObj.Count-1].transform.position;
            hittedObj.Clear();
            steps = 0;
        }
    }
}

