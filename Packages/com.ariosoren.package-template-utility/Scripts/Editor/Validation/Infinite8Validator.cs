namespace ArioSoren.PackageTemplateUtility.Editor.Validation
{
    public class Validator : StandardValidator
    {
        private static Validator _instance;
        
        #region CORE
        public static Validator Instance()
        {
            return _instance ??= new Validator();
        }
        #endregion CORE
    }
}