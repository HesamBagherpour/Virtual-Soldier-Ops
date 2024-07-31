using ArioSoren.PackageTemplateUtility.Editor.Validation.Logger;
using PackageManifest = ArioSoren.PackageTemplateUtility.Editor.Models.PackageManifestModel.PackageManifest;

namespace ArioSoren.PackageTemplateUtility.Editor.Validation
{
    public class ExternalStandardValidator : StandardValidator
    {
        private static ExternalStandardValidator _instance;
        
        #region CORE
        public static ExternalStandardValidator Instance()
        {
            return _instance ??= new ExternalStandardValidator();
        }
        #endregion CORE

        #region PACKAGE_JSON
        public override bool AllRequiredFieldsAreSet(PackageManifest deserializedPackageJson)
        {
            return base.AllRequiredFieldsAreSet(deserializedPackageJson) && ReferenceVersionIsValid(deserializedPackageJson);
        }

        public static bool ReferenceVersionIsValid(PackageManifest deserializedPackageManifest)
        {
            var referenceVersion = deserializedPackageManifest.ReferenceVersion;
            var version = deserializedPackageManifest.Version;
            var splitedReferenceVersion = referenceVersion.Split(".");
            var splitedVersion = version.Split(".");

            if (!CanParsToInt(splitedReferenceVersion[0], out var majorReferenceVersion, 
                "PackageJson-ReferenceVersion-Major")) return false;
            if (!CanParsToInt(splitedVersion[0], out var majorVersion, 
                "PackageJson-Version-Major")) return false;
            if (!CanParsToInt(splitedReferenceVersion[1], out var minorReferenceVersion, 
                "PackageJson-ReferenceVersion-Minor")) return false;
            if (!CanParsToInt(splitedVersion[1], out var minorVersion, 
                "PackageJson-Version-Minor")) return false;
            if (!CanParsToInt(splitedReferenceVersion[2], out var patchReferenceVersion, 
                "PackageJson-ReferenceVersion-Path")) return false;
            if (!CanParsToInt(splitedVersion[2], out var patchVersion, 
                "PackageJson-ReferenceVersion-Path")) return false;

            if (majorReferenceVersion > majorVersion) return false;
            if (majorReferenceVersion < majorVersion) return true;
            if (minorReferenceVersion > minorVersion) return false;
            if (minorReferenceVersion < minorVersion) return true;
            return patchReferenceVersion <= patchVersion;
        }

        protected static bool CanParsToInt(string reference, out int result, string referenceName)
        {
            if (int.TryParse(reference, out result))
            {
                _showPackageWindow.logWindow.AddNewLog(LogType.PASSED, $"{referenceName} sucssecfully parsed to integer.");
                return true;
            }
            
            _showPackageWindow.logWindow.AddNewLog(LogType.ERROR, $"Something is wrong in {referenceName}. The machine can not pars it to int.");
            return false;
        }
        #endregion PACKAGE_JSON
    }
}