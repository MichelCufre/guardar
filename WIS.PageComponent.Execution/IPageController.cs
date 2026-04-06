using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.PageComponent.Execution
{
    public interface IPageController
    {
        PageContext PageLoad(PageContext data);
        PageContext PageUnload(PageContext data);
    }
}
