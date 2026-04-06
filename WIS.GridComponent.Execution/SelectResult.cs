using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Components.Common.Select;

namespace WIS.GridComponent.Execution
{
    public class SelectResult
    {
        public List<SelectOption> Options { get; private set; }
        public bool MoreResultsAvailable { get; private set; }
        public int ResultLimit { get; private set; }

        public SelectResult(int resultLimit)
        {
            this.ResultLimit = resultLimit;
        }

        public void SetOptions(List<SelectOption> options)
        {
            this.Options = options?.Take(this.ResultLimit).ToList();
            this.MoreResultsAvailable = options.Count > this.ResultLimit;
        }

        public int GetUsableResultLimit()
        {
            return this.ResultLimit + 1;
        }
    }
}
