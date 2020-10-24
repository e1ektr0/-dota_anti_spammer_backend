using System;

namespace test
{
    class Program
    {
        [STAThread]    static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            LowLevelKeyboardHook kbh = new LowLevelKeyboardHook();
            kbh.OnKeyPressed += kbh_OnKeyPressed;
            kbh.OnKeyUnpressed += kbh_OnKeyUnpressed;
            kbh.HookKeyboard();

            Application.Run();

            kbh.UnHookKeyboard();

        }
    }
}