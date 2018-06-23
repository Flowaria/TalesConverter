using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalesConverter
{
    public class Preferences
    {
        public static bool TSM_Zip_Music { get; set; }
        public static bool TSM_Voice_Extract_Zip { get; set; }
        public static bool TSM_Fastmode { get; set; }
        public static bool TSM_Analyze { get; set; }

        public static bool TSI_Zip_Image { get; set; }
        public static bool TSI_Merge_Image { get; set; }

        #region Registry Config
        public static void LoadFromRegistry()
        {
            string regSubkey = "Software\\TalesConverter";
            RegistryKey rk = Registry.LocalMachine.OpenSubKey(regSubkey, true);
            if (rk != null)
            {
                TSM_Zip_Music = bool.Parse((string)rk.GetValue("tsm_zip_music"));
                TSM_Voice_Extract_Zip = bool.Parse((string)rk.GetValue("tsm_voice_extract_zip"));
                TSM_Fastmode = bool.Parse((string)rk.GetValue("tsm_fastmode"));
                TSM_Analyze = bool.Parse((string)rk.GetValue("tsm_analyze"));

                TSI_Zip_Image = bool.Parse((string)rk.GetValue("tsi_zip_image"));
                TSI_Merge_Image = bool.Parse((string)rk.GetValue("tsi_merge_image"));
            }
        }

        public static void SaveToRegistry()
        {
            string regSubkey = "Software\\TalesConverter";
            RegistryKey rk = Registry.LocalMachine.OpenSubKey(regSubkey, true);
            if (rk == null)
            {
                rk = Registry.LocalMachine.CreateSubKey(regSubkey, true);
            }

            rk.SetValue("tsm_zip_music", TSM_Zip_Music);
            rk.SetValue("tsm_voice_extract_zip", TSM_Voice_Extract_Zip);
            rk.SetValue("tsm_fastmode", TSM_Fastmode);
            rk.SetValue("tsm_analyze", TSM_Analyze);

            rk.SetValue("tsi_zip_image", TSI_Zip_Image);
            rk.SetValue("tsi_merge_image", TSI_Merge_Image);
        }
        #endregion
    }
}
