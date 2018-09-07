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
            Views.MainWindow MainWindow = new Views.MainWindow();
            Views.Controls.MainWindow.LedEntityButtons EntityButtons = new Views.Controls.MainWindow.LedEntityButtons();
            Views.Controls.LedEntityOverview Entity = new Views.Controls.LedEntityOverview();
            Views.Controls.MainWindow.EffectProperties EffectProperties = new Views.Controls.MainWindow.EffectProperties();
            Views.Controls.MainWindow.AudioUserControl audioUserControl = new Views.Controls.MainWindow.AudioUserControl();

            ViewModels.MainWindowVM MainViewModel = new ViewModels.MainWindowVM(MainWindow, Entity, EffectProperties, audioUserControl);

            System.Windows.Controls.Grid.SetColumn(EntityButtons, 1);
            MainWindow.Grid.Children.Add(EntityButtons);

            System.Windows.Controls.Grid.SetColumn(Entity, 1);
            System.Windows.Controls.Grid.SetRow(Entity, 1);
            MainWindow.Grid.Children.Add(Entity);

            System.Windows.Controls.Grid.SetColumn(EffectProperties, 2);
            System.Windows.Controls.Grid.SetRow(EffectProperties, 1);
            MainWindow.Grid.Children.Add(EffectProperties);

            System.Windows.Controls.Grid.SetRow(audioUserControl, 2);
            System.Windows.Controls.Grid.SetColumn(audioUserControl, 0);
            System.Windows.Controls.Grid.SetColumnSpan(audioUserControl, 3);
            MainWindow.Grid.Children.Add(audioUserControl);

            Instance.WindowService.ShowNewWindow(MainWindow, MainViewModel);
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
