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

    private float _intervalToPress = 0.125f;
    private float _pressedTime;

    private Tetrimino _previewTetrimino;

    public delegate void FallAction();
    public static event FallAction OnFalled;

    private void OnDisable() {
        if (_previewTetrimino) {
            Destroy(_previewTetrimino.gameObject);
        }
    }

    private void Start()
    {
        CreatePreviewTetrimino();
    }

    private void Update()
    {
        if (GameManager.Instance.State == GameState.Pause) {
            return;
        }

        MoveByOne();
        Rotate();
        SetFallSpeed();
        Preview();
        ShotTetrimino();
        Swap();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.State == GameState.Pause) {
            return;
        }

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
        transform.position += new Vector3(0, -1, 0);

        // cancel the fall if it's not valid and start counting time since
        if (!this.ValidMove()) {
            transform.position += new Vector3(0, 1, 0);
            _florTouched = true;
        } else {
            _florTouched = false;
        }

        if ((_florTouched && !_arrowPressed && _florTouchedTime > TIME_TO_FALL) || (_florTouchedTime > _allowedFlorTouchedTime)) {
            Fallen();
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

    private void Swap()
    {
        if (Input.GetKeyDown(KeyCode.C)) {
            TetriminosManager.Instance.Swap();
        }
    }

    private void ShotTetrimino()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            transform.position = _previewTetrimino.transform.position;
            Fallen();
        }
    }

    private void Rotate()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            _arrowPressed = true;
            RotateBy90();
            if (!this.ValidMove()) {
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
        if (!this.ValidMove()) {
            transform.position += new Vector3(1, 0, 0);
        }
    }

    private void MoveRight()
    {
        transform.position += new Vector3(1, 0, 0);
        if (!this.ValidMove()) {
            transform.position += new Vector3(-1, 0, 0);
        }
    }

    private void Preview()
    {

        _previewTetrimino.transform.position = transform.position;

        while (_previewTetrimino.ValidMove())
        {
            _previewTetrimino.transform.position += new Vector3(0, -1, 0);
        }
        _previewTetrimino.transform.position -= new Vector3(0, -1, 0);
    }

    private void CreatePreviewTetrimino()
    {
        _previewTetrimino = this.Duplicate();

        _previewTetrimino.enabled = false;

        foreach (Transform children in _previewTetrimino.transform)
        {
            Color tmp = children.GetComponent<SpriteRenderer>().color;
            tmp.a = 0.5f;
            children.GetComponent<SpriteRenderer>().color = tmp;
        }
    }

    private void Fallen()
    {
        OnFalled?.Invoke();
        Destroy(_previewTetrimino.gameObject);
        this.enabled = false;
    }

    private void ResetPressed()
    {
        _arrowPressed = false;
        _pressedTime = 0f;
    }

    private void RotateBy90(int multiplicator = 1)
    {
        transform.RotateAround(transform.TransformPoint(_rotationPoint), Vector3.forward, multiplicator * (-90));
        _previewTetrimino.transform.RotateAround(_previewTetrimino.transform.TransformPoint(_rotationPoint), Vector3.forward, multiplicator * (-90));
    }
}
