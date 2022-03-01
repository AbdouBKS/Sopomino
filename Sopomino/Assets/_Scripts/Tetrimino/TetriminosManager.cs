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

    public const int MAP_WIDTH = 10;
    public const int MAP_HEIGHT = 22;

    private Transform[,] _grid;
    public Transform[,] Grid {
        get {return _grid;}
    }

    private const int BUFFER_SIZE = 4;

    private void Start()
    {
        _grid = new Transform[MAP_WIDTH, MAP_HEIGHT];
        _nextTetrimios = new List<Tetrimino>(BUFFER_SIZE);

        AddTetriminoToBuffer(BUFFER_SIZE);
        SpawnTetrimino();
    }

    private void OnEnable() {
        Tetrimino.OnFalled += AddTetriminoToGrid;
        Tetrimino.OnFalled += CheckLines;
        Tetrimino.OnFalled += SpawnTetrimino;
    }

    private void OnDisable() {
        Tetrimino.OnFalled -= AddTetriminoToGrid;
        Tetrimino.OnFalled -= CheckLines;
        Tetrimino.OnFalled -= SpawnTetrimino;
    }

    public void SpawnTetrimino() {
        Tetrimino newTetrimino = _nextTetrimios[0];

        _nextTetrimios.Remove(newTetrimino);
        AddTetriminoToBuffer();

        _currentTetrimino = Instantiate(
            newTetrimino,
            transform.position,
            Quaternion.identity,
            _environment.transform
        );
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

            _grid[roundedX, roundedY] = children;
        }
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
}
