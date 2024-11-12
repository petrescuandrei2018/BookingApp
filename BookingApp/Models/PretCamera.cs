using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.Models
{
    public class PretCamera : BaseEntity
    {
        [Key]
        public int PretCameraId { get; set; }
        public float PretNoapte {  get; set; }
        public DateTime StartPretCamera {  get; set; }
        public DateTime EndPretCamera { get; set; }
        [ForeignKey("TipCamera")]
        public int TipCameraId {  get; set; }
        public TipCamera TipCamera { get; set; }    

        public PretCamera(int pretCameraId, float pretNoapte, DateTime startPretCamera, DateTime endPretCamera, int tipCameraId)
        {
            PretCameraId = pretCameraId;
            PretNoapte = pretNoapte;
            StartPretCamera = startPretCamera;
            EndPretCamera = endPretCamera;
            TipCameraId = tipCameraId;
        }
    }
}
