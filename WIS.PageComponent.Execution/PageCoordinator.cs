using System;
using System.Collections.Generic;
using System.Data;
using WIS.PageComponent.Execution.Serialization;
namespace WIS.PageComponent.Execution
{
    public class PageCoordinator : IPageCoordinator
    {
        public Dictionary<PageAction, Func<IPageWrapper, IPageController, IPageWrapper>> Actions { get; }

        public PageCoordinator()
        {
            this.Actions = new Dictionary<PageAction, Func<IPageWrapper, IPageController, IPageWrapper>>
            {
                [PageAction.Load] = this.Load,
                [PageAction.Unload] = this.Unload
            };
        }

        public bool IsActionAvailable(PageAction action)
        {
            return this.Actions.ContainsKey(action);
        }

        private IPageWrapper Load(IPageWrapper wrapper, IPageController controller)
        {
            var query = wrapper.GetData<PageContext>();

            IPageWrapper response = new PageWrapper
            {
                PageToken = wrapper.PageToken
            };

            query = controller.PageLoad(query);

            response.SetData(query);

            return response;
        }
        private IPageWrapper Unload(IPageWrapper wrapper, IPageController controller)
        {
            var query = wrapper.GetData<PageContext>();

            IPageWrapper response = new PageWrapper();

            query = controller.PageUnload(query);

            response.SetData(query);

            return response;
        }
    }
}
