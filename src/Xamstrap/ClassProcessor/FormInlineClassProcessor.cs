﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static Xamstrap.Enums;

namespace Xamstrap.ClassProcessor
{
    public static partial class Extension
    {
        public static void ProcessInlineClass(this Layout<View> element, double x, double y, double width, double height)
        {
            double xPos = x, yPos = y;
            double lastChildHeight = 0;
            double totalWidth = 0;
            DeviceSize deviceSize = Common.GetCurrentDeviceSize();

            foreach (var child in element.Children)
            {
                var request = child.Measure(width, height);
                var childWidth = request.Request.Width;
                var childHeight = request.Request.Height;

                lastChildHeight = Math.Max(childHeight + child.Margin.VerticalThickness, lastChildHeight);

                totalWidth += childWidth + child.Margin.HorizontalThickness;
                if (deviceSize == DeviceSize.ExtraSmall || deviceSize == DeviceSize.Small)
                {
                    xPos = x;
                    childWidth = width;
                }
                else
                {
                    if (totalWidth > width)
                    {
                        yPos += lastChildHeight;
                        xPos = x;
                        totalWidth = 0;
                    }
                }                

                var region = new Rectangle(xPos + child.Margin.Left, yPos + child.Margin.Top, childWidth, childHeight);
                child.Layout(region);               
                if (deviceSize == DeviceSize.ExtraSmall || deviceSize == DeviceSize.Small)
                {
                    yPos += childHeight + child.Margin.VerticalThickness;
                }
                else
                {
                    xPos += childWidth + child.Margin.HorizontalThickness;
                }
            }
        }

        public static SizeRequest ProcessFormInlineSizeRequest(this Layout<View> element, double widthConstraint, double heightConstraint)
        {
            DeviceSize device = Common.GetCurrentDeviceSize();
            if (element.WidthRequest > 0)
                widthConstraint = Math.Min(element.WidthRequest, widthConstraint);
            if (element.HeightRequest > 0)
                heightConstraint = Math.Min(element.HeightRequest, heightConstraint);

            double internalHeight = double.IsPositiveInfinity(heightConstraint) ? double.PositiveInfinity : Math.Max(0, heightConstraint);
            double internalWidth = double.IsPositiveInfinity(widthConstraint) ? double.PositiveInfinity : Math.Max(0, widthConstraint);

            // Measure children height
            double height = 0d;
            double lastChildHeight = 0d;
            double totalWidth = 0;
            double totalHeight = 0d;
            int totalRow = 1;
            foreach (var child in element.Children)
            {
                var size = child.Measure(internalWidth, internalHeight);
                lastChildHeight = Math.Max(size.Request.Height + child.Margin.VerticalThickness, lastChildHeight);
                totalWidth += size.Request.Width + child.Margin.HorizontalThickness;
                if (device == DeviceSize.ExtraSmall || device == DeviceSize.Small)
                {
                    totalHeight += size.Request.Height + child.Margin.VerticalThickness;
                }
                else
                {
                    totalHeight = lastChildHeight;
                    if (totalWidth > internalWidth)
                    {
                        totalRow += 1;
                        totalWidth = 0;
                    }
                }

            }

            height = element.Padding.VerticalThickness + totalHeight;
            if (totalRow > 1)
            {
                height = element.Padding.VerticalThickness + (totalHeight * totalRow);
            }

            return new SizeRequest(new Size(internalWidth, height), new Size(0, 0));
        }

    }
}
