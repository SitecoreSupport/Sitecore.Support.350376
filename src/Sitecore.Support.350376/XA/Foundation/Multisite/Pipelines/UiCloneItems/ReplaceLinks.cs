namespace Sitecore.Support.XA.Foundation.Multisite.Pipelines.UiCloneItems
{
    using Sitecore.Data.Items;
    using Sitecore.Pipelines;
    using Sitecore.Pipelines.ReplaceItemReferences;
    using Sitecore.Shell.Framework.Pipelines;
    using Sitecore.XA.Foundation.Multisite.Services;
    using System.Linq;

    public class ReplaceLinks
    {
        private readonly IDelegatedAreaService _delegatedAreaService;

        public ReplaceLinks(IDelegatedAreaService delegatedAreaService)
        {
            _delegatedAreaService = delegatedAreaService;
        }

        public void Process(CopyItemsArgs args)
        {
            if (args.Copies != null)
            {
                Item item = args.Copies.FirstOrDefault();
                if (item != null && _delegatedAreaService.CheckForDelegatedArea(item))
                {
                    CorePipeline.Run("replaceItemReferences", new ReplaceItemReferencesArgs
                    {
                        SourceItem = item.Source,
                        CopyItem = item,
                        Deep = true,
                        Async = false
                    });
                }
            }
        }
    }
}