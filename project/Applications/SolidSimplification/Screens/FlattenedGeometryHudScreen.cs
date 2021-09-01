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

            var loadDataButton = new Button(
                text: "Load 3D Model",
                position: new Vector2f(20, 20),
                callback: LoadResult,
                alignment: Shared.Menus.HorizontalAlignment.Left);

            buttons.Add(loadDataButton);
        }

        private void LoadResult()
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

            Scene scene = null;

            var loadSuccessful = true;

            try
            {
                scene = importer.ImportFile(filePath);
            }
            catch
            {
                loadSuccessful = false;
            }


            // There was an issue trying to parse the input data.
            if (!loadSuccessful)
            {
                notificationService.ShowToast(
                           ToastType.Error,
                           "An issue occurred while parsing the input data. Please check the format.");
                return;
            }

            // We have successfully loaded the scenario!
            notificationService.ShowToast(ToastType.Info, "Clipping...");

            // Mapteks current implementation
            Task.Run(() =>
            {
                // Existing hull generation technique
                var clipResult = HullGenerator.Generate(scene);

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