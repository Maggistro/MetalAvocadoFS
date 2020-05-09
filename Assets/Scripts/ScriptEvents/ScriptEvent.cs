using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Avocado
{
    abstract class ScriptEvent : MonoBehaviour
    {
        [Serializable]
        public struct Step
        {
            public ScriptEventType type;
            public Transform target;
            public float waitTime;
        }

        public List<Step> targets;

        protected abstract bool disableOnStart { get; set; }
        protected abstract ScriptEventType lastEvent  { get; set; }

        protected JesterController jester;
        protected CharacterController character;

        private bool switchingTarget = false;

        void Awake()
        {
            jester = FindObjectOfType<JesterController>();
            character = FindObjectOfType<CharacterController>();
            character.SetActivestate = disableOnStart;
        }

        void Update()
        {
            if (NextTargetCondition()) {
                if (!switchingTarget) {
                    switchingTarget = true;
                    StartCoroutine("SelectNextTarget");
                }
            } else {
                AnimateScriptEventTransition();
            }
        }


        void UpdateJesterMovement()
        {
            Transform target = targets.First().target;
            this.jester.movementDirection = target.position - jester.transform.position;
        }


        protected IEnumerator SelectNextTarget()
        {
            ExecuteEvent(targets.First().type);
            yield return new WaitForSeconds(targets.First().waitTime);
            lastEvent = targets.First().type;
            targets.Remove(targets.First());
            switchingTarget = false;
        }

        protected abstract void AnimateScriptEventTransition();
        protected abstract bool NextTargetCondition();
        protected abstract void ExecuteEvent(ScriptEventType type);
    }
}