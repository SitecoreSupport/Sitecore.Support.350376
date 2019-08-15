namespace Sitecore.Support.XA.Foundation.Multisite.Pipelines.UiCloneItems
{
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Shell.Framework.Pipelines;
    using Sitecore.Web.UI.Sheer;
    using Sitecore.XA.Foundation.Multisite.Services;
    using System.Linq;

    public class PopulateDelegatedArea
    {
        private readonly IDelegatedAreaService _delegatedAreaService;

        public PopulateDelegatedArea(IDelegatedAreaService delegatedAreaService)
        {
            _delegatedAreaService = delegatedAreaService;
        }

        public void Process(CopyItemsArgs args)
        {
            if (args.Parameters["delegatedArea"] == null)
            {
                return;
            }
            Database database = Factory.GetDatabase(args.Parameters["database"]);
            if (database == null)
            {
                return;
            }
            Item item = database.GetItem(args.Parameters["destination"]);
            if (item != null)
            {
                if (args.Copies != null)
                {
                    Item sharedItem = args.Copies.First();
                    if (!_delegatedAreaService.AddToDelegatedArea(sharedItem, item))
                    {
                        SheerResponse.Alert("This is a delegated area and items cannot be added.");
                    }
                }
            }
        }
    }
}