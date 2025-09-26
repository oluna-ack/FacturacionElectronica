namespace firmarStdSri
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rideNc.
    /// </summary>
    public partial class rideNc : Telerik.Reporting.Report
    {
        public rideNc()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
        public rideNc(string pathLogo, int largo, int alto)
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.picLogo.Width = Unit.Pixel(largo);
            this.picLogo.Height = Unit.Pixel(alto);
            this.picLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.ScaleProportional;
            this.picLogo.Value = pathLogo; //desarrollo local host
            //this.picLogo.Value = recursos.recursos.srtUrlLogoProduccion + logo; // producci�n
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}