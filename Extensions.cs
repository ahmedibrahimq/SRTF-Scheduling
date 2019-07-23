using System.Windows.Forms;

namespace SRTF
{
    static class Extensions
    {
        public static string DefaultText(this TextBox txt)
        {
            string[] DefaultTxt = { "process name", "burst time", "arrival time" };
            return DefaultTxt[txt.TabIndex];
        }
        
    }
}
