using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace Library.Tests
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer("Library.Tests.WpfFactDiscoverer", "Library.Tests")]
    public class WpfFactAttribute : FactAttribute { }
}
