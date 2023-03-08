using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GridManager : Singleton<GridManager>
{

    #region Fields

    public Transform[,] Grid { get; private set; }

    [SerializeField] private GameObject tetriminosEnvironment;

    private int _penaltyCount;

    [SerializeField] private GameObject emptySquare;

    private bool _isDead;

    public int penaltyCount {
        get => _penaltyCount;
        private set {
            _penaltyCount = Mathf.Clamp(value, 0, MaxPenaltyCount);
            OnPenaltyCountChange?.Invoke(_penaltyCount);
        }
    }

    #region Actions

    public static Action<int> OnPenaltyCountChange;
    public static Action<int> OnLinesComplete;

    #endregion

    #region Constants

    public const int MapWidth = 10;
    public const int MapHeight = 22;
    private const int MaxPenaltyCount = 20;

    #endregion Constants

    #endregion Fields

    #region Methods

    private void OnEnable()
    {
        GameManager.OnBeforeStateChanged +=CleanTetriminos;
        Tetrimino.OnFalled += AddTetriminoToGrid;
        Tetrimino.OnFalled += CheckFullLines;
        Tetrimino.HasFallen += AddPenaltyLines;

    }

    private void OnDisable()
    {
        GameManager.OnBeforeStateChanged -= CleanTetriminos;
        Tetrimino.OnFalled -= AddTetriminoToGrid;
        Tetrimino.OnFalled -= CheckFullLines;
        Tetrimino.HasFallen -= AddPenaltyLines;
    }

    protected override void Awake()
    {
        base.Awake();
        if (tetriminosEnvironment == null) {
            tetriminosEnvironment = GameObject.Find("Tetriminos");
        }
        Grid = new Transform[MapWidth, MapHeight + 1];
    }

    public void StartGame()
    {
        if (Grid == null) {
            Grid = new Transform[MapWidth, MapHeight + 1];
        }
        _isDead = false;
        penaltyCount = 0;
    }

    public void EndGame()
    {
        Grid = null;
    }

    // TMP
    // private void Update() {
    //     if (Input.GetKeyDown(KeyCode.M)) {
    //         IncrementPenaltyCount(UnityEngine.Random.Range(0, 4));
    //     }
    // }

    public void CleanTetriminos(GameState state)
    {
        if (state != GameState.TryAgain) {
            return;
        }

        tetriminosEnvironment.transform.DestroyChildren();
    }

    private void IncrementPenaltyCount(int count = 1)
    {
        penaltyCount += count;
    }

    public void AddTetriminoToGrid(Tetrimino tetrimino)
    {
        foreach (Transform children in tetrimino.transform) {
            var position = children.position;
            var roundedX = Mathf.RoundToInt(position.x);
            var roundedY = Mathf.RoundToInt(position.y);

            if (roundedY >= MapHeight - 1) {
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
            for (int j = 0; j < MapWidth; j++) {
                Destroy(Grid[j,i].gameObject);
                Grid[j,i] = null;
            }
        }

        void DownLines(int i)
        {
            for (int y = i; y < MapHeight; y++) {
                DownLine(y);
            }
        }

        void DownLine(int i)
        {;
            for (int j = 0; j < MapWidth; j++) {
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
            for (int j = 0; j < MapWidth; j++) {
                if (!Grid[j,i]) {
                    return false;
                }
            }
            return true;
        }
    }

    /// <summary>
    /// Add penalty lines at the bottom of the Grid
    /// </summary>
    private void AddPenaltyLines()
    {
        int lineNumber = penaltyCount;
        int skipSquare = UnityEngine.Random.Range(0, MapWidth - 1);

        for (int i = 0; i < lineNumber; i++)
        {
            UpLines();
            FillNewLine(0);
            if (IsDead()) {
                Die();
                return;
            }
        }

        penaltyCount = 0;
        return;

        void UpLines()
        {
            for (int y = MapHeight - 1; y >= 0; y--) {
                UpLine(y);
            }
        }

        void UpLine(int y)
        {
            for (int x = 0; x < MapWidth; x++) {
                if (!Grid[x,y]) {
                    continue;
                }

                Grid[x,y + 1] = Grid[x,y];
                Grid[x,y + 1].transform.position += Vector3.up;
                Grid[x,y] = null;
            }
        }

        void FillNewLine(int y)
        {
            for (int x = 0; x < MapWidth; x++) {
                if (x == skipSquare) {
                    continue;
                }
                GameObject square = Instantiate(
                    emptySquare,
                    new Vector3(x, y, 0),
                    Quaternion.identity, tetriminosEnvironment.transform
                );
                Grid[x,y] = square.transform;
            }
        }

        bool IsDead()
        {
            for (int x = 0; x < MapWidth; x++)
            {
                if (Grid[x, MapHeight]) {
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

    #endregion Methods
}
