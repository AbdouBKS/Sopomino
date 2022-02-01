using System.Collections.Generic;
using UnityEngine;

public class TetriminoManager : StaticInstance<TetriminoManager>
{
    [SerializeField]
    private List<Tetrimino> allTetriminos;

    private List<Tetrimino> nextTetrimios;
    private Tetrimino _newTetrimino;

    [SerializeField]
    private GameObject parent;

    private const int BUFFER_SIZE = 4;

    private void Start()
    {
        nextTetrimios = new List<Tetrimino>(BUFFER_SIZE);

        AddTetriminoToBuffer(BUFFER_SIZE);
        SpawnTetrimino();
    }

    private void OnEnable() {
        Tetrimino.OnFalled += SpawnTetrimino;
    }

    private void OnDisable() {
        Tetrimino.OnFalled -= SpawnTetrimino;
    }

    public void SpawnTetrimino() {
        _newTetrimino = nextTetrimios[0];

        nextTetrimios.Remove(_newTetrimino);
        AddTetriminoToBuffer();

        Instantiate(_newTetrimino, transform.position, Quaternion.identity, parent.transform);

    }

    private void AddTetriminoToBuffer(int number = 1)
    {
        Tetrimino newTetrimino = null;

        for (int i = 0; i < number; i++)
        {
            int rand = Random.Range(0, allTetriminos.Count);
            newTetrimino = allTetriminos[rand];
            nextTetrimios.Add(newTetrimino);
        }
    }

}
