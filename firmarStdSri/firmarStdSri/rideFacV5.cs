namespace firmarStdSri
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for rideFacV5.
    /// </summary>
    public partial class rideFacV5 : Telerik.Reporting.Report
    {
        public rideFacV5()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
        public rideFacV5(string pathLogo, int largo, int alto)
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            this.picLogo.Width = Unit.Pixel(largo);
            this.picLogo.Height = Unit.Pixel(alto);
            this.picLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.ScaleProportional;
            this.picLogo.Value = pathLogo;

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}