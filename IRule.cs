using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scat
{
    interface IRule
    {
        List<IVulnerability> Test();
    }
}
