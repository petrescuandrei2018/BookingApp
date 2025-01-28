namespace BookingApp.Helpers
{
    public static class UtilitatiGeografice
    {
        public static double CalculeazaDistanta(double latitudine1, double longitudine1, double latitudine2, double longitudine2)
        {
            const double razaPamantului = 6371; // Raza Pământului în kilometri
            var diferentaLatitudine = (latitudine2 - latitudine1) * Math.PI / 180;
            var diferentaLongitudine = (longitudine2 - longitudine1) * Math.PI / 180;

            var formula = Math.Sin(diferentaLatitudine / 2) * Math.Sin(diferentaLatitudine / 2) +
                          Math.Cos(latitudine1 * Math.PI / 180) * Math.Cos(latitudine2 * Math.PI / 180) *
                          Math.Sin(diferentaLongitudine / 2) * Math.Sin(diferentaLongitudine / 2);

            var circumferinta = 2 * Math.Atan2(Math.Sqrt(formula), Math.Sqrt(1 - formula));
            return razaPamantului * circumferinta; // Distanța în kilometri
        }
    }
}
