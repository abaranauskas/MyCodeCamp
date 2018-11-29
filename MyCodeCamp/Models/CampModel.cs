using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCodeCamp.Models
{
    public class CampModel
    {
        public string Url { get; set; }

        [Required, MinLength(3), MaxLength(20)]
        public string Moniker { get; set; }

        [Required, MinLength(5), MaxLength(100)]
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Length { get; set; }

        [Required, MinLength(25), MaxLength(4096)]
        public string Description { get; set; }

        public string LocaltionAddress1 { get; set; }
        public string LocaltionAddress2 { get; set; }
        public string LocaltionAddress3 { get; set; }
        public string LocaltionCityTown { get; set; }
        public string LocaltionStateProvince { get; set; }
        public string LocaltionPostalCode { get; set; }
        public string LocaltionCountry { get; set; }
    }
}
