using UnityEngine;

public enum TileContentType
{
    None,
    Path,
    Course,
    CourseStart,
    CourseEnd_Part1,
    CourseEnd_Part2,
    Home
}

public class PlaceableObjectData : MonoBehaviour
{
    public TileContentType type;
}