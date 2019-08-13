namespace Sitecore.Support.Shell.Framework.Pipelines
{
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Pipelines;
    using Sitecore.Pipelines.ReplaceItemReferences;
    using Sitecore.Shell.Framework.Pipelines;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    /// <summary>
    /// Defines the clone items class.
    /// </summary>
    public class CloneItems : Sitecore.Shell.Framework.Pipelines.CloneItems
    {
        /// <summary>
        /// Executes the specified args.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <contract>
        /// <requires name="args" condition="not null" />
        /// </contract>
        public override void Execute(CopyItemsArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (args.Parameters["AbortPipeline"] != "true")
            {
                Database database = CopyItems.GetDatabase(args);
                Item item = database.GetItem(args.Parameters["destination"]);
                Assert.IsNotNull(item, args.Parameters["destination"]);
                string value = args.Parameters["deep"];
                if (!bool.TryParse(value, out bool result))
                {
                    result = true;
                }
                ArrayList arrayList = new ArrayList();
                if (Settings.ItemCloning.Enabled)
                {
                    List<Item> items = CopyItems.GetItems(args);
                    DoCloneItems(item, items, arrayList, result);
                }
                args.Copies = (arrayList.ToArray(typeof(Item)) as Item[]);
            }
        }

        /// <summary>
        /// Relink  internal links within cloned subtree.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public new void RelinkClonedSubtree(CopyItemsArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (args.Parameters["AbortPipeline"] != "true")
            {
                NameValueCollection parameters = args.Parameters;
                Assert.IsNotNull(parameters, "parameters");
                if (!Settings.RelinkClonedSubtree)
                {
                    return;
                }
                Item[] copies = args.Copies;
                Assert.IsNotNull(copies, "copies");
                Item[] array = copies;
                foreach (Item item in array)
                {
                    if (item.SourceUri != null)
                    {
                        Item item2 = Database.GetItem(item.SourceUri);
                        if (item2 != null)
                        {
                            ReplaceItemReferencesArgs args2 = new ReplaceItemReferencesArgs
                            {
                                SourceItem = item2,
                                CopyItem = item,
                                Deep = true,
                                Async = false
                            };
                            CorePipeline.Run("replaceItemReferences", args2);
                        }
                    }
                }
            }
        }
    }
}