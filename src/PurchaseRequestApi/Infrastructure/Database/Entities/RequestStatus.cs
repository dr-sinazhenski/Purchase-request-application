using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Database.Entities
{
    public enum RequestStatus
    {
        Submited,
        Resubmited,
        Approved,
        Rejected,
        FinalReject,
    }
}
