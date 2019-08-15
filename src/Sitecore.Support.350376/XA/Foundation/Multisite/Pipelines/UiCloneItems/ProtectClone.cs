namespace Sitecore.Support.XA.Foundation.Multisite.Pipelines.UiCloneItems
{
    using Sitecore.Data.Items;
    using Sitecore.Shell.Framework.Pipelines;
    using Sitecore.XA.Foundation.Multisite.Services;
    using System.Linq;

    public class ProtectClone: Sitecore.XA.Foundation.Multisite.Pipelines.UiCloneItems.ProtectClone
    {
        private readonly IDelegatedAreaService _delegatedAreaService;

        public ProtectClone(IDelegatedAreaService delegatedAreaService) : base(delegatedAreaService)
        {
            _delegatedAreaService = delegatedAreaService;
        }

        public new void Process(CopyItemsArgs args)
        {
            if (args.Copies != null)
            {
                Item item = args.Copies.FirstOrDefault();
                if (item != null && _delegatedAreaService.CheckForDelegatedArea(item))
                {
                    ProtectBranch(item);
                }
            }
        }
    }
}