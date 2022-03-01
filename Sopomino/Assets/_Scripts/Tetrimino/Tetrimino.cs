using UnityEngine;

public class Tetrimino : MonoBehaviour
{

    [SerializeField] private Vector3 _rotationPoint = Vector3.zero;

    private bool _arrowPressed = false;

    private const float TIME_TO_FALL = 0.85f;
    private float _timeToFall = TIME_TO_FALL;
    private float _fallTime = 0f;

    private bool _florTouched = false;
    private float _allowedFlorTouchedTime = 4f;
    private float _florTouchedTime = 0f;

    private float _intervalToPress = 0.2f;
    private float _pressedTime;

    public delegate void FallAction();
    public static event FallAction OnFalled;


    private void Update()
    {
        MoveByOne();
        Rotate();
        SetFallSpeed();
    }

    private void FixedUpdate()
    {
        // increase the pressedTime in order to make the tetrimino move laterally faster
        if (_arrowPressed) {
            _pressedTime += Time.fixedDeltaTime;
        }

        _fallTime += Time.fixedDeltaTime;

        if (_florTouched) {
            _florTouchedTime += Time.fixedDeltaTime;
        }

        // make the tetrimino fall if enough time has been past
        if (_fallTime >= _timeToFall) {
            Fall();
            _fallTime = 0f;
        }

        // the pressedTime is enough to make the tetrimino move laterally faster
        if (_pressedTime > _intervalToPress) {
            Move();
        }
    }

    private void Fall()
    {
        if ((_florTouched && !_arrowPressed && _florTouchedTime > TIME_TO_FALL) || (_florTouchedTime > _allowedFlorTouchedTime)) {
            Fallen();
            OnFalled?.Invoke();
        }

        transform.position += new Vector3(0, -1, 0);

        // cancel the fall if it's not valid and start counting time since
        if (!ValidMove()) {
            transform.position += new Vector3(0, 1, 0);
            _florTouched = true;
        }
    }

    private void SetFallSpeed()
    {
        if (Input.GetKey(KeyCode.DownArrow)) {
            _timeToFall = 0f;
            return;
        }
        _timeToFall = TIME_TO_FALL;
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
        this.enabled = false;
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

            if (roundedX < 0 || roundedX >= TetriminosManager.MAP_WIDTH || roundedY < 0 || roundedY >= TetriminosManager.MAP_HEIGHT) {
                return false;
            }

            if (TetriminosManager.Instance.Grid[roundedX, roundedY] != null) {
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
