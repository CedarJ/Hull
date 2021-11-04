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

        // epsilon for simplification
        private double epsilon;

        // alpha for aggregation
        private float alpha;

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
            epsilon = 0.5;

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

            var setEpsilonButton = new Button(
                text: "Set Epsilon",
                position: new Vector2f(20, 260),
                callback: setEpsilon,
                alignment: Shared.Menus.HorizontalAlignment.Left);

            var setAlphaButton = new Button(
                text: "Set Alpha",
                position: new Vector2f(20, 320),
                callback: setAlpha,
                alignment: Shared.Menus.HorizontalAlignment.Left);

            buttons.Add(loadDataButton);
            buttons.Add(setEpsilonButton);
            buttons.Add(setAlphaButton);
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
                notificationService.ShowToast(ToastType.Info, "Generating Hull on X-axis...");

                // hull generation
                var clipResult = HullGenerator.Generate(scene,1, alpha);

                // hull simplification when epsilon is greater than 0
                if (this.epsilon > 0)
                {
                    System.Diagnostics.Debug.Write("Number of lines before simplification: ");
                    System.Diagnostics.Debug.Write(clipResult.Value.Count);
                    System.Diagnostics.Debug.Write("\n");

                    notificationService.ShowToast(ToastType.Info, "Simplifying...");

                    clipResult = HullSimplifier.Simplify(clipResult.Value, epsilon);

                    System.Diagnostics.Debug.Write("Number of lines after simplification: ");
                    System.Diagnostics.Debug.Write(clipResult.Value.Count);
                    System.Diagnostics.Debug.Write("\n");
                }

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
                notificationService.ShowToast(ToastType.Info,"Generating Hull on Y-axis...");

                // hull generation
                var clipResult = HullGenerator.Generate(scene, 2, alpha);

                // hull simplification when epsilon is greater than 0
                if (this.epsilon > 0)
                {
                    System.Diagnostics.Debug.Write("Number of lines before simplification: ");
                    System.Diagnostics.Debug.Write(clipResult.Value.Count);
                    System.Diagnostics.Debug.Write("\n");

                    notificationService.ShowToast(ToastType.Info, "Simplifying...");

                    clipResult = HullSimplifier.Simplify(clipResult.Value, epsilon);

                    System.Diagnostics.Debug.Write("Number of lines after simplification: ");
                    System.Diagnostics.Debug.Write(clipResult.Value.Count);
                    System.Diagnostics.Debug.Write("\n");
                }

                // error handling
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
                notificationService.ShowToast(ToastType.Info, "Generating Hull on Z-axis...");

                // hull generation
                var clipResult = HullGenerator.Generate(scene, 3, alpha);

                // hull simplification when epsilon is greater than 0
                if (this.epsilon > 0)
                {
                    System.Diagnostics.Debug.Write("Number of lines before simplification: ");
                    System.Diagnostics.Debug.Write(clipResult.Value.Count);
                    System.Diagnostics.Debug.Write("\n");

                    notificationService.ShowToast(ToastType.Info, "Simplifying...");

                    clipResult = HullSimplifier.Simplify(clipResult.Value, epsilon);

                    System.Diagnostics.Debug.Write("Number of lines after simplification: ");
                    System.Diagnostics.Debug.Write(clipResult.Value.Count);
                    System.Diagnostics.Debug.Write("\n");
                }

                // error handling
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

        // Set the epsilon value for hull simplification
        private void setEpsilon()
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Enter epsilon for hull simplification, 0 for no simplification", "Set Epsilon ", this.epsilon.ToString(), 0, 0);
            this.epsilon = Convert.ToDouble(input);
        }

        // Set alpha value for hull aggregation
        private void setAlpha()
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Enter integer alpha for hull aggregation, increase for convex and decrease for concave; if the output is incomplete or not closed, please increase the alpha; 0 for union only(recommended while side length gap is too large)", "Set alpha ", this.alpha.ToString(), 0, 0);
            this.alpha = Convert.ToSingle(input);
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