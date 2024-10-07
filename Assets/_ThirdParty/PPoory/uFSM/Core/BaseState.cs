
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;

namespace PPoory {

    [System.Serializable]
    [ExecuteInEditMode]
    public abstract class BaseState : MonoBehaviour {

        [HideInInspector]
        public uFSM fsm;

        //--------------------------------------------------
        void Reset () {

            fsm = GetComponent<uFSM>();

            if ( null != fsm ) {

                if ( null == fsm.initState ) {
                    fsm.initState = this;
                }

                // stateList

                fsm.ReorderStateList();

                if ( false == fsm.stateList.Contains( this ) ) {
                    fsm.stateList.Add( this );
                }

                // tansitionList

                fsm.ReorderStateTransitionList();

                bool found = false;
                foreach ( StateTransition stateTransition in fsm.stateTransitionList ) {
                    if ( stateTransition.state == this ) {
                        found = false;
                    }
                }

                if ( false == found ) {
                    StateTransition stateTransition = new StateTransition();
                    stateTransition.state = this;
                    fsm.stateTransitionList.Add( stateTransition );
                }
            }
        }

        //--------------------------------------------------
        protected virtual void Awake () {

            this.enabled = false;
        }

        //--------------------------------------------------
        void OnDestroy () {

            //Debug.Log( string.Format( "{0}.{1}.{2}",
            //    name, GetType().Name, MethodBase.GetCurrentMethod().Name ) );

            uFSM fsm = GetComponent<uFSM>();

            if ( null != fsm ) {

                fsm.stateList.Remove( this );
                fsm.ReorderStateList();

                for ( int i = 0; i < fsm.stateTransitionList.Count; i++ ) {
                    if ( fsm.stateTransitionList[i].state == this ) {
                        fsm.stateTransitionList.RemoveAt( i );
                    }
                }

                if ( this == fsm.initState ) {
                    if ( 0 < fsm.stateList.Count ) {
                        fsm.initState = fsm.stateList[0];
                    }
                    else {
                        fsm.initState = null;
                    }
                }
            }
        }

        //--------------------------------------------------
        public virtual void OnEnter () {
            this.enabled = true;
        }

        //--------------------------------------------------
        public virtual void OnExit () {
            this.enabled = false;
        }

        //--------------------------------------------------
        public abstract string GetStateName ();
    }

}

