using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led
{
    public partial class App : System.Windows.Application
    {
        public static App Instance = new App();

        public Services.WindowService WindowService;
        public Services.IOService IOService;
        public Services.MediatorService MediatorService;
        public Services.EffectService EffectService;
        public Services.ConnectivityService ConnectivityService;

        public Model.Project Project;

        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCode()]
        static void Main()
        {
            Instance.WindowService = new Services.WindowService();
            Instance.IOService = new Services.IOService();
            Instance.MediatorService = new Services.MediatorService();
            Instance.EffectService = new Services.EffectService();
            Instance.ConnectivityService = new Services.ConnectivityService();
            Instance.ConnectivityService.StartServer();
            MainWindowTest();
        }

        static void DialogTest()
        {
            Instance.WindowService = new Services.WindowService();

            View.SingleTextDialog dialog = new View.SingleTextDialog()
            {
                Title = "Neues Projekt erstellen"
            };
            ViewModels.NewProjectDialogVM dialogVM = new ViewModels.NewProjectDialogVM();

            Instance.WindowService.ShowNewWindow(dialog, dialogVM);
        }

        static void LedEntityCreationTest()
        {
            Views.CRUDs.LedEntityCRUD test = new Views.CRUDs.LedEntityCRUD();
            test.Grid.Children.Add(new Views.Controls.CRUDs.LedEntityGroupProperties());

            Instance.Run(test);
        }

        static void MainWindowTest()
        {
            Views.MainWindow mainWindow = new Views.MainWindow();

            Instance.EffectService.MainWindow = mainWindow;
            Instance.MediatorService.MainWindow = mainWindow;

            Views.Controls.MainWindow.LedEntityButtons entityButtons = new Views.Controls.MainWindow.LedEntityButtons();
            Views.Controls.LedEntityOverview entity = new Views.Controls.LedEntityOverview();
            Views.Controls.MainWindow.EffectProperties effectProperties = new Views.Controls.MainWindow.EffectProperties();
            Views.Controls.MainWindow.TimelineUserControl timelineUserControl = new Views.Controls.MainWindow.TimelineUserControl();
            Views.Controls.MainWindow.AudioUserControl audioUserControl = new Views.Controls.MainWindow.AudioUserControl();
            Views.Controls.MainWindow.NetworkClientOverview networkClientOverview = new Views.Controls.MainWindow.NetworkClientOverview();

            ViewModels.MainWindowVM MainViewModel = new ViewModels.MainWindowVM(mainWindow, entity, effectProperties, timelineUserControl, audioUserControl, networkClientOverview);

            System.Windows.Controls.Grid.SetColumn(entityButtons, 1);
            mainWindow.Grid.Children.Add(entityButtons);

            System.Windows.Controls.Grid.SetColumn(entity, 1);
            System.Windows.Controls.Grid.SetRow(entity, 1);
            mainWindow.Grid.Children.Add(entity);

            System.Windows.Controls.Grid.SetColumn(effectProperties, 2);
            System.Windows.Controls.Grid.SetRow(effectProperties, 1);
            mainWindow.Grid.Children.Add(effectProperties);           

            System.Windows.Controls.Grid.SetRow(timelineUserControl, 2);
            System.Windows.Controls.Grid.SetColumn(timelineUserControl, 0);
            System.Windows.Controls.Grid.SetColumnSpan(timelineUserControl, 3);
            mainWindow.Grid.Children.Add(timelineUserControl);

            System.Windows.Controls.Grid.SetRow(audioUserControl, 3);
            System.Windows.Controls.Grid.SetColumn(audioUserControl, 0);
            System.Windows.Controls.Grid.SetColumnSpan(audioUserControl, 3);
            mainWindow.Grid.Children.Add(audioUserControl);

            System.Windows.Controls.Grid.SetRow(networkClientOverview, 1);
            System.Windows.Controls.Grid.SetColumn(networkClientOverview, 0);
            mainWindow.Grid.Children.Add(networkClientOverview);

            Instance.WindowService.ShowNewWindow(mainWindow, MainViewModel);
        }

        static void JsonTest()
        {
            Model.Effect.EffectSetColor effectSetColor = new Model.Effect.EffectSetColor();
            List<Model.Effect.EffectBase> _tmp = new List<Model.Effect.EffectBase>
            {
                new Model.Effect.EffectFadeColor(),
                new Model.Effect.EffectBlinkColor(),
                effectSetColor
            };


            System.IO.File.WriteAllText(@"C:\Users\Robin\Desktop\test.json", Newtonsoft.Json.JsonConvert.SerializeObject(_tmp, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            }));

            List<Model.Effect.EffectBase> _tmp2 = JsonConvert.DeserializeObject<List<Model.Effect.EffectBase>>(System.IO.File.ReadAllText(@"C:\Users\Robin\Desktop\test.json"), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }
    }
}
