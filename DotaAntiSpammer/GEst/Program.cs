using System;
using System.Windows.Forms;

namespace GEst
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            LowLevelKeyboardHook kbh = new LowLevelKeyboardHook();
            kbh.OnKeyPressed += (sender, keys) =>
            {
                Console.WriteLine("sssss");
            };
                //kbh.OnKeyUnpressed += kbh_OnKeyUnpressed;
            kbh.HookKeyboard();

            Application.Run();

            kbh.UnHookKeyboard();

        }
    }
}