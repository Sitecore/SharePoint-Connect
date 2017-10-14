namespace Sitecore.Sharepoint.Data.Providers
{
  using System;
  using System.Security.Cryptography;
  using System.Text;
  using Sitecore.Data;

  public static class Utils
  {
    /// <summary>
    /// Genarate id for Sharepoint item in CMS.
    /// </summary>
    /// <param name="parentID">
    /// The parent id.
    /// </param>
    /// <param name="childID">
    /// The child id.
    /// </param>
    /// <returns>
    /// Id of Sharepoint item
    /// </returns>
    public static ID GetID(string parentID, string childID)
    {
      MD5 md5 = MD5.Create();
      byte[] hash =
        md5.ComputeHash(
          Encoding.ASCII.GetBytes(
            string.Format("{0}_{1}", parentID, childID)));
      return new ID(new Guid(hash));
    }
 }
}
