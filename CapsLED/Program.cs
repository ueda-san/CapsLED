using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CapsLED
{
    public class CapsLedContext : ApplicationContext
    {
        private readonly NotifyIcon trayIcon;
        private readonly Timer timer;
        private readonly LedControl ledControl = new LedControl();
        private bool imeStatus = false;

        public CapsLedContext()
        {
            trayIcon = new NotifyIcon()
            {
                Text = "CapsLED",
                Icon = Properties.Resources.icon,
                ContextMenu = new ContextMenu(new MenuItem[]
                {
                    new MenuItem("About", About),
                    new MenuItem("Exit", Exit)
                }),
                Visible = true
            };

            int ret = ledControl.OpenDevice();
            if (ret < 0)
            {
                MessageBox.Show("Not available on your system\nError code:" + ret, "CapsLED", MessageBoxButtons.OK, MessageBoxIcon.Error);
                trayIcon.Visible = false;
                Application.Exit();
            } else {
                timer = new Timer
                {
                    Interval = 200 // 0.2sec
                };
                timer.Tick += new EventHandler(CheckIME);
                timer.Start();
            }
        }

        void About(object sender, EventArgs e)
        {
            string mes = String.Format("CapsLED v{0}\n{1}",
                                       Application.ProductVersion,
                                       "https://github.com/ueda-san/CapsLED");
            MessageBox.Show(mes, "About");
        }

        void Exit(object sender, EventArgs e)
        {
            ledControl.Close();
            trayIcon.Visible = false;
            Application.Exit();
        }

        void CheckIME(object sender, EventArgs e)
        {
            const int WM_IME_CONTROL = 0x283;
            const int IMC_GETOPENSTATUS = 5;

            NativeMethods.GUITHREADINFO threadInfo = default;
            threadInfo.cbSize = Marshal.SizeOf<NativeMethods.GUITHREADINFO>(threadInfo);
            if (NativeMethods.GetGUIThreadInfo(0u, ref threadInfo))
            {
                bool ime = NativeMethods.SendMessage(NativeMethods.ImmGetDefaultIMEWnd(threadInfo.hwndFocus), WM_IME_CONTROL, (IntPtr)IMC_GETOPENSTATUS, IntPtr.Zero) != IntPtr.Zero;
                if (imeStatus != ime)
                {
                    imeStatus = ime;
                    ledControl.SetLED(imeStatus);
                }
            }
        }
    }

    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CapsLedContext());
        }
    }
}
