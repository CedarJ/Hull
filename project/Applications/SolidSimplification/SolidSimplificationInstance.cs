using Ninject;
using SFML.Graphics;
using SFML.System;
using Shared.Core;
using Shared.Interfaces;
using Shared.Interfaces.Services;
using SolidSimplification.Screens;

namespace SolidSimplification
{
    class SolidSimplificationInstance : ApplicationInstanceBase, IApplicationInstance
    {
        private IApplicationService appService;

        public SolidSimplificationInstance(IApplicationService appService)
        {
            this.appService = appService;

            var texture = new Texture(new Image("Resources\\Maptek.png"));
            texture.GenerateMipmap();
            texture.Smooth = true;

            Thumbnail = new RectangleShape(new Vector2f(300, 300))
            {
                Texture = texture
            };
        }

        public string DisplayName => "Hull Calculation";

        public override void Initialize()
        {
            var demoScreen = appService.Kernel.Get<SolidSimplificationScreen>();
            AddChildScreen(demoScreen);

            base.Initialize();
        }
    }
}
