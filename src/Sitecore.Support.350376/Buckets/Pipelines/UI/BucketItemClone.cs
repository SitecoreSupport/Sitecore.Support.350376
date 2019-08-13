namespace Sitecore.Support.Buckets.Pipelines.UI
{
    using Sitecore;
    using Sitecore.Buckets.Managers;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Shell;
    using Sitecore.Shell.Applications.Dialogs.ProgressBoxes;
    using Sitecore.Shell.Framework.Pipelines;
    using Sitecore.Web.UI.Sheer;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class BucketItemClone : CloneItems
    {
        public new void Execute(CopyItemsArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            string text = args.Parameters["destination"];
            if (!string.IsNullOrEmpty(text))
            {
                Item item = ExtractDatabase(args).GetItem(text);
                if (item != null && (IsBucket(item) || ShouldBeHandledWithinItemBuckets(ExtractItems(args))))
                {
                    ExecuteSync("Cloning Items", "-/icon/Core3/32x32/copy_to_folder.png", StartCloning, EndCloning);
                    args.AbortPipeline();
                }
            }
        }

        internal void StartCloning(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            CopyItemsArgs copyItemsArgs = args as CopyItemsArgs;
            if (copyItemsArgs == null)
            {
                return;
            }
            string text = args.Parameters["destination"];
            if (!string.IsNullOrEmpty(text))
            {
                List<Item> source = ExtractItems(copyItemsArgs);
                Database database = ExtractDatabase(copyItemsArgs);
                Item destination = database.GetItem(text);
                List<Item> list = (from item in source
                                   where item != null
                                   select CloneItem(item, destination, deep: true)).ToList();
                if (copyItemsArgs.Copies == null)
                {
                    copyItemsArgs.Copies = list.ToArray();
                }
                else
                {
                    copyItemsArgs.Copies = copyItemsArgs.Copies.Concat(list).ToArray();
                }
            }
        }

        private void EndCloning(ClientPipelineArgs args)
        {
            CopyItemsArgs copyItemsArgs = args as CopyItemsArgs;
            if (copyItemsArgs != null)
            {
                List<Item> items = CopyItems.GetItems(copyItemsArgs);
                if (copyItemsArgs.Copies != null && items.Any() && UserOptions.View.ShowBucketItems && items.Count == 1 && copyItemsArgs.Copies.Length != 0)
                {
                    Context.ClientPage.SendMessage(this, "item:load(id=" + copyItemsArgs.Copies[0].ID + ")");
                }
            }
        }

        protected virtual Item CloneItem(Item source, Item target, bool deep)
        {
            return BucketManager.CloneItem(source, target, deep);
        }

        protected virtual Database ExtractDatabase(CopyItemsArgs args)
        {
            return CopyItems.GetDatabase(args);
        }

        protected virtual bool IsBucket(Item item)
        {
            return BucketManager.IsBucket(item);
        }

        protected virtual bool ShouldBeHandledWithinItemBuckets(List<Item> items)
        {
            return ShouldBeHandledWithinItemBucketsStatic(items);
        }

        protected virtual List<Item> ExtractItems(CopyItemsArgs args)
        {
            return CopyItems.GetItems(args);
        }

        protected virtual void ExecuteSync(string jobName, string icon, Action<ClientPipelineArgs> action, Action<ClientPipelineArgs> postAction)
        {
            ProgressBox.ExecuteSync(jobName, icon, action, postAction);
        }

        internal static bool ShouldBeHandledWithinItemBucketsStatic(IEnumerable<Item> items)
        {
            Assert.ArgumentNotNull(items, "items");
            return items.Any(delegate (Item item)
            {
                if (!BucketManager.IsBucket(item))
                {
                    return BucketManager.IsItemContainedWithinBucket(item);
                }
                return true;
            });
        }
    }
}