
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace PPoory {

    [CustomEditor( typeof( uFSM ) )]
    [CanEditMultipleObjects]
    public class FsmEditor : Editor {

        private uFSM fsm;

        SerializedProperty ownerObjProp;
        SerializedProperty eventListProp;
        ReorderableList reorderEventList;
        Dictionary<string, ReorderableList> stateTransitionDic = new Dictionary<string, ReorderableList>();
        
        SerializedProperty infoMessage;

        //--------------------------------------------------
        void OnEnable () {

            fsm = (uFSM)target;

            if ( fsm.msgDebug ) {
                Debug.Log( string.Format( "{0}.{1}.{2}", name, GetType().Name, MethodBase.GetCurrentMethod().Name ) );
            }

            fsm.ReorderStateList();
            fsm.ReorderStateTransitionList();

            serializedObject.Update();

            ownerObjProp = serializedObject.FindProperty( "ownerObj" );
            eventListProp = serializedObject.FindProperty( "eventList" );
            reorderEventList = MakeEventList( eventListProp );

            infoMessage = serializedObject.FindProperty( "infoMessage" );

            MakeStateTransitionList();
        }

        //--------------------------------------------------
        public override void OnInspectorGUI () {

            if ( fsm.msgDebug ) {
                Debug.Log( string.Format( "{0}.{1}.{2}", name, GetType().Name, MethodBase.GetCurrentMethod().Name ) );
            }

            fsm.ReorderStateList();
            fsm.ReorderStateTransitionList();
            ReorderstateTransitionList();

            serializedObject.Update();

            EditorGUILayout.Space();

            // Owner GameObject
            EditorGUILayout.PropertyField( ownerObjProp );

            EditorGUILayout.Space();

            reorderEventList.DoLayoutList();

            EditorGUILayout.Space();

            InitStateGUI();
            CurrStateGUI();

            foreach ( KeyValuePair<string, ReorderableList> pair in stateTransitionDic ) {
                EditorGUILayout.Space();
                pair.Value.DoLayoutList();
            }

            EditorGUILayout.PropertyField( infoMessage );

            serializedObject.ApplyModifiedProperties();
        }

        //--------------------------------------------------
        ReorderableList MakeEventList ( SerializedProperty seriProplist ) {

            ReorderableList reorderList = new ReorderableList( serializedObject, seriProplist );

            reorderList.drawHeaderCallback = ( rect ) => {
                EditorGUI.LabelField( rect, seriProplist.displayName );
            };

            reorderList.drawElementCallback = ( rect, index, isActive, isFocused ) => {
                SerializedProperty element = seriProplist.GetArrayElementAtIndex( index );
                rect.height -= 4;
                rect.y += 2;
                EditorGUI.PropertyField( rect, element, new GUIContent() );
            };

            reorderList.onAddCallback += ( list ) => {
                seriProplist.arraySize++;
                list.index = seriProplist.arraySize - 1;
                SerializedProperty element = seriProplist.GetArrayElementAtIndex( list.index );
                element.stringValue = "New Event " + list.index;
            };

            return reorderList;
        }

        //--------------------------------------------------
        void MakeStateTransitionList () {

            SerializedProperty prop = serializedObject.FindProperty( "stateTransitionList" );

            stateTransitionDic.Clear();

            for ( int i = 0; i < prop.arraySize; i++ ) {
                string stateName = "";
                ReorderableList reorderList = MakeTransitionList( prop.GetArrayElementAtIndex( i ), out stateName );
                stateTransitionDic.Add( stateName, reorderList );
            }
        }

        //--------------------------------------------------
        void ReorderstateTransitionList () {

            string removeKey = "";

            foreach ( KeyValuePair<string, ReorderableList> pair in stateTransitionDic ) {
                bool found = false;
                foreach ( StateTransition trans in fsm.stateTransitionList ) {
                    if ( pair.Key == trans.state.GetStateName() ) {
                        found = true;
                    }
                }
                if ( false == found ) {
                    removeKey = pair.Key;
                }
            }

            stateTransitionDic.Remove( removeKey );
        }

        //--------------------------------------------------
        ReorderableList MakeTransitionList ( SerializedProperty transition, out string stateName ) {

            SerializedProperty stateProp = transition.FindPropertyRelative( "state" );
            stateName = ( (BaseState)stateProp.objectReferenceValue ).GetStateName();

            SerializedProperty transitionListProp = transition.FindPropertyRelative( "tansitionList" );

            ReorderableList reorderList = new ReorderableList( serializedObject, transitionListProp );

            // Setting State Transition Name
            reorderList.drawHeaderCallback = ( rect ) => {
                string header = "[ " + ( (BaseState)stateProp.objectReferenceValue ).GetStateName() + " ] Transition";
                EditorGUI.LabelField( rect, header );
            };

            // Setting Draw Element
            reorderList.drawElementCallback = ( rect, index, isActive, isFocused ) => {

                SerializedProperty element = transitionListProp.GetArrayElementAtIndex( index );
                SerializedProperty eventNameProp = element.FindPropertyRelative( "eventName" );
                SerializedProperty targetStateProp = element.FindPropertyRelative( "targetState" );

                // Event
                Rect rectEvent = rect;
                rectEvent.height -= 4;
                rectEvent.y += 2;
                rectEvent.width = ( rectEvent.width * 0.4f );

                List<string> options = fsm.eventList.ToList();
                options.Add( "None" );

                if ( options.Count == 1 ) {
                    EditorGUI.Popup( rectEvent, 0, options.ToArray() );
                    eventNameProp.stringValue = options[0];
                }
                else {

                    if ( string.IsNullOrEmpty( eventNameProp.stringValue ) ) {
                        eventNameProp.stringValue = "None";
                    }

                    int i = 0;

                    for ( i = 0; i < options.Count; i++ ) {
                        if ( options[i] == eventNameProp.stringValue ) {
                            break;
                        }
                    }

                    if ( i < options.Count ) {
                        i = EditorGUI.Popup( rectEvent, i, options.ToArray() );
                        eventNameProp.stringValue = options[i];
                    }
                    else {
                        EditorGUI.Popup( rectEvent, options.Count-1, options.ToArray() );
                        eventNameProp.stringValue = options[options.Count-1];
                    }
                }
                
                // Tartget State
                float offset = 5;

                Rect rectState = rect;
                rectState.height -= 4;
                rectState.y += 2;
                rectState.width = ( rectState.width - rectEvent.width - offset );
                rectState.x += rectEvent.width + offset;

                List<string> stateNameList = new List<string>();
                foreach ( BaseState state in fsm.stateList ) {
                    stateNameList.Add( state.GetStateName() );
                }

                string[] stateOptions = stateNameList.ToArray();

                int k = 0;
                for ( k = 0; k < stateOptions.Length; k++ ) {
                    if ( null == targetStateProp ) {
                        continue;
                    }
                    if ( null == targetStateProp.objectReferenceValue ) {
                        continue;
                    }
                    if ( stateOptions[k] == ((BaseState)targetStateProp.objectReferenceValue).GetStateName() ) {
                        break;
                    }
                }

                if ( k >= stateOptions.Length ) {
                    k = 0;
                }

                k = EditorGUI.Popup( rectState, k, stateOptions );
                targetStateProp.objectReferenceValue = fsm.stateList[k];

            };

            return reorderList;
        }

        

        //--------------------------------------------------
        private void InitStateGUI () {

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField( "Init State", GUILayout.Width( 100 ) );

            if ( fsm.stateList.Count == 0 ) {
                string[] ops = { "none" };
                EditorGUILayout.Popup( 0, ops );
            }
            else {

                List<string> stateNameList = new List<string>();
                foreach ( BaseState state in fsm.stateList ) {
                    stateNameList.Add( state.GetStateName() );
                }

                string[] stateOptions = stateNameList.ToArray();

                int k = 0;

                if ( null == fsm.initState ) {
                    k = 0;
                }
                else {
                    for ( k = 0; k < stateOptions.Length; k++ ) {
                        if ( stateOptions[k] == fsm.initState.GetStateName() ) {
                            break;
                        }
                    }
                }

                if ( k > stateOptions.Length ) {
                    Debug.LogError( "out of case" );
                }

                k = EditorGUILayout.Popup( k, stateOptions );

                fsm.initState = fsm.stateList[k];
            }

            EditorGUILayout.EndHorizontal();
        }

        //--------------------------------------------------
        private void CurrStateGUI () {

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField( "Current State", GUILayout.Width( 100 ) );

            string currStateName = "[ None ]";
            if ( null != fsm.currState ) {
                currStateName = "[ " + fsm.currState.GetStateName() + " ]";
            }
            EditorGUILayout.LabelField( currStateName );

            EditorGUILayout.EndHorizontal();
        }
        
    }

}


