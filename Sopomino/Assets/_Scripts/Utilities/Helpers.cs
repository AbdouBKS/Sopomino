using UnityEngine;

/// <summary>
/// A static class for general helpful methods
/// </summary>
public static class Helpers
{
    /// <summary>
    /// Destroy all child objects of this transform (Unintentionally evil sounding).
    /// Use it like so:
    /// <code>
    /// transform.DestroyChildren();
    /// </code>
    /// </summary>
    public static void DestroyChildren(this Transform t) {
        foreach (Transform child in t) Object.Destroy(child.gameObject);
    }

    /// <summary>
    /// Duplicate any type of class who herits from MonoBehaviour and return it.
    /// Use it like so:
    /// <code>
    /// objectToDuplicate.Duplicate()
    /// </code>
    /// </summary>
    public static T Duplicate<T>(this T original) where T : MonoBehaviour
    {
        T duplicated = Object.Instantiate(
            original,
            original.transform.position,
            Quaternion.identity,
            original.transform.parent
        );

        return duplicated;
    }

    /// <summary>
    /// Duplicate a GameObject and return it.
    /// Use it like so:
    /// <code>
    /// gameObject.Duplicate()
    /// </code>
    /// </summary>
    public static GameObject Duplicate(this GameObject original)
    {
        GameObject duplicated = Object.Instantiate(
            original,
            original.transform.position,
            Quaternion.identity,
            original.transform.parent
        );

        return duplicated;
    }

    public static  bool ValidMove(this Tetrimino tetrimino)
    {
        foreach (Transform children in tetrimino.transform)
        {
            int roundedX = Mathf.RoundToInt(children.position.x);
            int roundedY = Mathf.RoundToInt(children.position.y);

            if (roundedX < 0 || roundedX >= TetriminosManager.MAP_WIDTH || roundedY < 0 || roundedY >= TetriminosManager.MAP_HEIGHT) {
                return false;
            }

            if (TetriminosManager.Instance.Grid[roundedX, roundedY] != null) {
                return false;
            }
        }

        return true;
    }

    public static string RemoveContained(this string source, string removeString)
    {
        int index = source.IndexOf(removeString);
        string dest = "";

        dest = (index < 0)
            ? source
            : source.Remove(index, removeString.Length);

        return dest;
    }
}
