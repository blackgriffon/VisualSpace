using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Visual_Space
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            Tuple<AppTheme, Accent> appStyle = ThemeManager.DetectAppStyle(Application.Current);

        // now set the Green accent and dark theme
        ThemeManager.ChangeAppStyle(Application.Current,
                                    ThemeManager.GetAccent("Cyan"),
                                    ThemeManager.GetAppTheme("BaseLight")); // or appStyle.Item1

        base.OnStartup(e);

            /*You can choose between these available accents:
             “Red”, “Green”, “Blue”, “Purple”, “Orange”, “Lime”, “Emerald”, 
             “Teal”, “Cyan”, “Cobalt”, “Indigo”, “Violet”, “Pink”, “Magenta”, 
             “Crimson”, “Amber”, “Yellow”, “Brown”, “Olive”, “Steel”, “Mauve”, “Taupe”, “Sienna”
             
            and these themes:
            “BaseLight”, “BaseDark”
             */





            /*
             // add custom accent and theme resource dictionaries to the ThemeManager
        // you should replace MahAppsMetroThemesSample with your application name
        // and correct place where your custom accent lives
        ThemeManager.AddAccent("CustomAccent1", new Uri("pack://application:,,,/MahAppsMetroThemesSample;component/CustomAccents/CustomAccent1.xaml"));

        // get the current app style (theme and accent) from the application
        Tuple<AppTheme, Accent> theme = ThemeManager.DetectAppStyle(Application.Current);

        // now change app style to the custom accent and current theme
        ThemeManager.ChangeAppStyle(Application.Current,
                                    ThemeManager.GetAccent("CustomAccent1"),
                                    theme.Item1);

        base.OnStartup(e);
             
             */






        }




    }
}
