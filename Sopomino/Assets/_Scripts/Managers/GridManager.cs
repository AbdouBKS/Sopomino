using System;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{

    #region Fields

    public Transform[,] Grid { get; private set; }

    [SerializeField] private GameObject _environment;

    private int _penalityCount;

    [SerializeField] private GameObject _emptySquare;

    private bool _isDead;

    public int PenalityCount {
        get {
            return _penalityCount;
        }
        private set {
            _penalityCount = Mathf.Clamp(value, 0, MAX_PENALITY_COUNT);
            OnPenalityCountChange?.Invoke(_penalityCount);
        }
    }

    #region Actions

    public static Action<int> OnPenalityCountChange;
    public static Action<int> OnLinesComplete;

    #endregion

    #region Constants

    public const int MAP_WIDTH = 10;
    private const int MAX_PENALITY_COUNT = 20;
    public const int MAP_HEIGHT = 22;

    #endregion

    #endregion

    #region Methods

    private void OnEnable()
    {
        GameManager.OnBeforeStateChanged +=CleanTetriminos;
        Tetrimino.OnFalled += AddTetriminoToGrid;
        Tetrimino.OnFalled += CheckFullLines;
        Tetrimino.HasFallen += AddPenalityLines;

    }

    private void OnDisable()
    {
        GameManager.OnBeforeStateChanged -= CleanTetriminos;
        Tetrimino.OnFalled -= AddTetriminoToGrid;
        Tetrimino.OnFalled -= CheckFullLines;
        Tetrimino.HasFallen -= AddPenalityLines;
    }

    protected override void Awake()
    {
        base.Awake();
        if (_environment == null) {
            _environment = GameObject.Find("Environment");
        }
        Grid = new Transform[MAP_WIDTH, MAP_HEIGHT + 1];
    }

    public void StartGame()
    {
        if (Grid == null) {
            Grid = new Transform[MAP_WIDTH, MAP_HEIGHT + 1];
        }
        _isDead = false;
        PenalityCount = 0;
    }

    public void EndGame()
    {
        Grid = null;
    }

    // TMP
    // private void Update() {
    //     if (Input.GetKeyDown(KeyCode.M)) {
    //         IncrementPenalityCount(UnityEngine.Random.Range(0, 4));
    //     }
    // }

    public void CleanTetriminos(GameState state)
    {
        if (state != GameState.TryAgain) {
            return;
        }

        _environment.transform.DestroyChildren();
    }

    private void IncrementPenalityCount(int penalityCount = 1)
    {
        PenalityCount += penalityCount;
    }

    public void AddTetriminoToGrid(Tetrimino tetrimino)
    {
        foreach (Transform children in tetrimino.transform) {
            int roundedX = Mathf.RoundToInt(children.position.x);
            int roundedY = Mathf.RoundToInt(children.position.y);

            if (roundedY >= MAP_HEIGHT - 1) {
                _isDead = true;
                Die();
                return;
            }

            Grid[roundedX, roundedY] = children;
        }
    }

    private void CheckFullLines(Tetrimino tetrimino)
    {
        int combo = 0;

        if (_isDead) {
            return;
        }

        int start = GetHighestSquareHeight(tetrimino.transform);

        for (int i = start; i >= 0; i--) {
            if (IsLineFull(i)) {
                DeleteLine(i);
                DownLines(i);
                combo++;
            }
        }

        if (combo > 0) {
            OnLinesComplete?.Invoke(combo);
        }

        return;

        void DeleteLine(int i)
        {
            for (int j = 0; j < MAP_WIDTH; j++) {
                Destroy(Grid[j,i].gameObject);
                Grid[j,i] = null;
            }
        }

        void DownLines(int i)
        {
            for (int y = i; y < MAP_HEIGHT; y++) {
                DownLine(y);
            }
        }

        void DownLine(int i)
        {;
            for (int j = 0; j < MAP_WIDTH; j++) {
                if (Grid[j,i]) {
                    Grid[j,i - 1] = Grid[j,i];
                    Grid[j,i - 1].transform.position += Vector3.down;
                    Grid[j,i] = null;
                }
            }
        }

        int GetHighestSquareHeight(Transform tetrimino)
        {
            int highest = 0;
            int roundedY = 0;

            foreach (Transform children in tetrimino)
            {
                roundedY = Mathf.RoundToInt(children.position.y);
                if (roundedY > highest) {
                    highest = roundedY;
                }
            }

            return highest;
        }

        bool IsLineFull(int i)
        {
            for (int j = 0; j < MAP_WIDTH; j++) {
                if (!Grid[j,i]) {
                    return false;
                }
            }
            return true;
        }
    }

    /// <summary>
    /// Add penality lines at the bottom of the Grid
    /// </summary>
    private void AddPenalityLines()
    {
        int lineNumber = PenalityCount;
        int skipSquare = UnityEngine.Random.Range(0, MAP_WIDTH - 1);

        for (int i = 0; i < lineNumber; i++)
        {
            UpLines();
            fillNewLine(0);
            if (isDead()) {
                Die();
                return;
            }
        }

        PenalityCount = 0;
        return;

        void UpLines()
        {
            for (int y = MAP_HEIGHT - 1; y >= 0; y--) {
                upLine(y);
            }
        }

        void upLine(int y)
        {
            for (int x = 0; x < MAP_WIDTH; x++) {
                if (!Grid[x,y]) {
                    continue;
                }

                Grid[x,y + 1] = Grid[x,y];
                Grid[x,y + 1].transform.position += Vector3.up;
                Grid[x,y] = null;
            }
        }

        void fillNewLine(int y)
        {
            for (int x = 0; x < MAP_WIDTH; x++) {
                if (x == skipSquare) {
                    continue;
                }
                GameObject square = Instantiate(
                    _emptySquare,
                    new Vector3(x, y, 0),
                    Quaternion.identity, _environment.transform
                );
                Grid[x,y] = square.transform;
            }
        }

        bool isDead()
        {
            for (int x = 0; x < MAP_WIDTH; x++)
            {
                if (Grid[x, MAP_HEIGHT]) {
                    return true;
                }
            }

            return false;
        }
    }

    public void Die()
    {
        _isDead = true;
        GameManager.Instance.ChangeState(GameState.Loose);
    }

    #endregion
}
