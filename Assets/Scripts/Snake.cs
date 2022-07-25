using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Snake : MonoBehaviour
{
    private List<Transform> segments = new List<Transform>();

    public Transform segmentPrefab;
    public Transform segmentPrefab2;
    private bool flagColor = false;

    public Transform head;
    public Vector2 direction = Vector2.right;
    private Vector2 input;

    public int initialSize = 4;
    private Quaternion inicialRotation = Quaternion.Euler(0, 0, 180);

    private void Start()
    {
        head.rotation = inicialRotation;
        ResetState();
    }

    private void Update()
    {
        // Only allow turning up or down while moving in the x-axis
        if (direction.x != 0f)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
                input = Vector2.up;
                head.rotation = Quaternion.Euler(0, 0, -90);
            } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
                input = Vector2.down;
                head.rotation = Quaternion.Euler(0, 0, 90);
            }
        }
        // Only allow turning left or right while moving in the y-axis
        else if (direction.y != 0f)
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
                input = Vector2.right;
                head.rotation = Quaternion.Euler(0, 0, 180);
            } else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                input = Vector2.left;
                head.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    private void FixedUpdate()
    {
        // Set the new direction based on the input
        if (input != Vector2.zero) {
            direction = input;
        }

        // Set each segment's position to be the same as the one it follows. We
        // must do this in reverse order so the position is set to the previous
        // position, otherwise they will all be stacked on top of each other.
        for (int i = segments.Count - 1; i > 0; i--) {
            segments[i].position = segments[i - 1].position;
        }

        // Move the snake in the direction it is facing
        // Round the values to ensure it aligns to the grid
        float x = Mathf.Round(transform.position.x) + direction.x;
        float y = Mathf.Round(transform.position.y) + direction.y;

        transform.position = new Vector2(x, y);
    }

    public void Grow()
    {
        Transform segment;
        if (flagColor)
        {
            segment = Instantiate(segmentPrefab);
            flagColor = !flagColor;
        }
        else { 
            segment = Instantiate(segmentPrefab2);
            flagColor = !flagColor;
        }
        segment.position = segments[segments.Count - 1].position;
        segments.Add(segment);
    }

    public void ResetState()
    {
        direction = Vector2.right;
        transform.position = Vector3.zero;

        // Start at 1 to skip destroying the head
        for (int i = 1; i < segments.Count; i++) {
            Destroy(segments[i].gameObject);
        }

        // Clear the list but add back this as the head
        segments.Clear();
        segments.Add(transform);

        // -1 since the head is already in the list
        for (int i = 0; i < initialSize - 1; i++) {
            Grow();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Food")) {
            Grow();
        } else if (other.gameObject.CompareTag("Obstacle")) {
            ResetState();
        }
    }

}
