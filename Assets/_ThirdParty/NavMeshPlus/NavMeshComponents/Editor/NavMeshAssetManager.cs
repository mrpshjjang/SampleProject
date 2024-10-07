using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.AI;
using UnityEngine;
using NavMeshPlus.Extensions;
using UnityEditor;
using NavMeshPlus.Components;

namespace NavMeshPlus.Editors.Components
{
    public class NavMeshAssetManager : ScriptableSingleton<NavMeshAssetManager>
    {
        internal struct AsyncBakeOperation
        {
            public NavMeshSurface2D Surface2D;
            public NavMeshData bakeData;
            public AsyncOperation bakeOperation;
        }

        List<AsyncBakeOperation> m_BakeOperations = new List<AsyncBakeOperation>();
        internal List<AsyncBakeOperation> GetBakeOperations() { return m_BakeOperations; }

        struct SavedPrefabNavMeshData
        {
            public NavMeshSurface2D Surface2D;
            public NavMeshData navMeshData;
        }

        List<SavedPrefabNavMeshData> m_PrefabNavMeshDataAssets = new List<SavedPrefabNavMeshData>();

        static string GetAndEnsureTargetPath(NavMeshSurface2D surface2D)
        {
            // Create directory for the asset if it does not exist yet.
            var activeScenePath = surface2D.gameObject.scene.path;

            var targetPath = "Assets";
            if (!string.IsNullOrEmpty(activeScenePath))
            {
                targetPath = Path.Combine(Path.GetDirectoryName(activeScenePath), Path.GetFileNameWithoutExtension(activeScenePath));
            }
            else
            {
                var prefabStage = PrefabStageUtility.GetPrefabStage(surface2D.gameObject);
                var isPartOfPrefab = prefabStage != null && prefabStage.IsPartOfPrefabContents(surface2D.gameObject);

                if (isPartOfPrefab)
                {
#if UNITY_2020_1_OR_NEWER
                    var assetPath = prefabStage.assetPath;
#else
                    var assetPath = prefabStage.prefabAssetPath;
#endif
                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        var prefabDirectoryName = Path.GetDirectoryName(assetPath);
                        if (!string.IsNullOrEmpty(prefabDirectoryName))
                            targetPath = prefabDirectoryName;
                    }
                }
            }
            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);
            return targetPath;
        }

        static void CreateNavMeshAsset(NavMeshSurface2D surface2D)
        {
            var targetPath = GetAndEnsureTargetPath(surface2D);

            var combinedAssetPath = Path.Combine(targetPath, "NavMesh-" + surface2D.name + ".asset");
            combinedAssetPath = AssetDatabase.GenerateUniqueAssetPath(combinedAssetPath);
            AssetDatabase.CreateAsset(surface2D.navMeshData, combinedAssetPath);
        }

        NavMeshData GetNavMeshAssetToDelete(NavMeshSurface2D navSurface2D)
        {
            if (PrefabUtility.IsPartOfPrefabInstance(navSurface2D) && !PrefabUtility.IsPartOfModelPrefab(navSurface2D))
            {
                // Don't allow deleting the asset belonging to the prefab parent
                var parentSurface = PrefabUtility.GetCorrespondingObjectFromSource(navSurface2D) as NavMeshSurface2D;
                if (parentSurface && navSurface2D.navMeshData == parentSurface.navMeshData)
                    return null;
            }

            // Do not delete the NavMeshData asset referenced from a prefab until the prefab is saved
            var prefabStage = PrefabStageUtility.GetPrefabStage(navSurface2D.gameObject);
            var isPartOfPrefab = prefabStage != null && prefabStage.IsPartOfPrefabContents(navSurface2D.gameObject);
            if (isPartOfPrefab && IsCurrentPrefabNavMeshDataStored(navSurface2D))
                return null;

            return navSurface2D.navMeshData;
        }

        void ClearSurface(NavMeshSurface2D navSurface2D)
        {
            var hasNavMeshData = navSurface2D.navMeshData != null;
            StoreNavMeshDataIfInPrefab(navSurface2D);

            var assetToDelete = GetNavMeshAssetToDelete(navSurface2D);
            navSurface2D.RemoveData();

            if (hasNavMeshData)
            {
                SetNavMeshData(navSurface2D, null);
                EditorSceneManager.MarkSceneDirty(navSurface2D.gameObject.scene);
            }

            if (assetToDelete)
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(assetToDelete));
        }

        public void StartBakingSurfaces(UnityEngine.Object[] surfaces)
        {
            // Remove first to avoid double registration of the callback
            EditorApplication.update -= UpdateAsyncBuildOperations;
            EditorApplication.update += UpdateAsyncBuildOperations;

            foreach (NavMeshSurface2D surf in surfaces)
            {
                StoreNavMeshDataIfInPrefab(surf);

                var oper = new AsyncBakeOperation();

                oper.bakeData = InitializeBakeData(surf);
                oper.bakeOperation = surf.UpdateNavMesh(oper.bakeData);
                oper.Surface2D = surf;

                m_BakeOperations.Add(oper);
            }
        }

        static NavMeshData InitializeBakeData(NavMeshSurface2D surface2D)
        {
            var emptySources = new List<NavMeshBuildSource>();
            var emptyBounds = new Bounds();
            return UnityEngine.AI.NavMeshBuilder.BuildNavMeshData(surface2D.GetBuildSettings(), emptySources, emptyBounds
                , surface2D.transform.position, surface2D.transform.rotation);
        }

        void UpdateAsyncBuildOperations()
        {
            foreach (var oper in m_BakeOperations)
            {
                if (oper.Surface2D == null || oper.bakeOperation == null)
                    continue;

                if (oper.bakeOperation.isDone)
                {
                    var surface = oper.Surface2D;
                    var delete = GetNavMeshAssetToDelete(surface);
                    if (delete != null)
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(delete));

                    surface.RemoveData();
                    SetNavMeshData(surface, oper.bakeData);

                    if (surface.isActiveAndEnabled)
                        surface.AddData();
                    CreateNavMeshAsset(surface);
                    EditorSceneManager.MarkSceneDirty(surface.gameObject.scene);
                }
            }
            m_BakeOperations.RemoveAll(o => o.bakeOperation == null || o.bakeOperation.isDone);
            if (m_BakeOperations.Count == 0)
                EditorApplication.update -= UpdateAsyncBuildOperations;
        }

        public bool IsSurfaceBaking(NavMeshSurface2D surface2D)
        {
            if (surface2D == null)
                return false;

            foreach (var oper in m_BakeOperations)
            {
                if (oper.Surface2D == null || oper.bakeOperation == null)
                    continue;

                if (oper.Surface2D == surface2D)
                    return true;
            }

            return false;
        }

        public void ClearSurfaces(UnityEngine.Object[] surfaces)
        {
            foreach (NavMeshSurface2D s in surfaces)
                ClearSurface(s);
        }

        static void SetNavMeshData(NavMeshSurface2D navSurface2D, NavMeshData navMeshData)
        {
            var so = new SerializedObject(navSurface2D);
            var navMeshDataProperty = so.FindProperty("m_NavMeshData");
            navMeshDataProperty.objectReferenceValue = navMeshData;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        void StoreNavMeshDataIfInPrefab(NavMeshSurface2D surface2DToStore)
        {
            var prefabStage = PrefabStageUtility.GetPrefabStage(surface2DToStore.gameObject);
            var isPartOfPrefab = prefabStage != null && prefabStage.IsPartOfPrefabContents(surface2DToStore.gameObject);
            if (!isPartOfPrefab)
                return;

            // check if data has already been stored for this surface
            foreach (var storedAssetInfo in m_PrefabNavMeshDataAssets)
                if (storedAssetInfo.Surface2D == surface2DToStore)
                    return;

            if (m_PrefabNavMeshDataAssets.Count == 0)
            {
                PrefabStage.prefabSaving -= DeleteStoredNavMeshDataAssetsForOwnedSurfaces;
                PrefabStage.prefabSaving += DeleteStoredNavMeshDataAssetsForOwnedSurfaces;

                PrefabStage.prefabStageClosing -= ForgetUnsavedNavMeshDataChanges;
                PrefabStage.prefabStageClosing += ForgetUnsavedNavMeshDataChanges;
            }

            var isDataOwner = true;
            if (PrefabUtility.IsPartOfPrefabInstance(surface2DToStore) && !PrefabUtility.IsPartOfModelPrefab(surface2DToStore))
            {
                var basePrefabSurface = PrefabUtility.GetCorrespondingObjectFromSource(surface2DToStore) as NavMeshSurface2D;
                isDataOwner = basePrefabSurface == null || surface2DToStore.navMeshData != basePrefabSurface.navMeshData;
            }
            m_PrefabNavMeshDataAssets.Add(new SavedPrefabNavMeshData { Surface2D = surface2DToStore, navMeshData = isDataOwner ? surface2DToStore.navMeshData : null });
        }

        bool IsCurrentPrefabNavMeshDataStored(NavMeshSurface2D surface2D)
        {
            if (surface2D == null)
                return false;

            foreach (var storedAssetInfo in m_PrefabNavMeshDataAssets)
            {
                if (storedAssetInfo.Surface2D == surface2D)
                    return storedAssetInfo.navMeshData == surface2D.navMeshData;
            }

            return false;
        }

        void DeleteStoredNavMeshDataAssetsForOwnedSurfaces(GameObject gameObjectInPrefab)
        {
            // Debug.LogFormat("DeleteStoredNavMeshDataAsset() when saving prefab {0}", gameObjectInPrefab.name);

            var surfaces = gameObjectInPrefab.GetComponentsInChildren<NavMeshSurface2D>(true);
            foreach (var surface in surfaces)
                DeleteStoredPrefabNavMeshDataAsset(surface);
        }

        void DeleteStoredPrefabNavMeshDataAsset(NavMeshSurface2D surface2D)
        {
            for (var i = m_PrefabNavMeshDataAssets.Count - 1; i >= 0; i--)
            {
                var storedAssetInfo = m_PrefabNavMeshDataAssets[i];
                if (storedAssetInfo.Surface2D == surface2D)
                {
                    var storedNavMeshData = storedAssetInfo.navMeshData;
                    if (storedNavMeshData != null && storedNavMeshData != surface2D.navMeshData)
                    {
                        var assetPath = AssetDatabase.GetAssetPath(storedNavMeshData);
                        AssetDatabase.DeleteAsset(assetPath);
                    }

                    m_PrefabNavMeshDataAssets.RemoveAt(i);
                    break;
                }
            }

            if (m_PrefabNavMeshDataAssets.Count == 0)
            {
                PrefabStage.prefabSaving -= DeleteStoredNavMeshDataAssetsForOwnedSurfaces;
                PrefabStage.prefabStageClosing -= ForgetUnsavedNavMeshDataChanges;
            }
        }

        void ForgetUnsavedNavMeshDataChanges(PrefabStage prefabStage)
        {
            // Debug.Log("On prefab closing - forget about this object's surfaces and stop caring about prefab saving");

            if (prefabStage == null)
                return;

            var allSurfacesInPrefab = prefabStage.prefabContentsRoot.GetComponentsInChildren<NavMeshSurface2D>(true);
            NavMeshSurface2D surface2DInPrefab = null;
            var index = 0;
            do
            {
                if (allSurfacesInPrefab.Length > 0)
                    surface2DInPrefab = allSurfacesInPrefab[index];

                for (var i = m_PrefabNavMeshDataAssets.Count - 1; i >= 0; i--)
                {
                    var storedPrefabInfo = m_PrefabNavMeshDataAssets[i];
                    if (storedPrefabInfo.Surface2D == null)
                    {
                        // Debug.LogFormat("A surface from the prefab got deleted after it has baked a new NavMesh but it hasn't saved it. Now the unsaved asset gets deleted. ({0})", storedPrefabInfo.navMeshData);

                        // surface got deleted, thus delete its initial NavMeshData asset
                        if (storedPrefabInfo.navMeshData != null)
                        {
                            var assetPath = AssetDatabase.GetAssetPath(storedPrefabInfo.navMeshData);
                            AssetDatabase.DeleteAsset(assetPath);
                        }

                        m_PrefabNavMeshDataAssets.RemoveAt(i);
                    }
                    else if (surface2DInPrefab != null && storedPrefabInfo.Surface2D == surface2DInPrefab)
                    {
                        //Debug.LogFormat("The surface {0} from the prefab was storing the original navmesh data and now will be forgotten", surfaceInPrefab);

                        var baseSurface = PrefabUtility.GetCorrespondingObjectFromSource(surface2DInPrefab) as NavMeshSurface2D;
                        if (baseSurface == null || surface2DInPrefab.navMeshData != baseSurface.navMeshData)
                        {
                            var assetPath = AssetDatabase.GetAssetPath(surface2DInPrefab.navMeshData);
                            AssetDatabase.DeleteAsset(assetPath);

                            //Debug.LogFormat("The surface {0} from the prefab has baked new NavMeshData but did not save this change so the asset has been now deleted. ({1})",
                            //    surfaceInPrefab, assetPath);
                        }

                        m_PrefabNavMeshDataAssets.RemoveAt(i);
                    }
                }
            } while (++index < allSurfacesInPrefab.Length);

            if (m_PrefabNavMeshDataAssets.Count == 0)
            {
                PrefabStage.prefabSaving -= DeleteStoredNavMeshDataAssetsForOwnedSurfaces;
                PrefabStage.prefabStageClosing -= ForgetUnsavedNavMeshDataChanges;
            }
        }
    }
}
