using SFML.Graphics;
using SFML.System;
using Shared.Core;
using Shared.Events.EventArgs;
using Shared.Helpers;
using Shared.Interfaces;
using System;
using System.Collections.Generic;

namespace Dashboard.Home
{
    public class ApplicationDashboard
    {
        public bool IsActive { get; set; }

        public ApplicationInstanceVisual SelectedApplication => applications[selectedApplicationIndex];

        private Color clearColor = new Color(0xe1, 0xe1, 0xe1);
        private Vector2u selectedIconSize;
        private Vector2u nonSelectedIconSize;
        private List<ApplicationInstanceVisual> applications;
        private bool transitioning;
        private Vector2u size;
        private float gapSize;
        private int selectedApplicationIndex = 0;
        private Font defaultFont;
        private RectangleShape leftVisual;
        private RectangleShape rightVisual;
        private RectangleShape selectedVisual;
        private IApplicationManager applicationManager;

        public ApplicationDashboard(
            List<ApplicationInstanceVisual> applications,
            IApplicationManager applicationManager)
        {
            defaultFont = new Font("Resources\\font.ttf");
            selectedIconSize = new Vector2u(300, 300);
            nonSelectedIconSize = new Vector2u(220, 220);
            this.applications = applications;
            this.applicationManager = applicationManager;
            this.size = applicationManager.GetWindowSize();
            this.UpdateSizingAndSpacing();

            IsActive = true;

            selectedVisual = new RectangleShape(new Vector2f(selectedIconSize.X, selectedIconSize.Y));
            leftVisual = new RectangleShape(new Vector2f(nonSelectedIconSize.X, nonSelectedIconSize.Y));
            rightVisual = new RectangleShape(new Vector2f(nonSelectedIconSize.X, nonSelectedIconSize.Y));
        }

        public void OnMouseWheelMove(MouseWheelScrolledEventArgs eventArgs)
        {
            if (eventArgs.Args.Delta > 0)
            {
                selectedApplicationIndex -= 1;
                if (selectedApplicationIndex < 0)
                {
                    selectedApplicationIndex = applications.Count - 1;
                }
            }
            else
            {
                selectedApplicationIndex += 1;
                if (selectedApplicationIndex >= applications.Count)
                {
                    selectedApplicationIndex = 0;
                }
            }
        }

        public void LeftKeyPressed(KeyboardEventArgs _)
        {
            selectedApplicationIndex -= 1;
            if (selectedApplicationIndex < 0)
            {
                selectedApplicationIndex = applications.Count - 1;
            }
        }

        public void RightKeyPressed(KeyboardEventArgs _)
        {
            selectedApplicationIndex += 1;
            if (selectedApplicationIndex >= applications.Count)
            {
                selectedApplicationIndex = 0;
            }
        }

        public void OnMouseClick(MouseClickEventArgs eventArgs)
        {
            if (selectedVisual.GetGlobalBounds().Contains(eventArgs.Args.X, eventArgs.Args.Y))
            {
                applicationManager.SetActiveApplication(applications[GetSelectedApplicationIndex()].ApplicationInstance);
            }

            if (leftVisual.GetGlobalBounds().Contains(eventArgs.Args.X, eventArgs.Args.Y))
            {
                applicationManager.SetActiveApplication(applications[GetLeftApplicationIndex()].ApplicationInstance);
            }

            if (rightVisual.GetGlobalBounds().Contains(eventArgs.Args.X, eventArgs.Args.Y))
            {
                applicationManager.SetActiveApplication(applications[GetRightApplicationIndex()].ApplicationInstance);
            }
        }

        public void Update(float deltaT)
        {
            if (!transitioning)
            {

            }
            else
            {
                // Update the transition
            }
        }

        public void Draw(RenderTarget target)
        {
            target.Clear(clearColor);

            if (!transitioning)
            {
                var selected = GetSelectedApplicationIndex();
                var left = GetLeftApplicationIndex();
                var right = GetRightApplicationIndex();
                
                selectedVisual.Texture = applications[selected].Image.Texture;
                selectedVisual.Position = GetSelectedApplicationPosition();

                var selectedText = new Text(applications[selected].DisplayName, defaultFont, 28)
                {
                    Position = new Vector2f(size.X / 2, selectedVisual.Position.Y + 310),
                    FillColor = Color.Black
                };

                var bounds = selectedText.GetLocalBounds();
                selectedText.Origin = new Vector2f(bounds.Width / 2, bounds.Height / 2);

                leftVisual.Texture = applications[left].Image.Texture;
                leftVisual.Position = GetLeftApplicationPosition();

                rightVisual.Texture = applications[right].Image.Texture;
                rightVisual.Position = GetRightApplicationPosition();

                target.Draw(selectedVisual);
                target.Draw(leftVisual);
                target.Draw(rightVisual);
                target.Draw(selectedText);
            }
            else
            {
                // Draw the applications positions/scales
            }
        }

        private int GetSelectedApplicationIndex()
        {
            return selectedApplicationIndex;
        }

        private int GetLeftApplicationIndex()
        {
            return selectedApplicationIndex > 0
                ? selectedApplicationIndex - 1
                : applications.Count - 1;
        }

        private int GetRightApplicationIndex()
        {
            return selectedApplicationIndex < (applications.Count - 1)
                ? selectedApplicationIndex + 1
                : 0;
        }

        private Vector2f GetSelectedApplicationPosition()
        {
            var x = size.X / 2 - selectedIconSize.X / 2;
            var y = size.Y / 2 - selectedIconSize.Y / 2;
            return new Vector2f(x, y);
        }

        private Vector2f GetLeftApplicationPosition()
        {
            // Get the position of the bottom left hand corner of the thumbnail
            var x = gapSize;
            var y = (size.Y / 2) - (nonSelectedIconSize.Y / 2);

            return new Vector2f(x, y);
        }

        private Vector2f GetRightApplicationPosition()
        {
            // Right hand side - gap - width of icon gives left hand side.
            var x = size.X - gapSize - nonSelectedIconSize.X;

            //
            var y = (size.Y / 2) - (nonSelectedIconSize.Y / 2);

            return new Vector2f(x, y);
        }

        private void UpdateSizingAndSpacing()
        {
            // Ensure that the selected application thumbnail sizes will fit on our screen with a reasonable gap

            var totalIconWidth = selectedIconSize.X + 2 * nonSelectedIconSize.X;
            var remainingSize = size.X - totalIconWidth;
            gapSize = remainingSize / 4;

            if(gapSize < 50)
            {
                selectedIconSize /= 2;
                nonSelectedIconSize /= 2;
                UpdateSizingAndSpacing();
            }
        }
    }
}
