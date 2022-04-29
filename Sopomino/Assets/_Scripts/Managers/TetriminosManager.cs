using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class TetriminosManager : StaticInstance<TetriminosManager>
{

    #region Fields

    [SerializeField]
    private GameObject _environment;

    [SerializeField]
    private List<Tetrimino> _tetriminos;

    private Dictionary<string, Tetrimino> _previewTetriminos;
    private const string PREVIEW_PREFIX = "Preview ";

    [HideInInspector]
    public ReadOnlyCollection<Tetrimino> NextTetriminos {
        get {
            return _nextTetriminos.AsReadOnly();
        }
    }
    private List<Tetrimino> _nextTetriminos;

    private Tetrimino _currentTetrimino;

    private Tetrimino _swappableTetrimino;
    private Tetrimino _swappableToDestroy;
    private bool _canSwap;

    public delegate void ScoreAction();
    public static event ScoreAction OnScoreChange;

    public delegate void SwappableAction(string swappableName);
    public static event SwappableAction OnSwappableChange;

    public delegate void TetriminoBufferAction();
    public static event TetriminoBufferAction OnTetriminoBufferChange;

    private const string SWAPPABLE_NAME = "Swappable";

    public const int MAP_WIDTH = 10;
    public const int MAP_HEIGHT = 22;

    private int _combo;
    public int Score { get; private set; }

    public int Lines { get; private set; }

    public Transform[,] Grid { get; private set; }

    private const int BUFFER_SIZE = 4;

    private bool _isDead;

    #endregion

    #region Method

    private void OnEnable() {
        Tetrimino.OnFalled += AddTetriminoToGrid;
        Tetrimino.OnFalled += CheckLines;
        Tetrimino.OnFalled += SpawnTetrimino;
    }

    private void OnDisable()
    {
        Tetrimino.OnFalled -= AddTetriminoToGrid;
        Tetrimino.OnFalled -= CheckLines;
        Tetrimino.OnFalled -= SpawnTetrimino;
    }

    private void OnDestroy()
    {
        EndGame();
    }

    public void StartGame()
    {
        Score = 0;
        Lines = 0;
        Grid = new Transform[MAP_WIDTH, MAP_HEIGHT];
        _nextTetriminos = new List<Tetrimino>(BUFFER_SIZE);
        _previewTetriminos = new Dictionary<string, Tetrimino>();
        _swappableTetrimino = null;
        _isDead = false;
        _canSwap = true;

        OnScoreChange?.Invoke();
        AddTetriminoToBuffer(BUFFER_SIZE);
        if (_previewTetriminos.Count != _tetriminos.Count) {
            CreatePreviewTetriminos();
        }
        SpawnTetrimino();
    }

    public void EndGame()
    {
        Grid = null;
        _nextTetriminos = null;
        _previewTetriminos = null;
        _swappableTetrimino = null;
    }

    private void CreatePreviewTetriminos()
    {
        foreach (Tetrimino tetrimino in _tetriminos)
        {
            createPreviewTetrimono(tetrimino);
        }

        void createPreviewTetrimono(Tetrimino tetrimino)
        {
            Tetrimino previewTetrimino = Instantiate(tetrimino);

            previewTetrimino.name = PREVIEW_PREFIX + tetrimino.name;
            previewTetrimino.enabled = false;
            previewTetrimino.gameObject.SetActive(false);

            foreach (Transform children in previewTetrimino.transform)
            {
                SpriteRenderer childrenSr = children.GetComponent<SpriteRenderer>();
                Color tmp = childrenSr.color;
                tmp.a = 0.5f;
                childrenSr.color = tmp;
            }

            _previewTetriminos.Add(previewTetrimino.name, previewTetrimino);
        }
    }

    private Tetrimino InstantiateTetrimino(Tetrimino tetrimino)
    {
        Tetrimino newTetrimino = Instantiate(tetrimino);

        Tetrimino previewTetrimino = _previewTetriminos[PREVIEW_PREFIX + tetrimino.name];

        previewTetrimino.gameObject.SetActive(true);
        newTetrimino.PreviewTetrimino = previewTetrimino;

        return newTetrimino;
    }

    private Tetrimino Instantiate(Tetrimino tetrimino)
    {
        return Instantiate(
            tetrimino,
            transform.position,
            Quaternion.identity,
            _environment.transform
        );
    }

    private void SpawnTetrimino()
    {
        if (_isDead) {
            return;
        }

        _currentTetrimino = InstantiateTetrimino(_nextTetriminos[0]);

        _nextTetriminos.Remove(_nextTetriminos[0]);
        AddTetriminoToBuffer();

        _canSwap = true;
    }


    // TODO: refacto la fonction en plusieurs petite
    public void Swap()
    {
        Tetrimino swappedTetrimino = null;

        if (!_canSwap) {
            return;
        }

        swappedTetrimino = GetTetriminoFromSwap();
        SetSwappedName(swappedTetrimino.gameObject);

        _swappableTetrimino = _currentTetrimino;

        _swappableTetrimino.gameObject.SetActive(false);


        SetSwappableName();

        _currentTetrimino = InstantiateTetrimino(swappedTetrimino);

        _currentTetrimino.gameObject.SetActive(true);

        OnSwappableChange?.Invoke(_swappableTetrimino.name.Split(' ')[0]);

        if (_swappableToDestroy) {
            Destroy(_swappableToDestroy.gameObject);
        }

        _canSwap = false;
    }

    private Tetrimino GetTetriminoFromSwap()
    {
        Tetrimino swapped = _swappableTetrimino;

        // it can be null and it's controlled during the destruction
        _swappableToDestroy = _swappableTetrimino;

        if (swapped == null) {
            swapped = _nextTetriminos[0];
            _nextTetriminos.Remove(_nextTetriminos[0]);
            AddTetriminoToBuffer();
        }

        return swapped;
    }

    private void AddTetriminoToBuffer(int count = 1)
    {
        Tetrimino newTetrimino = null;

        for (int i = 0; i < count; i++)
        {
            int rand = Random.Range(0, _tetriminos.Count);
            newTetrimino = _tetriminos[rand];
            _nextTetriminos.Add(newTetrimino);
        }

        OnTetriminoBufferChange?.Invoke();
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

            Grid[roundedX, roundedY] = children;
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
        _combo = 1;

        if (_isDead) {
            return;
        }

        for (int i = MAP_HEIGHT - 1; i >= 0; i--) {
            if (HasLine(i)) {
                DeleteLine(i);
                DownLines(i);
                IncrementScore();
            }
        }
    }

    private void IncrementScore()
    {
        Score += _combo;
        Lines++;
        _combo++;

        OnScoreChange?.Invoke();
    }

    private bool HasLine(int i)
    {
        for (int j = 0; j < MAP_WIDTH; j++) {
            if (!Grid[j,i]) {
                return false;
            }
        }
        return true;
    }

    private void DeleteLine(int i)
    {
        for (int j = 0; j < MAP_WIDTH; j++) {
            Destroy(Grid[j,i].gameObject);
            Grid[j,i] = null;
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
            if (Grid[j,i]) {
                Grid[j,i - 1] = Grid[j,i];
                Grid[j,i - 1].transform.position += new Vector3(0, -1, 0);
                Grid[j,i] = null;
            }
        }
    }

    private void SetSwappableName()
    {
        string shape = _swappableTetrimino.name.Split('(')[0];

        _swappableTetrimino.name = shape + " " + SWAPPABLE_NAME;
    }

    private void SetSwappedName(GameObject swapped)
    {
        swapped.name = swapped.name.RemoveContained(SWAPPABLE_NAME);
        swapped.name = swapped.name.Split('(')[0].Trim();
    }

    #endregion
}
