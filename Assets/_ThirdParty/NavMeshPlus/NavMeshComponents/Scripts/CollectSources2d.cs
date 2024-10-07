using NavMeshPlus.Components;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

namespace NavMeshPlus.Extensions
{
    [ExecuteAlways]
    [AddComponentMenu("Navigation/Navigation CollectSources2d", 30)]
    public class CollectSources2d: NavMeshExtension
    {
        [SerializeField]
        bool m_OverrideByGrid;
        public bool overrideByGrid { get { return m_OverrideByGrid; } set { m_OverrideByGrid = value; } }

        [SerializeField]
        GameObject m_UseMeshPrefab;
        public GameObject useMeshPrefab { get { return m_UseMeshPrefab; } set { m_UseMeshPrefab = value; } }

        [SerializeField]
        bool m_CompressBounds;
        public bool compressBounds { get { return m_CompressBounds; } set { m_CompressBounds = value; } }

        [SerializeField]
        Vector3 m_OverrideVector = Vector3.one;
        public Vector3 overrideVector { get { return m_OverrideVector; } set { m_OverrideVector = value; } }

        public override void CalculateWorldBounds(NavMeshSurface2D surface2D, List<NavMeshBuildSource> sources, NavMeshBuilderState navNeshState)
        {
            if (surface2D.collectObjects != CollectObjects.Volume)
            {
                navNeshState.worldBounds.Encapsulate(CalculateGridWorldBounds(surface2D, navNeshState.worldToLocal, navNeshState.worldBounds));
            }
        }

        private static Bounds CalculateGridWorldBounds(NavMeshSurface2D surface2D, Matrix4x4 worldToLocal, Bounds bounds)
        {
            var grid = FindObjectOfType<Grid>();
            var tilemaps = grid?.GetComponentsInChildren<Tilemap>();
            if (tilemaps == null || tilemaps.Length < 1)
            {
                return bounds;
            }
            foreach (var tilemap in tilemaps)
            {
                var lbounds = NavMeshSurface2D.GetWorldBounds(worldToLocal * tilemap.transform.localToWorldMatrix, tilemap.localBounds);
                bounds.Encapsulate(lbounds);
                if (!surface2D.hideEditorLogs)
                {
                    Debug.Log($"From Local Bounds [{tilemap.name}]: {tilemap.localBounds}");
                    Debug.Log($"To World Bounds: {bounds}");
                }
            }
            return bounds;
        }

        public override void CollectSources(NavMeshSurface2D surface2D, List<NavMeshBuildSource> sources, NavMeshBuilderState navNeshState)
        {
            if (!surface2D.hideEditorLogs)
            {
                if (!Mathf.Approximately(transform.eulerAngles.x, 270f))
                {
                    Debug.LogWarning("NavMeshSurface is not rotated respectively to (x-90;y0;z0). Apply rotation unless intended.");
                }
                if (Application.isPlaying)
                {
                    if (surface2D.useGeometry == NavMeshCollectGeometry.PhysicsColliders && Time.frameCount <= 1)
                    {
                        Debug.LogWarning("Use Geometry - Physics Colliders option in NavMeshSurface may cause inaccurate mesh bake if executed before Physics update.");
                    }
                }
            }
            var builder = navNeshState.GetExtraState<NavMeshBuilder2dState>();
            builder.defaultArea = surface2D.defaultArea;
            builder.layerMask = surface2D.layerMask;
            builder.agentID = surface2D.agentTypeID;
            builder.useMeshPrefab = useMeshPrefab;
            builder.overrideByGrid = overrideByGrid;
            builder.compressBounds = compressBounds;
            builder.overrideVector = overrideVector;
            builder.CollectGeometry = surface2D.useGeometry;
            builder.CollectObjects = (CollectObjects)(int)surface2D.collectObjects;
            builder.parent = surface2D.gameObject;
            builder.hideEditorLogs = surface2D.hideEditorLogs;
            builder.SetRoot(navNeshState.roots);
            NavMeshBuilder2d.CollectSources(sources, builder);
        }
    }
}
