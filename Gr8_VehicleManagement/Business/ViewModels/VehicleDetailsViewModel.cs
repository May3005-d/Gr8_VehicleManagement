using Gr8_VehicleManagement.Business.DTOs;

namespace Gr8_VehicleManagement.Business.ViewModels
{
    public class VehicleDetailsViewModel
    {
        public VehicleModelDto Model { get; set; } = null!;
        public List<VehicleVersionDto> Versions { get; set; } = new List<VehicleVersionDto>();
        public Dictionary<string, object>? Specifications { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }
}

