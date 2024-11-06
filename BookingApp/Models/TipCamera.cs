﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.Models
{
    public class TipCamera : BaseEntity
    {
        [Key]
        public int TipCameraId { get; set; }
        public string Name { get; set; }
        public int CapacitatePersoane {  get; set; }
        public int NrTotalCamere { get; set; }  
        public int NrCamereDisponibile { get; set;}
        public int NrCamereOcupate { get; set; }
        public ICollection<HotelTipCamera> HotelSiTipCamere { get; set; }

        public TipCamera(int tipCameraId, string name, int capacitatePersoane,
            int nrTotalCamere, int nrCamereDisponibile, int nrCamereOcupate)
        {
            TipCameraId = tipCameraId;
            Name = name;
            CapacitatePersoane = capacitatePersoane;
            NrTotalCamere = nrTotalCamere;
            NrCamereDisponibile = nrCamereDisponibile;
            NrCamereOcupate = nrCamereOcupate; 
        }
    }
}
