using CG_Lab5;

namespace ����_5
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new GrafficResultForm());
        }
    }
}