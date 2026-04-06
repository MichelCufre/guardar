using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WIS.Components.Common.Select
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
            this.MoreResultsAvailable = options != null ? options.Count > this.ResultLimit : false;
        }

        public int GetUsableResultLimit()
        {
            return this.ResultLimit + 1;
        }
    }
}
