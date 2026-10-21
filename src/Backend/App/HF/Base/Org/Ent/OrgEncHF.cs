using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Encoded class of Organisation entity
/// Created: June 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Org.Ent
{
    [Keyless]
    public class OrgEncHF : OrgEnc
    {
       
        /// <summary>
        /// Test.
        /// </summary>
        [NotMapped]
        public bool IHfEnabled { get; set; } = true;

        /// <summary>
        /// Test 1.
        /// </summary>
        [NotMapped]
        public bool IHfEnabled1 { get; set; } = false;

    }

}
