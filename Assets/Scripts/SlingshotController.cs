using UnityEngine;

public class SlingshotController : MonoBehaviour
{
    public Transform anchor;
    public float maxDragDistance = 3f;
    public float launchMultiplier = 8f;
    public float nextBirdDelay = 2.5f;
    public float grabRadius = 150f;

    private Rigidbody currentBird;
    private bool dragging;
    private Camera cam;
    private Transform armLeftTip;
    private Transform armRightTip;
    private LineRenderer leftBand;
    private LineRenderer rightBand;

    void Start()
    {
        cam = Camera.main;
        if (anchor == null) anchor = transform;
        armLeftTip = anchor.Find("SlingshotArmLeft");
        armRightTip = anchor.Find("SlingshotArmRight");
        leftBand = CreateBand();
        rightBand = CreateBand();
        SpawnBird();
    }

    LineRenderer CreateBand()
    {
        GameObject go = new GameObject("SlingshotBand");
        go.transform.SetParent(anchor);
        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.black;
        lr.endColor = Color.black;
        return lr;
    }

    void LateUpdate()
    {
        Vector3 tip = currentBird != null ? currentBird.transform.position : anchor.position;
        if (armLeftTip != null)
        {
            leftBand.SetPosition(0, armLeftTip.position);
            leftBand.SetPosition(1, tip);
        }
        if (armRightTip != null)
        {
            rightBand.SetPosition(0, armRightTip.position);
            rightBand.SetPosition(1, tip);
        }
    }

    void Update()
    {
        if (currentBird == null || GameManager.Instance == null || GameManager.Instance.gameEnded) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 birdScreenPos = cam.WorldToScreenPoint(currentBird.position);
            float screenDist = Vector2.Distance(Input.mousePosition, new Vector2(birdScreenPos.x, birdScreenPos.y));
            if (screenDist <= grabRadius)
            {
                dragging = true;
            }
        }

        if (dragging && Input.GetMouseButton(0))
        {
            Vector3 dragPoint = GetDragPoint();
            Vector3 offset = dragPoint - anchor.position;
            if (offset.magnitude > maxDragDistance)
            {
                offset = offset.normalized * maxDragDistance;
            }
            currentBird.transform.position = anchor.position + offset;
        }

        if (dragging && Input.GetMouseButtonUp(0))
        {
            dragging = false;
            Launch();
        }
    }

    Vector3 GetDragPoint()
    {
        // Locked to the anchor's Z depth so dragging never leaks sideways (Z) velocity into the shot,
        // regardless of the camera's pitch. Keeps aiming purely a power/angle (X/Y) control.
        Plane plane = new Plane(Vector3.forward, anchor.position);
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        return anchor.position;
    }

    void Launch()
    {
        Vector3 launchDir = anchor.position - currentBird.transform.position;
        currentBird.isKinematic = false;
        currentBird.linearVelocity = launchDir * launchMultiplier;

        GameManager.Instance.RegisterShotFired();
        currentBird = null;

        if (GameManager.Instance.shotsRemaining > 0)
        {
            Invoke(nameof(SpawnBird), nextBirdDelay);
        }
    }

    void SpawnBird()
    {
        if (GameManager.Instance != null && GameManager.Instance.gameEnded) return;

        GameObject bird = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bird.name = "Bird";
        bird.tag = "Bird";
        bird.transform.position = anchor.position;
        bird.transform.localScale = Vector3.one * 0.6f;

        Rigidbody rb = bird.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        bird.GetComponent<Renderer>().material.color = Color.red;
        currentBird = rb;
    }

    // Test/verification helper: simulates a pull-and-release without real mouse input.
    public void DebugPullAndLaunch(Vector3 pullOffset)
    {
        if (currentBird == null) return;
        if (pullOffset.magnitude > maxDragDistance)
        {
            pullOffset = pullOffset.normalized * maxDragDistance;
        }
        currentBird.transform.position = anchor.position + pullOffset;
        dragging = false;
        Launch();
    }
}
