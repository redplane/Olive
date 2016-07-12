using System;

namespace Shared.Helpers
{
    public class CodeGeneratorHelper
    {
        /// <summary>
        ///     Generate code with given length.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public string Generate(int length)
        {
            // Initialize random number generator.
            var random = new Random();

            // Code which will be generated and returned.
            var code = "";

            // Start generating code.
            for (var i = 0; i < length; i++)
                code += Characters[random.Next(Characters.Length)];

            return code;
        }

        #region Properties

        /// <summary>
        ///     Static instance of CodeGeneratorHelper.
        /// </summary>
        private static CodeGeneratorHelper _instance;

        /// <summary>
        ///     - As available, CodeGeneratorHelper instance will be returned.
        ///     - Otherwise, initialize it before return.
        /// </summary>
        public static CodeGeneratorHelper Instance
        {
            get { return _instance ?? (_instance = new CodeGeneratorHelper()); }
        }

        /// <summary>
        ///     List of supported characters.
        /// </summary>
        private const string Characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        #endregion
    }
}