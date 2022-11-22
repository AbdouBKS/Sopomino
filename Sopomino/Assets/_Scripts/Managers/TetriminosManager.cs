using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[RequireComponent(typeof(GridManager))]
public class TetriminosManager : StaticInstance<TetriminosManager>
{

    #region Fields

    [SerializeField]
    private List<Tetrimino> _tetriminos;

    [SerializeField]
    private GameObject _environment;

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

    public delegate void SwappableAction(string swappableName);
    public static event SwappableAction OnSwappableChange;

    public delegate void TetriminoBufferAction();
    public static event TetriminoBufferAction OnTetriminoBufferChange;

    private const string SWAPPABLE_NAME = "Swappable";

    private const int BUFFER_SIZE = 4;

    private bool _isDead;

    #endregion

    #region Constants

    private const int MAX_PUNISHMENT_COUNT = 20;

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
        _nextTetriminos = new List<Tetrimino>(BUFFER_SIZE);
        _previewTetriminos = new Dictionary<string, Tetrimino>();
        _swappableTetrimino = null;
        _isDead = false;
        _canSwap = true;

        if (_environment == null) {
            _environment = GameObject.Find("Environment");
        }

        AddTetriminoToBuffer(BUFFER_SIZE);
        if (_previewTetriminos.Count != _tetriminos.Count) {
            CreatePreviewTetriminos();
        }
        SpawnTetrimino();

        void CreatePreviewTetriminos()
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

        if (!_currentTetrimino.ValidMove()) {
            Die();
            return;
        }

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
        Tetrimino newTetrimino = null;

        for (int i = 0; i < count; i++)
        {
            int rand = UnityEngine.Random.Range(0, _tetriminos.Count);
            newTetrimino = _tetriminos[rand];
            _nextTetriminos.Add(newTetrimino);
        }

        OnTetriminoBufferChange?.Invoke();
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
