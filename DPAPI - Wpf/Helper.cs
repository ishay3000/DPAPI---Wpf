using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DPAPI___Wpf
{
    public static class Helper
    {
        public static void CloseWindowOfWhichThereIsOnlyOne<T>()
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            foreach (Window w in System.Windows.Application.Current.Windows)
            {
                if (w.GetType().Assembly == currentAssembly && w is T)
                {
                    w.Close();
                    
                }
            }
            foreach (System.Windows.Forms.Form item in System.Windows.Forms.Application.OpenForms)
            {
                if (item.GetType().Assembly == currentAssembly && item is T)
                {
                    item.Close();
                }
            }
        }
    }
}
