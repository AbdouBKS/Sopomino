using System;
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

    [HideInInspector]
    public Tetrimino PreviewTetrimino;

    private int _tryKickCount = 0;

    private enum KickTries
    {
        UP,
        UP_AGAIN,
        LEFT,
        LEFT_AGAIN,
        RIGHT,
        RIGHT_AGAIN,
    }


    public static Action<Tetrimino> OnFalled;
    public static Action HasFallen;

    private void OnDisable() {
        if (PreviewTetrimino) {
            PreviewTetrimino.gameObject.SetActive(false);
            PreviewTetrimino.transform.position =  new Vector3(999, 999, 999);
        }
    }

    private void OnEnable() {
        if (PreviewTetrimino) {
            PreviewTetrimino.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Playing) {
            return;
        }

        MoveByOne();
        GetRotate();
        SetFallSpeed();
        Preview();
        ShotTetrimino();
        Swap();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.State != GameState.Playing) {
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
        transform.position += Vector3.down;

        // cancel the fall if it's not valid and start counting time since
        if (!this.ValidMove()) {
            transform.position += Vector3.up;
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
            transform.position = PreviewTetrimino.transform.position;
            Fallen();
        }
    }

    private void GetRotate()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            _arrowPressed = true;
            Rotate();
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
        transform.position += Vector3.left;
        if (!this.ValidMove()) {
            transform.position += Vector3.right;
        }
    }

    private void MoveRight()
    {
        transform.position += Vector3.right;
        if (!this.ValidMove()) {
            transform.position += Vector3.left;

        }
    }

    private void MoveUp()
    {
        transform.position += Vector3.up;
        if (!this.ValidMove()) {
            transform.position += Vector3.down;
        }
    }

    private void Rotate()
    {
        Vector3 initialPos = transform.position;

        RotateBy90();
        if (!this.ValidMove()) {
            TryKick(initialPos);
        }
    }

    private void TryKick(Vector3 initialPos)
    {
        switch (_tryKickCount)
        {
            case (int)KickTries.UP:
                FirstTryKickDirection(Vector3.up);
                break;
            case (int)KickTries.UP_AGAIN:
                SecondTryKickDirection(Vector3.up);
                break;
            case (int)KickTries.LEFT:
                FirstTryKickDirection(Vector3.left);
                break;
            case (int)KickTries.LEFT_AGAIN:
                SecondTryKickDirection(Vector3.left);
                break;
            case (int)KickTries.RIGHT:
                FirstTryKickDirection(Vector3.right);
                break;
            case (int)KickTries.RIGHT_AGAIN:
                SecondTryKickDirection(Vector3.right);
                break;
            default:
                _tryKickCount = 0;
                RotateBy90(-1);
                transform.position = initialPos;
                break;
        }

        void FirstTryKickDirection(Vector3 direction)
        {
            transform.position += direction;
            if (!this.ValidMove()) {
                _tryKickCount++;
                TryKick(initialPos);
            }
        }

        void SecondTryKickDirection(Vector3 direction)
        {
            transform.position += direction;
            if (!this.ValidMove()) {
                _tryKickCount++;
                transform.position = initialPos;
                TryKick(initialPos);
            }
        }
    }

    private void Preview()
    {
        if (PreviewTetrimino.gameObject.activeSelf != true) {
            PreviewTetrimino.gameObject.SetActive(true);
        }
        PreviewTetrimino.transform.position = transform.position;
        PreviewTetrimino.transform.rotation = transform.rotation;

        while (PreviewTetrimino.ValidMove())
        {
            PreviewTetrimino.transform.position += new Vector3(0, -1, 0);
        }
        PreviewTetrimino.transform.position -= new Vector3(0, -1, 0);
    }

    private void Fallen()
    {
        OnFalled?.Invoke(this);
        HasFallen?.Invoke();

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
    }
}
