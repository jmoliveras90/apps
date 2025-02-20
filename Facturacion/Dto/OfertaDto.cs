using DocumentFormat.OpenXml.Wordprocessing;
using System.Globalization;
using System.Security.Cryptography;

namespace Facturacion.Dto
{
    public record OfertaDto
    {
        public string Propietario { get; set; }
        public string Campaña { get; set; }
        public string IdOferta { get; set; }
        public decimal ImporteOferta { get; set; }
        public string Etapa { get; set; }
        public string EstadoContrato { get; set; }
        public string Nombre { get; set; }
        public string SubActividad { get; set; }
        public string Solution => Nombre.ToLower().Contains("lvge") ? "LVGE" : (Nombre.Contains("cashlogy", StringComparison.CurrentCultureIgnoreCase) || Nombre.Contains("inlane", StringComparison.CurrentCultureIgnoreCase)) ? "FRONT" : "BACK";
        public short NumCentros { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int Año => FechaModificacion.Year;
        public int Mes => FechaModificacion.Month;
        public int Dia => FechaModificacion.Day;
        public int Semana
        { 
            get
            {                
                DateTime firstMonthDay = new(FechaModificacion.Year, FechaModificacion.Month, 1);

                int daysDifference = (FechaModificacion.Day - firstMonthDay.Day);
               
                return (daysDifference / 7) + 1;
            }
        }

        public int SemanaAño
        {
            get
            {
                DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
                DateTime date = FechaModificacion;
                System.Globalization.Calendar cal = dfi.Calendar;

                return cal.GetWeekOfYear(date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            }
        }

        public short NumOfertas => 1;
    }
}
