using System.Collections;
using UnityEngine;

namespace UnitControl
{
    public class HandleAnimations : MonoBehaviour
    {
        Animator anim;
        UnitStates states;

        private void Start()
        {
            states = GetComponent<UnitStates>();

            SetupAnimator();
        }

        private void Update()
        {
            anim.SetFloat("Movement", (states.move) ? 1 : 0, 0.1f, Time.deltaTime);

            states.movingSpeed = anim.GetFloat("Movement") * states.maxSpeed;
        }

        void SetupAnimator()
        {
            anim = GetComponent<Animator>();

            Animator[] a = GetComponentsInChildren<Animator>();

            for (int i = 0; i < a.Length; i++)
            {
                if(a[i] != anim)
                {
                    anim.avatar = a[i].avatar;
                    Destroy(a[i]);
                    break;
                }
            }
        }
    }
}
