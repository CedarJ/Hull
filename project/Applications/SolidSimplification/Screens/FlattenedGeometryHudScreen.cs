using Assimp;
using ClipperLib;
using SFML.Graphics;
using SFML.System;
using Shared.Core;
using Shared.Events.CallbackArgs;
using Shared.Events.EventArgs;
using Shared.Interfaces;
using Shared.Interfaces.Services;
using Shared.Menus;
using Shared.Notifications;
using SolidSimplification.Data;
using SolidSimplification.HelperMethods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Button = Shared.Menus.Button;

namespace SolidSimplification.Screens
{
    public class FlattenedGeometryHudScreen : Shared.Core.Screen
    {        // Notification service is used to display toast messages as feedback for the user.
        private INotificationService notificationService;
        private IApplicationManager applicationManager;
        private IEventService eventService;
        private FlattenedGeometrySharedData dataProvider;

        // Track the buttons for mouse click callback.
        private List<Button> buttons;

        // cache the current scene
        private Scene scene;

        public FlattenedGeometryHudScreen(
            IApplicationManager applicationManager,
            IEventService eventService,
            INotificationService notificationService,
            FlattenedGeometrySharedData dataProvider)
        {
            this.notificationService = notificationService;
            this.applicationManager = applicationManager;
            this.eventService = eventService;
            this.dataProvider = dataProvider;
        }

        public override void InitializeScreen()
        {
            // Buttons themselves have no listener, so we must listen for click callbacks and 
            // pass that event through to our buttons. Not optimal, but works.
            eventService.RegisterMouseClickCallback(
                this.Id,
                new MouseClickCallbackEventArgs(SFML.Window.Mouse.Button.Left),
                OnMousePress);

            // Initialize all screen data
            buttons = new List<Button>();

            scene = null;

            var loadDataButton = new Button(
                text: "Load 3D Model",
                position: new Vector2f(20, 20),
                callback: LoadModel,
                alignment: Shared.Menus.HorizontalAlignment.Left);

            var projectXButton = new Button(
                text: "Show X",
                position: new Vector2f(20, 80),
                callback: DrawResultX,
                alignment: Shared.Menus.HorizontalAlignment.Left);

            var projectYButton = new Button(
                text: "Show Y",
                position: new Vector2f(20, 140),
                callback: DrawResultY,
                alignment: Shared.Menus.HorizontalAlignment.Left);

            var projectZButton = new Button(
                text: "Show Z",
                position: new Vector2f(20, 200),
                callback: DrawResultZ,
                alignment: Shared.Menus.HorizontalAlignment.Left);

            buttons.Add(loadDataButton);
            buttons.Add(projectXButton);
            buttons.Add(projectYButton);
            buttons.Add(projectZButton);
        }

        private void LoadModel()
        {
            string filePath;

            // Currently hard coded to *.obj files, Assimp can handle many other types, if you want to import
            // other file types for testing, this can be modified.
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Obj files (*.obj)|*.obj";
            var result = openFileDialog.ShowDialog();

            // The open file dialog was closed.
            if (result == DialogResult.No || result == DialogResult.Cancel)
            {
                return;
            }

            filePath = openFileDialog.FileName;

            var importer = new AssimpContext();

            try
            {
                scene = importer.ImportFile(filePath);
            }
            catch
            {
                // There was an issue trying to parse the input data.
                notificationService.ShowToast(
                           ToastType.Error,
                           "An issue occurred while parsing the input data. Please check the format.");
                return;
            }

            // We have successfully loaded the scenario!
            notificationService.ShowToast(ToastType.Info, "Loaded model successfully. Choose a projection axis. ");
        }

        private void DrawResultX()
        {
            if(scene == null)
            {
                notificationService.ShowToast(
                               ToastType.Error,
                               "No model loaded. Please load model first. ");
                return;
            }

            // Mapteks current implementation
            Task.Run(() =>
            {
                // Existing hull generation technique
                var clipResult = HullGenerator.Generate(scene,1);

                // ----------------------------------------------------------
                // ----------- :TODO: Your code goes here -------------------
                // ----------------------------------------------------------

                if (clipResult.IsFailure)
                {
                    notificationService.ShowToast(
                               ToastType.Error,
                               "An issue occurred while performing the shape clipping.");
                    return;
                }

                this.dataProvider.SetVisuals(clipResult.Value);
            });
        }

        private void DrawResultY()
        {
            if (scene == null)
            {
                notificationService.ShowToast(
                               ToastType.Error,
                               "No model loaded. Please load model first. ");
                return;
            }

            // Mapteks current implementation
            Task.Run(() =>
            {
                // Existing hull generation technique
                var clipResult = HullGenerator.Generate(scene, 2);

                // ----------------------------------------------------------
                // ----------- :TODO: Your code goes here -------------------
                // ----------------------------------------------------------

                if (clipResult.IsFailure)
                {
                    notificationService.ShowToast(
                               ToastType.Error,
                               "An issue occurred while performing the shape clipping.");
                    return;
                }

                this.dataProvider.SetVisuals(clipResult.Value);
            });
        }

        private void DrawResultZ()
        {
            if (scene == null)
            {
                notificationService.ShowToast(
                               ToastType.Error,
                               "No model loaded. Please load model first. ");
                return;
            }

            // Mapteks current implementation
            Task.Run(() =>
            {
                // Existing hull generation technique
                var clipResult = HullGenerator.Generate(scene, 3);

                // ----------------------------------------------------------
                // ----------- :TODO: Your code goes here -------------------
                // ----------------------------------------------------------

                if (clipResult.IsFailure)
                {
                    notificationService.ShowToast(
                               ToastType.Error,
                               "An issue occurred while performing the shape clipping.");
                    return;
                }

                this.dataProvider.SetVisuals(clipResult.Value);
            });
        }

        public override void OnRender(RenderTarget target)
        {
            target.SetView(applicationManager.GetDefaultView());
            buttons.ForEach(b => b.OnRender(target));
        }

        private void OnMousePress(MouseClickEventArgs args)
        {
            buttons.ForEach(b => b.TryClick(args));
        }
    }
}