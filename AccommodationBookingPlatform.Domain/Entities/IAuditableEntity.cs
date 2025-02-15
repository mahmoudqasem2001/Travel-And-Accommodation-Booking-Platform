using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccommodationBookingPlatform.Domain.Entities
{
    public interface IAuditableEntity
    {
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? ModifiedAtUtc { get; set; }
    }
}
