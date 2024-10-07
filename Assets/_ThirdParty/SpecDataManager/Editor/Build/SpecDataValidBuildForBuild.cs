using Sample.SpecData.Editor.Asset;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Sample.SpecData.Editor.Build
{
    /// <summary>
    /// 빌드 전 검증
    /// </summary>
    public class SpecDataValidBuildForBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => int.MinValue;
        public void OnPreprocessBuild(BuildReport report)
        {
            SpecDataAsset asset = SpecDataAsset.GetAssets();
            if (asset == null || !asset.PreBuildValid)
            {
                return;
            }

            // 검증 실행
            if (!SpecDataValidator.Valid())
            {
                // 검증 실패시 Build 취소
                throw new BuildFailedException("SpecData 검증 실패");
            }
        }
    }
}
