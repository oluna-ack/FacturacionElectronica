namespace firmarStdSri
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;
    using System.Data;

    /// <summary>
    /// Summary description for rideFactLiquidacion.
    /// </summary>
    public partial class rideFactLiquidacion : Telerik.Reporting.Report
    {
        public rideFactLiquidacion()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
        public rideFactLiquidacion(string pathLogo, int largo, int alto)
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            this.picLogo.Width = Unit.Pixel(largo);
            this.picLogo.Height = Unit.Pixel(alto);
            this.picLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.ScaleProportional;
            //this.picLogo.Value = recursos.recursos.srtUrlLogoDesarrollo + logo; //desarrollo local host
            this.picLogo.Value = pathLogo; // producción

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}