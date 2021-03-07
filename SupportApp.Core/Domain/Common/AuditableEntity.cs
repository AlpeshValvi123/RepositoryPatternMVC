using System;

namespace SupportApp.Core.Domain.Common
{
    public abstract class AuditableEntity : BaseEntity
    {
        public Guid CreatedBy { get; set; }

        public DateTime Created { get; set; }

        public Guid? LastModifiedBy { get; set; }

        public DateTime? LastModified { get; set; }
    }
}
