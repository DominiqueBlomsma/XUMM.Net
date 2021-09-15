﻿using System.ComponentModel.DataAnnotations;

namespace XUMM.Net.Enums
{
    public enum XummKycStatus
    {
        [Display(Name = "NONE")]
        None,

        [Display(Name = "IN_PROGRESS")]
        InProgress,

        [Display(Name = "REJECTED")]
        Rejected,

        [Display(Name = "SUCCESSFUL")]
        Successful
    }
}
