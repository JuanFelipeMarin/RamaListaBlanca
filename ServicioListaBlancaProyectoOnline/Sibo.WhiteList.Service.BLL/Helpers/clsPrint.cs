using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.BLL.Helpers
{
    public class clsPrint
    {
        private eWhiteList _whiteList = null;

        public clsPrint(eWhiteList whiteList)
        {
            _whiteList = whiteList;
        }

        public async void ImprimirReservas()
        {
            await Task.Run(() =>
            {
                PrintDocument printDoc = new PrintDocument();
                PrinterSettings ps = new PrinterSettings();
                printDoc.PrinterSettings = ps;
                printDoc.PrintPage += PrintBookings;

                if (_whiteList != null)
                    printDoc.Print();
            });
        }

        private void PrintBookings(object sender, PrintPageEventArgs e)
        {

            Font font = new Font("Arial", 14, FontStyle.Regular, GraphicsUnit.Point);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.Black);
            RectangleF rect = new System.Drawing.RectangleF(0, 100, 100, 50);
            var culture = new System.Globalization.CultureInfo("es-MX");

            string textToPrint = "TIQUETE DE RESERVA\r\n";

            if (_whiteList.reserveId > 0)
                textToPrint += "N° Reserva: " + _whiteList.reserveId + "\r\n";

            if (!string.IsNullOrEmpty(_whiteList.id))
                textToPrint += "ID Cliente: " + _whiteList.id + "\r\n";

            if (!string.IsNullOrEmpty(_whiteList.name))
                textToPrint += "Nombre: " + _whiteList.name + "\r\n";

            if (!string.IsNullOrEmpty(_whiteList.className))
                textToPrint += "Clase: " + _whiteList.className + "\r\n";

            if (_whiteList.dateClass != null)
            {
                DateTime classDate = (DateTime)_whiteList.dateClass;
                var day = culture.DateTimeFormat.GetDayName(classDate.DayOfWeek).ToString().ToUpper();

                textToPrint += "Fecha: " + classDate.Date.ToString("dd/MM/yyyy") + "\r\n";
                textToPrint += "Hora: " + classDate.Hour + ":" + classDate.Minute + "\r\n";
                textToPrint += "Día: " + day + "\r\n";
            }

            if (!string.IsNullOrEmpty(_whiteList.employeeName))
                textToPrint += "Profesor: " + _whiteList.employeeName + "\r\n";

            textToPrint += "Estado: Asistió\r\n";

            DateTime defaultDate = new DateTime(1900, 1, 1);
            if (_whiteList.expirationDate != null && _whiteList.expirationDate != defaultDate)
                textToPrint += "Plan vigente hasta: " + ((DateTime)_whiteList.expirationDate).Date.ToString("dd/MM/yyyy") + "\r\n";

            if (_whiteList.availableEntries > 0)
                textToPrint += "Tiquetes disponibles: " + _whiteList.availableEntries + "\r\n";

            e.Graphics.DrawString(textToPrint, font, drawBrush, (e.PageBounds.Width / 2), 200, sf);


        }
    }
}
