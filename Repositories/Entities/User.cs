﻿using Microsoft.AspNetCore.Identity;

namespace Repositories.Entities
{
    public class User : IdentityUser<int>
    {
        public string? FullName { get; set; }
        public string? UnsignFullName { get; set; } = "";
        public DateTime? Dob { get; set; }
        public bool? Gender { get; set; }
        public string? Image { get; set; } = "";
        public string? University { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? DeletionDate { get; set; }
        public int? DeleteBy { get; set; }
        public bool? IsDeleted { get; set; } = false;


        public virtual Wallet Wallet { get; set; }
    }
}
