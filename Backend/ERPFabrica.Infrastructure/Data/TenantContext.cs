using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPFabrica.Infrastructure.Data
{
    public static class TenantContext
    {
        private static readonly AsyncLocal<int> _currentTenantId = new();

        public static int CurrentTenantId
        {
            get => _currentTenantId.Value;
            set => _currentTenantId.Value = value;
        }
    }
}
