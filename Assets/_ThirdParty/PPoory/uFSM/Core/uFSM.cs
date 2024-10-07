
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PPoory {

    [DisallowMultipleComponent]
    public class uFSM : MonoBehaviour {

        public bool infoMessage;

        [HideInInspector]
        public bool msgDebug = false;

        public GameObject ownerObj;

        public List<string> eventList = new List<string>();

        public List<BaseState> stateList = new List<BaseState>();

        public List<StateTransition> stateTransitionList = new List<StateTransition>();

        public BaseState initState;
        public BaseState currState;

        //--------------------------------------------------
        void Reset () {

            if ( msgDebug ) {
                Debug.Log( string.Format( "{0}.{1}.{2}", name, GetType().Name, MethodBase.GetCurrentMethod().Name ) );
            }

            ownerObj = gameObject;

            // BaseState List
            BaseState[] stateComArray = GetComponents<BaseState>();
            stateList = stateComArray.ToList();

            foreach ( BaseState state in stateComArray ) {
                state.fsm = this;
            }

            // Transition List
            int cnt = stateComArray.Length;
            for ( int i = 0; i < cnt; i++ ) {
                StateTransition stateTransition = new StateTransition();
                stateTransition.state = stateComArray[i];
                stateTransitionList.Add( stateTransition );
            }

            // Event List
            eventList.Add( "finish" );

            // Init BaseState
            if ( stateList.Count > 0 ) {
                initState = stateList[0];
            }
        }

        //--------------------------------------------------
        void Awake () {

            if ( msgDebug ) {
                Debug.Log( string.Format( "{0}.{1}.{2}", name, GetType().Name, MethodBase.GetCurrentMethod().Name ) );
            }

            currState = initState;
        }

        //--------------------------------------------------
        // void Start () {
        //
        //     if ( msgDebug ) {
        //         Debug.Log( string.Format( "{0}.{1}.{2}", name, GetType().Name, MethodBase.GetCurrentMethod().Name ) );
        //     }
        //
        //     currState.OnEnter();
        // }

        //--------------------------------------------------
        void ChangeState ( BaseState nextState ) {

            if ( infoMessage ) {
                Debug.Log( string.Format( "{0}.{1}.{2}", name, GetType().Name, MethodBase.GetCurrentMethod().Name ) );
            }

            if ( null == nextState ) {
                Debug.LogWarning( "initState is null" );
                return;
            }

            if ( null != currState ) {
                currState.OnExit();
            }

            currState = nextState;
            currState.OnEnter();
        }

        //--------------------------------------------------
        public void Event ( string evt ) {

            if ( msgDebug ) {
                Debug.Log( string.Format( "{0}.{1}.{2}", name, GetType().Name, MethodBase.GetCurrentMethod().Name ) );
            }

            if ( infoMessage ) {
                Debug.Log( string.Format( "Event Call ===== Stage:{0}, Event:{1}", currState, evt ) );
            }

            // Check uFSM Event
            if ( false == eventList.Any( s => s.Contains( evt ) ) ) {
                Debug.LogWarning( string.Format( "A nonexistent Event was called. ( {0} )", evt ) );
                return;
            }

            // Check Transition
            StateTransition currStateTransition = null;
            foreach ( StateTransition stateTransition in stateTransitionList ) {
                if ( stateTransition.state == currState ) {
                    currStateTransition = stateTransition;
                    break;
                }
            }
            if ( null == currStateTransition ) {
                Debug.LogWarning( string.Format( "[fsm] bad StateTransition. BaseState:{0}, Event{1}", currState, evt ) );
                return;
            }

            // Check Transition Event
            BaseState targetState = null;
            foreach ( StateTransition.Tansition transition in currStateTransition.tansitionList ) {
                if ( evt == transition.eventName ) {
                    targetState = transition.targetState;
                }
            }
            if ( null == targetState ) {
                if ( infoMessage ) {
                    Debug.Log( string.Format( "[{0}] Tansition has no [{1}] event.", currState.GetStateName(), evt ) );
                }
                return;
            }

            // Change State
            ChangeState( targetState );
        }

        //--------------------------------------------------
        public void ReorderStateList () {
            stateList = stateList.Where( e => e != null ).Distinct().ToList();
        }

        //--------------------------------------------------
        public void ReorderStateTransitionList () {
            stateTransitionList = stateTransitionList.Where( e => e.state != null ).Distinct().ToList();
        }

    }

}







