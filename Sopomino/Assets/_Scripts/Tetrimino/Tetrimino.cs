using UnityEngine;

public class Tetrimino : MonoBehaviour
{

    [SerializeField] private Vector3 _rotationPoint = Vector3.zero;

    private bool _arrowPressed = false;

    private float _timeToFall = 0.85f;
    private float _fallTime = 0f;

    private float _intervalToPress = 0.2f;
    private float _pressedTime;

    private const int MAP_WIDTH = 10;
    private const int MAP_HEIGHT = 22;

    private static Transform[,] grid = new Transform[MAP_WIDTH, MAP_HEIGHT];

    public delegate void FallAction();
    public static event FallAction OnFalled;


    private void Update()
    {
        MoveByOne();
        Rotate();

        if (Input.GetKey(KeyCode.DownArrow)) {
            _fallTime *= 5;
        }
    }

    private void FixedUpdate()
    {
        _fallTime += !_arrowPressed ? Time.deltaTime : Time.deltaTime * 0.5f;

        if (_arrowPressed) {
            _pressedTime += Time.deltaTime;
        }

        if (_fallTime >= _timeToFall) {
            Fall();
            _fallTime = 0f;
        }

        if (_pressedTime > _intervalToPress) {
            Move();
        }
    }

    private void Fall()
    {
        transform.position += new Vector3(0, -1, 0);
        if (!ValidMove()) {
            transform.position += new Vector3(0, 1, 0);
            OnFalled?.Invoke();
            Fallen();
        }
    }

    private void Rotate()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            _arrowPressed = true;
            RotateBy90();
            if (!ValidMove()) {
                RotateBy90(-1);
            }
        }
    }

    private void Move()
    {
        if (Input.GetKey(KeyCode.LeftArrow)) {
            MoveLeft();
            return;
        }

        if (Input.GetKey(KeyCode.RightArrow)) {
            MoveRight();
            return;
        }

        ResetPressed();
    }

    private void MoveByOne()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            MoveLeft();
            _arrowPressed = true;
            _pressedTime = 0f;
            return;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            MoveRight();
            _pressedTime = 0f;
            _arrowPressed = true;
            return;
        }
    }

    private void MoveLeft()
    {
        transform.position += new Vector3(-1, 0, 0);
        if (!ValidMove()) {
            transform.position += new Vector3(1, 0, 0);
        }
    }

    private void MoveRight()
    {
        transform.position += new Vector3(1, 0, 0);
        if (!ValidMove()) {
            transform.position += new Vector3(-1, 0, 0);
        }
    }

    private void Fallen()
    {
        AddToGrid();
        CheckLines();
        this.enabled = false;
    }

    private void CheckLines()
    {
        for (int i = MAP_HEIGHT - 1; i >= 0; i--) {
            if (HasLine(i)) {
                DeleteLine(i);
                DownLines(i);
            }
        }
    }

    private bool HasLine(int i)
    {
        for (int j = 0; j < MAP_WIDTH; j++) {
            if (!grid[j,i]) {
                return false;
            }
        }
        return true;
    }

    private void DeleteLine(int i)
    {
        for (int j = 0; j < MAP_WIDTH; j++) {
            Destroy(grid[j,i].gameObject);
            grid[j,i] = null;
        }
    }

    private void DownLines(int i)
    {
        for (int y = i; y < MAP_HEIGHT; y++) {
            DownLine(y);
        }
    }

    private void DownLine(int i)
    {
        for (int j = 0; j < MAP_WIDTH; j++) {
            if (grid[j,i]) {
                grid[j,i - 1] = grid[j,i];
                grid[j,i - 1].transform.position += new Vector3(0, -1, 0);
                grid[j,i] = null;
            }
        }
    }

    private void AddToGrid()
    {
        foreach (Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.position.x);
            int roundedY = Mathf.RoundToInt(children.position.y);

            grid[roundedX, roundedY] = children;
        }
    }

    private void ResetPressed()
    {
        _arrowPressed = false;
        _pressedTime = 0f;
    }

    private bool ValidMove()
    {
        foreach (Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.position.x);
            int roundedY = Mathf.RoundToInt(children.position.y);

            if (roundedX < 0 || roundedX >= MAP_WIDTH || roundedY < 0 || roundedY >= MAP_HEIGHT) {
                return false;
            }

            if (grid[roundedX, roundedY] != null) {
                return false;
            }
        }

        return true;
    }

    private void RotateBy90(int multiplicator = 1)
    {
        transform.RotateAround(transform.TransformPoint(_rotationPoint), Vector3.forward, multiplicator * (-90));
    }
}
