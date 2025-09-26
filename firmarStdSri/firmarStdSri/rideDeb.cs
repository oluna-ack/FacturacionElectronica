namespace firmarStdSri
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rideDeb.
    /// </summary>
    public partial class rideDeb : Telerik.Reporting.Report
    {
        public rideDeb()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
        public rideDeb(string pathLogo, int largo, int alto)
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            this.picLogo.Width = Unit.Pixel(largo);
            this.picLogo.Height = Unit.Pixel(alto);
            this.picLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.ScaleProportional;
            this.picLogo.Value = pathLogo; //desarrollo local host
            //this.picLogo.Value = recursos.recursos.srtUrlLogoProduccion + logo; // producción

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}