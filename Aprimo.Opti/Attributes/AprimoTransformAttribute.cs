using System;
using System.ComponentModel.DataAnnotations;

namespace Aprimo.Opti.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AprimoTransformAttribute : ScaffoldColumnAttribute
    {
        public AprimoTransformAttribute()
            : base(false)
        {
        }

        public AprimoTransformAttribute(
            string width,
            string height,
            string format,
            string crop,
            string dpr,
            string fit,
            string pad,
            string quality,
            string saturation,
            string sharpen,
            string trim,
            string contrast,
            string brighten,
            string blur,
            string backgroundColor,
            string auto)
            : this()
        {
            this.Width = width;
            this.Height = height;
            this.Format = format;
            this.Crop = crop;
            this.DPR = dpr;
            this.Fit = fit;
            this.Pad = pad;
            this.Quality = quality;
            this.Saturation = saturation;
            this.Sharpen = sharpen;
            this.Trim = trim;
            this.Contrast = contrast;
            this.Brightness = brighten;
            this.Blur = blur;
            this.BackgroundColor = backgroundColor;
            this.Auto = auto;
        }

        public string Width { get; set; }

        public string Height { get; set; }

        public string Format { get; set; }

        public string Crop { get; set; }

        public string DPR { get; set; }

        public string Fit { get; set; }

        public string Pad { get; set; }

        public string Quality { get; set; }

        public string Saturation { get; set; }

        public string Sharpen { get; set; }

        public string Trim { get; set; }

        public string Contrast { get; set; }

        public string Brightness { get; set; }

        public string Blur { get; set; }

        public string BackgroundColor { get; set; }

        public string Auto { get; set; }
    }
}