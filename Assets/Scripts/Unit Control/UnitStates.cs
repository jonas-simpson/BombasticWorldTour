using System.Collections;
using UnityEngine;

namespace UnitControl
{
    [RequireComponent(typeof(HandleAnimations))]
    public class UnitStates : MonoBehaviour
    {
        public int team;
        public int health;
        public bool selected;
        public bool hasPath;
        public bool move;

        public float maxSpeed = 6;
        public float movingSpeed;
    }
}
