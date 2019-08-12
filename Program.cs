using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PB_Resetter
{
    public class Program
    {
        [DllImport("kernel32")]
        private static extern int OpenProcess(int dwDesiredAccess, int bInheritHandle, int dwProcessId);

        [DllImport("kernel32")]
        private static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int nSize, int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, int lpNumberOfBytesRead);

        private static int processHandle;

        static void Main()
        {
            Application.EnableVisualStyles();

            Process[] processList = Process.GetProcessesByName("ASN_App_PcDx9_Final");

            foreach (Process process in processList)
            {
                processHandle = OpenProcess(0x38, 0, process.Id);

                if (processHandle == 0)
                {
                    MessageBox.Show("Could not access the game, please run as administrator!", "ASRT PB Resetter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(2);
                }

                SetBytes(0x4EEDF4, new byte[] { 0x02 });
                SetBytes(0x79873C, new byte[] { 0x90, 0x90 });
                SetBytes(0x79ABCF, new byte[] { 0x90, 0x90 });
                SetBytes(0x79AC55, new byte[] { 0xEB });
                SetBytes(0x79AD10, new byte[] { 0x90, 0x90 });
                SetBytes(0x83D051, new byte[] { 0xEB });
            }

            if (processList.Length > 0)
            {
                MessageBox.Show("PB Resetter is now active!\n\n" +
                    "Each time you complete a lap in time attack it will be set as your new PB. " +
                    "When you finish the time attack session it will be saved to your save file and the online leaderboards.\n\n" +
                    "To disable the PB resetter, simply restart the game.", "ASRT PB Resetter", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Please start S&ASRT first!", "ASRT PB Resetter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
        }

        private static int ReadInt(int Address)
        {
            return BitConverter.ToInt32(ReadBytes(Address, 4), 0);
        }

        private static byte[] ReadBytes(int Address, int ByteCount)
        {
            byte[] Bytes = new byte[ByteCount];
            ReadProcessMemory(processHandle, Address, Bytes, ByteCount, 0);
            return Bytes;
        }

        public static void SetBytes(int Address, byte[] Bytes)
        {
            WriteProcessMemory(processHandle, Address, Bytes, Bytes.Length, 0);
        }
    }
}