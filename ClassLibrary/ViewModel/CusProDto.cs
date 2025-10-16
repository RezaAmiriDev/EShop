

using ModelLayer.ViewModel;

namespace ClassLibrary.ViewModel
{
    public class CusProDto
    {
        public Guid? Id { get; set; }
        public AddressDto? addressDto { get; set; }
        public string? Name { get; set; }
        public string? Family { get; set; }
        public DateTime? Birth { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? NationalCode { get; set; }

    }
}
