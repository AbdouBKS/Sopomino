using UnityEngine;

public class Tetrimino : MonoBehaviour
{

    [SerializeField] private Vector3 _rotationPoint = Vector3.zero;

    private bool _arrowPressed = false;

    private float _intervalToPress = 0.2f;
    private float _pressedTime;

    public static int mapWidth = 10;
    public static int mapHeight = 20;

    private void Update()
    {
        MoveByOne();
        Rotate();
    }

    private void FixedUpdate()
    {
        if (_arrowPressed) {
            _pressedTime += Time.deltaTime;
        }

        if (_pressedTime > _intervalToPress) {
            Move();
        }
    }


    private void Rotate()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
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

            if (roundedX < 0 || roundedX >= mapWidth || roundedY < 0 || roundedY >= mapHeight) {
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
