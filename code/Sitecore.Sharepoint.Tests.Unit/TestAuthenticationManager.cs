namespace Sitecore.Sharepoint.Tests.Unit
{
  using Sitecore.Abstractions;
  using Sitecore.Security.Accounts;

  [UsedImplicitly]
  public class TestAuthenticationManager : BaseAuthenticationManager
  {
    public override User BuildVirtualUser(string userName, bool isAuthenticated)
    {
      throw new System.NotImplementedException();
    }

    public override bool CheckLegacyPassword(User user, string password)
    {
      throw new System.NotImplementedException();
    }

    public override User GetActiveUser()
    {
      return null;
    }

    public override bool LoginVirtualUser(User user)
    {
      throw new System.NotImplementedException();
    }

    public override bool Login(User user)
    {
      throw new System.NotImplementedException();
    }

    public override bool Login(string userName)
    {
      throw new System.NotImplementedException();
    }

    public override bool Login(string userName, string password)
    {
      throw new System.NotImplementedException();
    }

    public override bool Login(string userName, bool persistent)
    {
      throw new System.NotImplementedException();
    }

    public override bool Login(string userName, string password, bool persistent)
    {
      throw new System.NotImplementedException();
    }

    public override bool Login(string userName, bool persistent, bool allowLoginToShell)
    {
      throw new System.NotImplementedException();
    }

    public override bool Login(string userName, string password, bool persistent, bool allowLoginToShell)
    {
      throw new System.NotImplementedException();
    }

    public override void Logout()
    {
      throw new System.NotImplementedException();
    }

    public override void SetActiveUser(string userName)
    {
      throw new System.NotImplementedException();
    }

    public override void SetActiveUser(User user)
    {
      throw new System.NotImplementedException();
    }

    public override string GetAuthenticationData(string key)
    {
      throw new System.NotImplementedException();
    }

    public override void SetAuthenticationData(string key, string authenticationData)
    {
      throw new System.NotImplementedException();
    }

    public override bool IsAuthenticationTicketExpired()
    {
      throw new System.NotImplementedException();
    }
  }
}