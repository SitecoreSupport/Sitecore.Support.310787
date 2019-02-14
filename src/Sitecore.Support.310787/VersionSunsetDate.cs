namespace Sitecore.Support.Publishing.Service.Delivery.ContentSearch.ComputedFields
{
  using Sitecore.ContentSearch;
  using Sitecore.ContentSearch.ComputedFields;
  using Sitecore.Data.Items;
  using System;

  public class VersionSunsetDate : IComputedIndexField
  {
    public string FieldName
    {
      get;
      set;
    }

    public string ReturnType
    {
      get;
      set;
    }

    public object ComputeFieldValue(IIndexable indexable)
    {
      SitecoreIndexableItem sitecoreIndexableItem = indexable as SitecoreIndexableItem;
      if (sitecoreIndexableItem != null)
      {
        Item item = sitecoreIndexableItem;
        if (item != null)
        {
          DateTime nextVersionSunrise = GetNextVersionSunrise(item);
          return GetSunset(item.Versions.IsLatestVersion(), item.Publishing.ValidTo, nextVersionSunrise);
        }
      }
      return DateTime.MaxValue;
    }

    public DateTime GetSunset(bool currentVersionIsLatest, DateTime currentVersionValidTo, DateTime nextVersionSunrise)
    {
      if (currentVersionIsLatest && (currentVersionValidTo == DateTime.MaxValue || currentVersionValidTo == DateTime.MinValue))
      {
        return DateTime.MaxValue;
      }
      if (currentVersionValidTo != DateTime.MaxValue)
      {
        return (nextVersionSunrise <= currentVersionValidTo) ? nextVersionSunrise : currentVersionValidTo;
      }
      return nextVersionSunrise;
    }

    protected virtual DateTime GetNextVersionSunrise(Item item)
    {
      DateTime maxValue = DateTime.MaxValue;
      Item[] laterVersions = item.Versions.GetLaterVersions();
      foreach (Item item2 in laterVersions)
      {
        if (!item2.Publishing.HideVersion)
        {
          return item2.Publishing.ValidFrom;
        }
      }
      return maxValue;
    }
  }
}