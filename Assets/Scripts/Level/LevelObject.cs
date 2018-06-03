using System.Collections;
using UnityEngine;

namespace LevelControl
{
    public class LevelObject : MonoBehaviour
    {
        public LvlObjectType objType;

        public enum LvlObjectType
        {
            floor,
            obstacle,
            wall
        }
    }
}
