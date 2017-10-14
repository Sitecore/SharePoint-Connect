<%@ Page Language="C#" AutoEventWireup="true" Codebehind="Intranet.Main.aspx.cs" Inherits="Sitecore.Intranet.Layouts.Intranet_Main" %>
<%@ Register TagPrefix="sc" Namespace="Sitecore.Web.UI.WebControls" Assembly="Sitecore.Kernel" %>
<%@ Register TagPrefix="sc" Namespace="Sitecore.Web.UI.WebControls" Assembly="Sitecore.Analytics" %>
<%@ Register TagPrefix="intranetOverride" Namespace="Sitecore.Intranet.HtmlControls" Assembly="Sitecore.Intranet" %>
<%@ Register Src="..\Intranet.PersonalContext.ascx" TagName="IntranetPersonalContext" TagPrefix="intranet" %>
<%@ Register Src="..\Intranet.FrontendEditing.EditIcon.ascx" TagName="FrontendEditingEditIcon" TagPrefix="editing" %>
<%@ Register Src="..\Intranet.FrontendEditing.AddNewIcon.ascx" TagName="FrontendEditingAddNewIcon" TagPrefix="editing" %>
<%@ Register Src="..\Intranet.ContentLanguages.ascx" TagName="ContentLanguages" TagPrefix="cl" %>
<%@ Register Src="..\Intranet.FrontendEditing.ContentLanguages.ascx" TagName="EditorLanguages" TagPrefix="cl" %>
<%@ Register Src="..\Intranet.Skin.ascx" TagName="Skin" TagPrefix="intranet" %>
<%@ Register TagPrefix="intranet" Namespace="Sitecore.Intranet.WebControls" Assembly="Sitecore.Intranet" %>
<%@ OutputCache Location="None" VaryByParam="none" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >
<html>
<head>
   <title><intranet:LocalizedText Field="browsertitle" runat="server" /></title>
   <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
   <meta name="CODE_LANGUAGE" content="C#">
   <meta name="vs_defaultClientScript" content="JavaScript">
   <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
   <intranet:Skin ID="IntranetSkin1" runat="server" />
   <sc:VisitorIdentification runat="server" />

   <script src="/script/intranet.js" type="text/javascript"></script>
   <script type="text/javascript">

   // Top menu variables
   var menuwidth='<intranet:SkinnedText field="DropDownWidth" runat="server" />' //default menu width
   var menubgcolor='<intranet:SkinnedText field="DropDownBgColor" runat="server" />'  //menu bgcolor
   var disappeardelay='<intranet:SkinnedText field="DropDownDisappearDelay" runat="server" />';  //menu disappear speed onMouseout (in miliseconds)
   var hidemenu_onclick="yes" //hide menu when user clicks within menu?

   </script>
   <script src="/script/Intranet.TopMenu.js" type="text/javascript"></script>

</head>
<body id="mainpage"><form id="mainform" method="post" runat="server">
<asp:ScriptManager ID="spListScriptManager" runat="server">
   </asp:ScriptManager>
      <div style="width:100%; text-align:center" id="ctl14">
        <div class="SiteContentCentered" id="ctl15">
          <sc:placeholder runat="server" key="webedit"></sc:placeholder>
        </div>
      </div>
      <table class="maintable SiteContentCentered" height="450" cellspacing="0" cellpadding="0" border="0" align="center" id="ctl1">
         <tbody>
            <tr>
               <td class="maintabletopcell" colspan="5" id="ctl3">
                  <sc:xslfile runat="server" id="XslFile2" path="/xsl/intranet.topsearch.xslt" cacheable="true" varybyuser="true"></sc:xslfile>
                  <sc:xslfile runat="server" id="XslFile3" path="/xsl/intranet.top.xslt"></sc:xslfile>
               </td>
            </tr>
            <tr>
               <td class="maintabletopmenucell" colspan="5" id="ctl4">
                  <sc:xslfile runat="server" id="XslFile1" path="/xsl/Intranet.TopMenu.xslt" cacheable="true" varybyuser="true" varybydata="true"></sc:xslfile>
               </td>
            </tr>
            <tr class="maincontentarearow" valign="top">
               <td class="maincontentarealeft" rowspan="2" id="ctl5">
           <sc:placeholder runat="server" key="content-left"></sc:placeholder></td>
               <td class="maincontentareadots" width="1" rowspan="2" id="ctl6"><img alt="" width="1" height="1" src="/sitecore/images/blank.gif?w=1&amp;h=1&amp;as=1" border="0" /></td>
               <td class="maincontentareacenter" id="ctl7">
                  <div class="maincontentareacenterdiv" id="ctl26">
                     <sc:xslfile runat="server" path="/xsl/intranet.breadcrumbs.xslt"></sc:xslfile>
                     <table width="100%" cellpadding="0" cellspacing="0" border="0" id="ctl2">
                        <tbody>
                           <tr>
                              <td id="ctl8">
                                 <cl:contentlanguages runat="server" id="contentLanguages" xmlns:cl="http://www.sitecore.net/xhtml"></cl:contentlanguages>
                              </td>
                              <td id="ctl9">
                                 <sc:xslfile runat="server" path="/xsl/intranet.sitetools.xslt"></sc:xslfile>
                                 <div style="text-align: right" id="ctl33">
                                    <editing:frontendeditingaddnewicon runat="server" id="FrontendEditingAddNewIcon1" xmlns:editing="http://www.sitecore.net/xhtml"></editing:frontendeditingaddnewicon>
                                    <editing:frontendeditingediticon runat="server" id="FrontendEditingEditIcon1" xmlns:editing="http://www.sitecore.net/xhtml"></editing:frontendeditingediticon>
                                    <cl:editorlanguages runat="server" id="editorLanguages1" xmlns:cl="http://www.sitecore.net/xhtml"></cl:editorlanguages>
                                 </div>
                              </td>
                           </tr>
                        </tbody>
                     </table>
                                         <br style="clear: both" />
                     <intranetoverride:localizedplaceholder runat="server" id="plhContent" key="content" xmlns:intranetoverride="http://www.sitecore.net/xhtml"></intranetoverride:localizedplaceholder>
                  </div>
                  <br style="clear: both" />
               </td>
               <td class="maincontentareadots" width="1" rowspan="2" id="ctl10"><img alt="" width="1" height="1" src="/sitecore/images/blank.gif?w=1&amp;h=1&amp;as=1" border="0" /></td>
               <td class="maincontentarearight" rowspan="2" id="ctl11">
                  <intranet:intranetpersonalcontext runat="server" id="IntranetPersonalContext1" xmlns:intranet="http://www.sitecore.net/xhtml"></intranet:intranetpersonalcontext>
                  <br />
                  <intranet:quickpoll runat="server" allowvote="true" debug="true" id="mainPoll" barheight="7" baremptycolor="lightgray" barcolor="#333333" xmlns:intranet="http://www.sitecore.net/xhtml"></intranet:quickpoll>
                  <sc:xslfile runat="server" path="/xsl/intranet.context.xslt"></sc:xslfile>
               <sc:placeholder runat="server" key="content-right"></sc:placeholder></td>
            </tr>
            <tr>
               <td class="MainContentAuthorRow" id="ctl12">
                  <sc:xslfile runat="server" id="xslAuthor" path="/xsl/intranet.author.xslt"></sc:xslfile>
               </td>
            </tr>
            <tr>
               <td class="MainBottomCell" colspan="5" id="ctl13">
                  <sc:xslfile runat="server" path="/xsl/intranet.footer.xslt"></sc:xslfile>
               </td>
            </tr>
         </tbody>
      </table>
   </form></body>
</html>
