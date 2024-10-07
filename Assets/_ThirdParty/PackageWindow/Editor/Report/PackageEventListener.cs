using UnityEditor;
using UnityEditor.PackageManager;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;


namespace CookApps.Package.Report.Editor
{
    [InitializeOnLoad]
    internal class PackageEventListener
    {
        //--------------------------------------------------------------------------------//
        //-----------------------------------FIELD----------------------------------------//
        //--------------------------------------------------------------------------------//
        //------------------- Inspector ------------------//

        //------------------- public ------------------//

        //------------------- protected ------------------//

        //------------------- private ------------------//

        //--------------------------------------------------------------------------------//
        //------------------------------------PROPERTY------------------------------------//
        //--------------------------------------------------------------------------------//

        //--------------------------------------------------------------------------------//
        //------------------------------------METHOD--------------------------------------//
        //--------------------------------------------------------------------------------//
        //───────────────────────────────────────────────────────────────────────────────────────
        static PackageEventListener()
        {
            string bundleVersion = PlayerSettings.bundleVersion;
            if (bundleVersion.Equals("0.1")
                || bundleVersion.Equals("0.1.0"))
            {
                return;
            }

            if (PackagesScriptable.instance.IsCheckAllPackages == false)
            {
                CheckAllInstalledPackages();
            }

            Events.registeredPackages -= OnRegisteredPackages;
            Events.registeredPackages += OnRegisteredPackages;
        }

        private static async void CheckAllInstalledPackages()
        {
            await GoogleSheetInternal.Initialize();
            await GoogleSheetInternal.CheckAllInstalledPackages();
            TechPackagesReporter.ReportCells(GoogleSheetInternal.SheetDatas);
            TechPackagesReporter.ReportNotes(GoogleSheetInternal.ProjectName, GoogleSheetInternal.Notes);

            PackagesScriptable.instance.Check();
        }

        private static async void OnRegisteredPackages(PackageRegistrationEventArgs args)
        {
            await GoogleSheetInternal.Initialize();

            foreach (PackageInfo package in args.added)
            {
                if (IsTechPackage(package.name, package.author.name))
                {
                    string packageName = GetMinimalPackageName(package.name);
                    GoogleSheetInternal.AddCellData(packageName, package.version);
                    // TechPackagesReporter.Added(PlayerSettings.productName, package.name, package.version);
                }
            }

            foreach (PackageInfo package in args.removed)
            {
                if (IsTechPackage(package.name, package.author.name))
                {
                    string packageName = GetMinimalPackageName(package.name);
                    GoogleSheetInternal.RemoveCellData(packageName, package.version);
                    // TechPackagesReporter.Removed(PlayerSettings.productName, package.name, package.version);
                }
            }

            foreach (PackageInfo package in args.changedTo)
            {
                if (IsTechPackage(package.name, package.author.name))
                {
                    string packageName = GetMinimalPackageName(package.name);
                    GoogleSheetInternal.UpdateCellData(packageName, package.version);
                    // TechPackagesReporter.Updated(PlayerSettings.productName, package.name, package.version);
                }
            }

            TechPackagesReporter.ReportCells(GoogleSheetInternal.SheetDatas);
            TechPackagesReporter.ReportNotes(GoogleSheetInternal.ProjectName, GoogleSheetInternal.Notes);
        }

        internal static string GetMinimalPackageName(string packageName)
        {
            // packageName = packageName.Replace("CookApps", ""); //CookApps글자 제거
            // packageName = Regex.Replace(packageName, @"\s+", ""); //공백 제거
            packageName = packageName.Replace("com.cookapps.", "");
            return packageName;
        }

        private static bool IsTechPackage(string name, string author)
        {
            return name.Contains("com.cookapps") && author.Equals("CookApps");
        }
    }
}
