using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Serialization;


[RequireComponent(typeof(GridManager))]
public class TetriminosManager : StaticInstance<TetriminosManager>
{

    #region Fields

    [FormerlySerializedAs("_tetriminos")] [SerializeField]
    private List<Tetrimino> tetriminos;

    [FormerlySerializedAs("_environment")] [SerializeField]
    private GameObject environment;

    private Dictionary<string, Tetrimino> _previewTetriminos;
    private const string PreviewPrefix = "Preview ";

    public ReadOnlyCollection<Tetrimino> NextTetriminos => _nextTetriminos.AsReadOnly();

    private List<Tetrimino> _nextTetriminos;

    private Tetrimino _currentTetrimino;

    private Tetrimino _swappableTetrimino;
    private Tetrimino _swappableToDestroy;
    private bool _canSwap;

    public delegate void SwappableAction(string swappableName);
    public static event SwappableAction OnSwappableChange;

    public delegate void TetriminoBufferAction();
    public static event TetriminoBufferAction OnTetriminoBufferChange;

    private const string SwappableName = "Swappable";

    private const int BufferSize = 4;

    private bool _isDead;

    #endregion

    #region Constants

    private const int MaxPunishmentCount = 20;

    #endregion

    #region Actions

    public static Action<int> OnPunishmentCountChange;

    #endregion

    #region Method

    private void OnEnable() {
        Tetrimino.HasFallen += SpawnTetrimino;
    }

    private void OnDisable()
    {
        Tetrimino.HasFallen -= SpawnTetrimino;
    }

    private void OnDestroy()
    {
        EndGame();
    }

    public void StartGame()
    {
        _nextTetriminos = new List<Tetrimino>(BufferSize);
        _previewTetriminos = new Dictionary<string, Tetrimino>();
        _swappableTetrimino = null;
        _isDead = false;
        _canSwap = true;

        if (environment == null) {
            environment = GameObject.Find("Environment");
        }

        AddTetriminoToBuffer(BufferSize);
        if (_previewTetriminos.Count != tetriminos.Count) {
            CreatePreviewTetriminos();
        }
        SpawnTetrimino();

        void CreatePreviewTetriminos()
        {
            foreach (Tetrimino tetrimino in tetriminos)
            {
                CreatePreviewTetrimono(tetrimino);
            }

            void CreatePreviewTetrimono(Tetrimino tetrimino)
            {
                Tetrimino previewTetrimino = Instantiate(tetrimino);

                previewTetrimino.name = PreviewPrefix + tetrimino.name;
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
    }

    public void EndGame()
    {
        _nextTetriminos = null;
        _previewTetriminos = null;
        _swappableTetrimino = null;
    }

    private Tetrimino InstantiateTetrimino(Tetrimino tetrimino)
    {
        Tetrimino newTetrimino = Instantiate(tetrimino);

        Tetrimino previewTetrimino = _previewTetriminos[PreviewPrefix + tetrimino.name];

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
            environment.transform
        );
    }

    private void SpawnTetrimino()
    {
        if (_isDead) {
            return;
        }

        _currentTetrimino = InstantiateTetrimino(_nextTetriminos[0]);

        if (!_currentTetrimino.ValidMove()) {
            Die();
            return;
        }

        _nextTetriminos.Remove(_nextTetriminos[0]);
        AddTetriminoToBuffer();

        _canSwap = true;
    }

    public void Swap()
    {
        Tetrimino swappedTetrimino;

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

        Tetrimino GetTetriminoFromSwap()
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
    }

    private void Die()
    {
        _isDead = true;
        GameManager.Instance.ChangeState(GameState.Loose);
    }

    private void AddTetriminoToBuffer(int count = 1)
    {
        Tetrimino newTetrimino;

        for (int i = 0; i < count; i++)
        {
            int rand = UnityEngine.Random.Range(0, tetriminos.Count);
            newTetrimino = tetriminos[rand];
            _nextTetriminos.Add(newTetrimino);
        }

        OnTetriminoBufferChange?.Invoke();
    }

    private void SetSwappableName()
    {
        string shape = _swappableTetrimino.name.Split('(')[0];

        _swappableTetrimino.name = shape + " " + SwappableName;
    }

    private void SetSwappedName(GameObject swapped)
    {
        swapped.name = swapped.name.RemoveContained(SwappableName);
        swapped.name = swapped.name.Split('(')[0].Trim();
    }

    #endregion
}
