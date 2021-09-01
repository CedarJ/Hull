using Ninject;
using Ninject.Parameters;
using SFML.Graphics;
using SFML.System;
using Shared.Core;
using Shared.Core.Hierarchy;
using Shared.Helpers;
using Shared.Interfaces;
using Shared.Interfaces.Services;
using SolidSimplification.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static SFML.Window.Mouse;

namespace SolidSimplification.Screens
{
    public class SolidSimplificationScreen : StackedScreen
    {
        private IApplicationService applicationService;
        private IApplicationManager applicaitonManager;
        private INotificationService notificationService;
        private IEventService eventService;

        public SolidSimplificationScreen(
           IKernel kernel,
           IApplicationManager applicaitonManager,
           IApplicationService applicationService,
           INotificationService notificationService,
           IEventService eventService)
        {
            // Application manager is used to add/remove screens, go home, get window size
            this.applicaitonManager = applicaitonManager;

            // Currently just holds the kernel.
            this.applicationService = applicationService;

            // Used to display information to the user - currently toast notifications
            this.notificationService = notificationService;

            // Used to capture all input (mouse/keyboard events). Allows screen to mark
            // an event as handled.
            this.eventService = eventService;

            var provider = new FlattenedGeometrySharedData();

            var hud = kernel.Get<FlattenedGeometryHudScreen>(
                new ConstructorArgument("dataProvider", provider));

            var visualizer = kernel.Get<FlattenedGeometryVisualizerScreen>(
                new ConstructorArgument("dataProvider", provider));

            AddScreen(visualizer);
            AddScreen(hud);

            this.Camera.SetCentre(new Vector2f(0, 0));
        }
    }
}
