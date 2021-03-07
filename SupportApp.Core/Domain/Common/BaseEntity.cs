using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportApp.Core.Domain.Common
{
    public abstract class BaseEntity
    {
        public BaseEntity()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
    }
}
