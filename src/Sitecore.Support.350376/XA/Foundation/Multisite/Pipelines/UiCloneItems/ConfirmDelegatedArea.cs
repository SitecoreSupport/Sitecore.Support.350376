namespace Sitecore.Support.XA.Foundation.Multisite.Pipelines.UiCloneItems
{
    using Sitecore.Data.Items;
    using Sitecore.Globalization;
    using Sitecore.Web.UI.Sheer;
    using Sitecore.XA.Foundation.Multisite;
    using Sitecore.XA.Foundation.Multisite.Services;
    using Sitecore.XA.Foundation.SitecoreExtensions.Extensions;
    using Sitecore.XA.Foundation.SitecoreExtensions.Repositories;
    using System.Collections.Generic;
    using System.Linq;

    public class ConfirmDelegatedArea : Sitecore.XA.Foundation.Multisite.Pipelines.UiCloneItems.ConfirmDelegatedArea
    {
        private readonly IMultisiteContext _multisiteContext;

        public ConfirmDelegatedArea(IDelegatedAreaService delegatedAreaService, IMultisiteContext multisiteContext, IDatabaseRepository databaseRepository) : base(delegatedAreaService, multisiteContext, databaseRepository)
        {
            _multisiteContext = multisiteContext;
        }

        public new void Process(ClientPipelineArgs args)
        {
            Item item = GetDatabase(args)?.GetItem(args.Parameters["destination"]);
            if (item == null)
            {
                return;
            }
            IList<Item> items = GetItems(args);
            if (!items.Any())
            {
                return;
            }
            Item item2 = items.FirstOrDefault();
            if ((item2 != null && !item2.InheritsFrom(Templates.Page.ID)) || !BelongsToSameTenant(items.FirstOrDefault(), item, _multisiteContext) || !DestinationIsUnderSite(item, _multisiteContext))
            {
                return;
            }
            if (args.IsPostBack)
            {
                if (args.HasResult && args.Result == "yes")
                {
                    args.Parameters.Add("delegatedArea", "true");
                    args.IsPostBack = false;
                }
            }
            else
            {
                YesNo(Translate.Text("Do you want to make this clone and its children an SXA delegated area?") + "<br/><br/>" + Translate.Text("Sharing content as a delegated area lets you control the content from one central location in the content tree.  When the clone is part of an SXA delegated area, it is managed by SXA, and cannot be changed locally. "), "500", "280", preformatted: false);
                args.WaitForPostBack();
            }
        }
    }
}