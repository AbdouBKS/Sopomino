using System;
using UnityEngine;
using Unity.Netcode;

public class Tetrimino : NetworkBehaviour
{
    [SerializeField] private Vector3 rotationPoint = Vector3.zero;

    private bool _arrowPressed;

    private const float TimeToFall = 0.85f;
    private float _timeToFall = TimeToFall;
    private float _fallTime;

    private bool _florTouched;
    private float _allowedFlorTouchedTime = 4f;
    private float _florTouchedTime;

    private float _intervalToPress = 0.125f;
    private float _pressedTime;

    [NonSerialized] public Tetrimino PreviewTetrimino;

    private int _tryKickCount;

    public static Action<Tetrimino> OnFalled;
    public static Action HasFallen;

    private enum KickTries
    {
        Up,
        UpAgain,
        Left,
        LeftAgain,
        Right,
        RightAgain,
    }

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

        if ((_florTouched && !_arrowPressed && _florTouchedTime > TimeToFall) || (_florTouchedTime > _allowedFlorTouchedTime)) {
            Fallen();
        }
    }

    private void SetFallSpeed()
    {
        if (Input.GetKey(KeyCode.DownArrow)) {
            _timeToFall = 0f;
            return;
        }
        _timeToFall = TimeToFall;
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
            case (int)KickTries.Up:
                FirstTryKickDirection(Vector3.up);
                break;
            case (int)KickTries.UpAgain:
                SecondTryKickDirection(Vector3.up);
                break;
            case (int)KickTries.Left:
                FirstTryKickDirection(Vector3.left);
                break;
            case (int)KickTries.LeftAgain:
                SecondTryKickDirection(Vector3.left);
                break;
            case (int)KickTries.Right:
                FirstTryKickDirection(Vector3.right);
                break;
            case (int)KickTries.RightAgain:
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

        var previewTetriminoTransform = PreviewTetrimino.transform;
        var tetriminoTransform = transform;
        previewTetriminoTransform.position = tetriminoTransform.position;
        previewTetriminoTransform.rotation = tetriminoTransform.rotation;

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

    private void RotateBy90(int multiplier = 1)
    {
        transform.RotateAround(transform.TransformPoint(rotationPoint), Vector3.forward, multiplier * (-90));
    }
}
