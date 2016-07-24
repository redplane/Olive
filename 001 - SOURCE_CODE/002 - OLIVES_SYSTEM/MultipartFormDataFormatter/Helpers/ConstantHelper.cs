﻿namespace MultipartFormDataMediaFormatter.Helpers
{
    public class ConstantHelper
    {
        /// <summary>
        ///     Static instance of ConstantHelper.
        /// </summary>
        private static ConstantHelper _instance;

        /// <summary>
        ///     List of image MIME types.
        /// </summary>
        public string[] ImageMediaTypes =
        {
            "image/cgm", "image/fits", "image/g3fax", "image/gif", "image/ief", "image/jp2", "image/jpeg", "image/jpm",
            "image/jpx", "image/naplps", "image/png", "image/prs.btif", "image/prs.pti", "image/t38", "image/tiff",
            "image/tiff-fx",
            "image/vnd.adobe.photoshop",
            "image/vnd.cns.inf2",
            "image/vnd.djvu",
            "image/vnd.dwg",
            "image/vnd.dxf",
            "image/vnd.fastbidsheet",
            "image/vnd.fpx",
            "image/vnd.fst",
            "image/vnd.fujixerox.edmics-mmr",
            "image/vnd.fujixerox.edmics-rlc",
            "image/vnd.globalgraphics.pgb",
            "image/vnd.microsoft.icon",
            "image/vnd.mix",
            "image/vnd.ms-modi",
            "image/vnd.net-fpx",
            "image/vnd.sealed.png",
            "image/vnd.sealedmedia.softseal.gif",
            "image/vnd.sealedmedia.softseal.jpg",
            "image/vnd.svf",
            "image/vnd.wap.wbmp",
            "image/vnd.xiff"
        };

        /// <summary>
        ///     Return instance of ConstantHelper.
        /// </summary>
        public static ConstantHelper Instance
        {
            get
            {
                // Static instance will be initialized if it hasn't been done before.
                return _instance ?? (_instance = new ConstantHelper());
            }
        }
    }
}