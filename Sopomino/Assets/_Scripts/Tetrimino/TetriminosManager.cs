using System.Collections.Generic;
using UnityEngine;

public class TetriminosManager : StaticInstance<TetriminosManager>
{
    [SerializeField]
    private GameObject _environment;

    [SerializeField]
    private List<Tetrimino> _allTetriminos;

    private List<Tetrimino> _nextTetrimios;
    private Tetrimino _currentTetrimino;

    [SerializeField]
    private Transform _swappableTetriminoPosition;
    private GameObject _swappableTetrimino;
    private GameObject _swappableToDestroy;
    private bool _canSwap;
    private const string SWAPPABLE_NAME = "Swappable";

    public const int MAP_WIDTH = 10;
    public const int MAP_HEIGHT = 22;

    private Transform[,] _grid;
    public Transform[,] Grid {
        get {return _grid;}
    }

    private const int BUFFER_SIZE = 4;

    private bool _isDead;

    private void OnEnable() {
        Tetrimino.OnFalled += AddTetriminoToGrid;
        Tetrimino.OnFalled += CheckLines;
        Tetrimino.OnFalled += SpawnTetrimino;

        _grid = new Transform[MAP_WIDTH, MAP_HEIGHT];
        _nextTetrimios = new List<Tetrimino>(BUFFER_SIZE);
        _isDead = false;
        _swappableTetrimino = null;
        _canSwap = true;


        AddTetriminoToBuffer(BUFFER_SIZE);
        SpawnTetrimino();
    }

    private void OnDisable() {
        Tetrimino.OnFalled -= AddTetriminoToGrid;
        Tetrimino.OnFalled -= CheckLines;
        Tetrimino.OnFalled -= SpawnTetrimino;

        _grid = null;
        _nextTetrimios = null;
        _swappableTetrimino = null;
    }

    public void SpawnTetrimino() {
        if (_isDead) {
            return;
        }

        Tetrimino newTetrimino = _nextTetrimios[0];

        _nextTetrimios.Remove(_nextTetrimios[0]);
        AddTetriminoToBuffer();

        _currentTetrimino = Instantiate(
            newTetrimino,
            transform.position,
            Quaternion.identity,
            _environment.transform
        );

        _canSwap = true;
    }

    // TODO: refacto la fonction en plusieurs petite
    public void Swap()
    {
        GameObject swappedObject = null;

        if (!_canSwap) {
            return;
        }

        swappedObject = GetTetriminoObjectFromSwap();
        setSwappedName(swappedObject);

        _swappableTetrimino = _currentTetrimino.gameObject;

        _swappableTetrimino.transform.position = _swappableTetriminoPosition.transform.position;
        _swappableTetrimino.GetComponent<Tetrimino>().enabled = false;
        _swappableTetrimino.transform.rotation = Quaternion.identity;
        setSwappableName();

        _currentTetrimino = Instantiate(
            swappedObject,
            transform.position,
            Quaternion.identity,
            _environment.transform
        ).GetComponent<Tetrimino>();
        _currentTetrimino.enabled = true;


        if (_swappableToDestroy) {
            Destroy(_swappableToDestroy);
        }

        _canSwap = false;
    }

    private GameObject GetTetriminoObjectFromSwap()
    {
        GameObject swapped = _swappableTetrimino;

        // it can be null and it's controlled during the destruction
        _swappableToDestroy = _swappableTetrimino;

        if (swapped == null) {
            swapped = _nextTetrimios[0].gameObject;
            _nextTetrimios.Remove(_nextTetrimios[0]);
            AddTetriminoToBuffer();
        }

        return swapped;
    }

    private void AddTetriminoToBuffer(int count = 1)
    {
        Tetrimino newTetrimino = null;

        for (int i = 0; i < count; i++)
        {
            int rand = Random.Range(0, _allTetriminos.Count);
            newTetrimino = _allTetriminos[rand];
            _nextTetrimios.Add(newTetrimino);
        }
    }

    private void AddTetriminoToGrid()
    {
        foreach (Transform children in _currentTetrimino.transform)
        {
            int roundedX = Mathf.RoundToInt(children.position.x);
            int roundedY = Mathf.RoundToInt(children.position.y);

            if (roundedY >= MAP_HEIGHT - 1) {
                Die();
                return;
            }

            _grid[roundedX, roundedY] = children;
        }
    }

    private void Die()
    {
        _isDead = true;
        GameManager.Instance.ChangeState(GameState.Loose);
    }

    public void CleanTetriminos()
    {
        _environment.transform.DestroyChildren();
    }

    private void CheckLines()
    {
        if (_isDead) {
            return;
        }

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
            if (!_grid[j,i]) {
                return false;
            }
        }
        return true;
    }

    private void DeleteLine(int i)
    {
        for (int j = 0; j < MAP_WIDTH; j++) {
            Destroy(_grid[j,i].gameObject);
            _grid[j,i] = null;
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
            if (_grid[j,i]) {
                _grid[j,i - 1] = _grid[j,i];
                _grid[j,i - 1].transform.position += new Vector3(0, -1, 0);
                _grid[j,i] = null;
            }
        }
    }

    private void setSwappableName()
    {
        string shape = _swappableTetrimino.name.Split('(')[0];

        _swappableTetrimino.name = shape + " " + SWAPPABLE_NAME;
    }

    private void setSwappedName(GameObject swapped)
    {
        swapped.name = swapped.name.RemoveContained(SWAPPABLE_NAME);
        swapped.name = swapped.name.Split('(')[0];
    }
}
