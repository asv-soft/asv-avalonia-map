using System;
using Avalonia;
using Avalonia.Controls;

namespace Asv.Avalonia.Map;

public class RadialPanel : Panel

    {

        // Measure each children and give as much room as they want 

 

        protected override Size MeasureOverride(Size availableSize)

        {

            foreach (var elem in Children)

            {

                //Give Infinite size as the avaiable size for all the children

                elem.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            }

            return base.MeasureOverride(availableSize);

        }

 

        //Arrange all children based on the geometric equations for the circle.

        protected override Size ArrangeOverride(Size finalSize)

        {

            if (Children.Count == 0)
                return finalSize;

            var incAngle = 0.0;
            var startAngle = 0.0;
            
            switch (Children.Count)
            {
                case 1:
                    incAngle = 0;
                    startAngle = 90;
                    break;
                case 2:
                    startAngle = 30+90;
                    incAngle = -60;
                    break;
                case 3:
                    startAngle = 45+90;
                    incAngle = -45;
                    break;
                default:
                    startAngle = 180;
                    incAngle = -(180.0 / (Children.Count - 1));
                    break;
            }

            startAngle *= Math.PI / 180;
            incAngle *=Math.PI / 180;
            
            
            
            var radiusX = finalSize.Width / 3;
            var radiusY = finalSize.Height / 3;

            foreach (var elem in Children)
            {

                //Calculate the point on the circle for the element

 

                var childPoint = new Point(Math.Cos(startAngle) * radiusX, -Math.Sin(startAngle) * radiusY);

                //Offsetting the point to the Avalable rectangular area which is FinalSize.

                var actualChildPoint = new Point(finalSize.Width / 2 + childPoint.X - elem.DesiredSize.Width / 2,finalSize.Height / 2 + childPoint.Y - elem.DesiredSize.Height / 2);

 

                //Call Arrange method on the child element by giving the calculated point as the placementPoint.

                elem.Arrange(new Rect(actualChildPoint.X, actualChildPoint.Y, elem.DesiredSize.Width, elem.DesiredSize.Height));

 

                //Calculate the new _angle for the next element

                startAngle += incAngle;

 

            }

 

            return finalSize;

        }

    }