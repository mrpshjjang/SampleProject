using NavMeshPlus.Components;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace NavMeshPlus.Extensions
{
    public abstract class NavMeshExtension: MonoBehaviour
    {
        public int Order { get; protected set; }
        public virtual void CollectSources(NavMeshSurface2D surface2D, List<NavMeshBuildSource> sources, NavMeshBuilderState navNeshState) { }
        public virtual void CalculateWorldBounds(NavMeshSurface2D surface2D, List<NavMeshBuildSource> sources, NavMeshBuilderState navNeshState) { }
        public virtual void PostCollectSources(NavMeshSurface2D surface2D, List<NavMeshBuildSource> sources, NavMeshBuilderState navNeshState) { }
        public NavMeshSurface2D NavMeshSurface2DOwner
        {
            get
            {
                if (m_navMeshOwner == null)
                    m_navMeshOwner = GetComponent<NavMeshSurface2D>();
                return m_navMeshOwner;
            }
        }
        NavMeshSurface2D m_navMeshOwner;

        protected virtual void Awake()
        {
            ConnectToVcam(true);
        }
#if UNITY_EDITOR
        [UnityEditor.Callbacks.DidReloadScripts]
        static void OnScriptReload()
        {
            var extensions = Resources.FindObjectsOfTypeAll(
                typeof(NavMeshExtension)) as NavMeshExtension[];
            foreach (var e in extensions)
                e.ConnectToVcam(true);
        }
#endif
        protected virtual void OnEnable() { }
        protected virtual void OnDestroy()
        {
            ConnectToVcam(false);
        }
        protected virtual void ConnectToVcam(bool connect)
        {
            if (connect && NavMeshSurface2DOwner == null)
                Debug.LogError("NevMeshExtension requires a NavMeshSurface component");
            if (NavMeshSurface2DOwner != null)
            {
                if (connect)
                    NavMeshSurface2DOwner.NevMeshExtensions.Add(this, Order);
                else
                    NavMeshSurface2DOwner.NevMeshExtensions.Remove(this);
            }
        }
    }
}
